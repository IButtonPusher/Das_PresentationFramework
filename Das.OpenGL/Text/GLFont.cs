using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Das.Extensions;
using Das.OpenGL.Text.FreeType;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;

namespace Das.OpenGL.Text
{
    public class GLFont : IFontRenderer
    {
        public GLFont(IFont font, String fontFile, UInt32 size, IGLContext context)
        {
            Font = font;
            _fontSize = size;
            _context = context;
            _wordRects = new Dictionary<String, Rectangle>();

            _charGlyphs = new Dictionary<Int32, Glyph>();

            var ret = FT.FT_Init_FreeType(out var library);
            if (ret != FtErrors.Ok) throw new Exception("FreeType library Exception: " + ret);

            ret = FT.FT_New_Face(library, fontFile, 0, out _facePtr);
            if (ret != FtErrors.Ok) throw new Exception("FreeType font Exception: " + ret);

            _ftFont = (FreeTypeFont) Marshal.PtrToStructure(_facePtr, typeof(FreeTypeFont));
            _hasKerning = ((FaceFlags) _ftFont.face_flags & FaceFlags.Kerning) == FaceFlags.Kerning;

            var w = IntPtr.Zero;
            var h = (IntPtr) (size << 6);
            FT.FT_Set_Char_Size(_facePtr, w, h, 0, 96);

            _textures = new UInt32[128];
            _extentX = new Int32[128];
            _listBase = GL.glGenLists(128);
            GL.glGenTextures(128, _textures);
            for (var c = 0; c < 128; c++)
                CompileCharacter(_facePtr, c);
        }

        public IFont Font { get; }

        public void DrawStringInRect<TBrush, TRectangle>(String s,
                                                         TBrush brush,
                                                         TRectangle bounds)
            where TBrush : IBrush
            where TRectangle : IRectangle
        {
            DrawString(s, brush, bounds.TopLeft);
        }

        public ValueSize MeasureString(String text)
        {
            Single w = 0;
            var isUnderrun = false;
            Single overrun = 0;
            Single underrun = 0;
            var top = 0f;
            Single bottom = 0;

            var glyphIndex = FT.FT_Get_Char_Index(_facePtr, text[0]);

            for (var i = 0; i < text.Length; i++)
            {
                var glyph = _charGlyphs[text[i]];

                var gAdvanceX = (Single) glyph.Advance.x;
                var gBearingX = glyph.HorizontalBearingX;
                var gWidth = glyph.Width;

                underrun += -gBearingX;
                if (w.AreEqualEnough(0))
                    w += underrun;

                if (!isUnderrun && underrun <= 0)
                {
                    underrun = 0;
                    isUnderrun = true;
                }


                if (gBearingX + gWidth > 0 || gAdvanceX > 0)
                {
                    overrun -= Math.Max(gBearingX + gWidth, gAdvanceX);
                    if (overrun <= 0) overrun = 0;
                }

                overrun += gBearingX.AreEqualEnough(0) &&
                           gWidth.AreEqualEnough(0)
                    ? 0
                    : gBearingX + gWidth - gAdvanceX;

                if (i == text.Length - 1)
                    w += overrun;

                var glyphTop = glyph.HorizontalBearingY;
                var glyphBottom = glyph.Height - glyph.HorizontalBearingY;
                if (glyphTop > top)
                    top = glyphTop;
                if (glyphBottom > bottom)
                    bottom = glyphBottom;

                w += gAdvanceX;

                if (!_hasKerning || i >= text.Length - 1)
                    continue;

                var cNext = text[i + 1];
                var wasIndex = glyphIndex;
                glyphIndex = FT.FT_Get_Char_Index(_facePtr, cNext);
                var err = FT.FT_Get_Kerning(_facePtr, wasIndex, glyphIndex, (UInt32) KerningMode.Default,
                    out var kern);

                var kerning = (Single) kern.X;

                if (err != FtErrors.Ok)
                    throw new InvalidOperationException(err.ToString());

                if (kerning > gAdvanceX * 5 || kerning < -(gAdvanceX * 5))
                    kerning = 0;

                w += kerning;
            }

            _wordRects[text] = new Rectangle(0, top, w, bottom);

            var h = Math.Max(top + bottom, _maxHeight + bottom);

            return new ValueSize(w, h);
        }

        public  void DrawString<TBrush, TPoint>(String text,
                                                TBrush brush,
                                                TPoint point2D)
            where TBrush : IBrush
            where TPoint : IPoint2D
        {
            if (!_wordRects.TryGetValue(text, out var wordRect))
            {
                MeasureString(text);
                wordRect = _wordRects[text];
            }

            var font = _listBase;


            if (!(brush is SolidColorBrush scb))
                throw new NotImplementedException();

            var r = scb.Color.R;
            var g = scb.Color.G;
            var b = scb.Color.B;

            var y = _context.Size.Height - point2D.Y;


            GL.glPushAttrib(GL.TRANSFORM_BIT);
            var viewport = new Int32[4];
            GL.glGetIntegerv(GL.VIEWPORT, viewport);
            GL.glMatrixMode(GL.PROJECTION);
            GL.glPushMatrix();
            GL.glLoadIdentity();
            GL.glOrtho(viewport[0], viewport[2], viewport[1], viewport[3], -1, 1);

            GL.glPopAttrib();

            GL.glPushAttrib(GL.LIST_BIT | GL.CURRENT_BIT | GL.ENABLE_BIT | GL.TRANSFORM_BIT);
            GL.glMatrixMode(GL.MODELVIEW);
            GL.glDisable(GL.LIGHTING);
            GL.glEnable(GL.TEXTURE_2D);
            GL.glDisable(GL.DEPTH_TEST);
            GL.glEnable(GL.BLEND);
            GL.glBlendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
            GL.glListBase(font);
            var modelview_matrix = new Single[16];
            GL.glGetFloatv(GL.MODELVIEW_MATRIX, modelview_matrix);
            GL.glPushMatrix();
            GL.glColor3f(r, g, b);
            GL.glLoadIdentity();

            var bottom = wordRect.Height;

            var gap = _maxHeight - _fontSize + wordRect.Height;
            var offset = y - _fontSize - gap + bottom;

            GL.glTranslated(point2D.X, offset, 0);
            GL.glMultMatrixf(modelview_matrix);

            var textbytes = new Byte[text.Length];
            for (var i = 0; i < text.Length; i++)
                textbytes[i] = (Byte) text[i];
            GL.glCallLists(text.Length, GL.UNSIGNED_BYTE, textbytes);

            GL.glPopMatrix();
            GL.glPopAttrib();

            GL.glPushAttrib(GL.TRANSFORM_BIT);
            GL.glMatrixMode(GL.PROJECTION);
            GL.glPopMatrix();
            GL.glPopAttrib();
        }


        public void Dispose()
        {
            FT.FT_Done_Face(_facePtr);

            GL.glDeleteLists(_listBase, 128);
            GL.glDeleteTextures(128, _textures);
            //_textures = null;
            //_extentX = null;
        }

        public void CompileCharacter(IntPtr face, Int32 c)
        {
            //  We first convert the number index to a character index
            var index = FT.FT_Get_Char_Index(face, Convert.ToChar(c));

            //  Here we load the actual glyph for the character
            var ret = FT.FT_Load_Glyph(face, index, (Int32) FtLoadFlags.Default);
            if (ret != 0)
                return;

            var slot2 = (GlyphSlotRec) Marshal.PtrToStructure(_ftFont.glyph, typeof(GlyphSlotRec));

            ret = FT.FT_Get_Glyph(_ftFont.glyph, out var glyph);
            if (ret != FtErrors.Ok) throw new Exception("FreeType character Exception for " + c + ": " + ret);

            FT.FT_Glyph_To_Bitmap(out glyph, FontModes.Normal, 0, 1);

            var glyph_bmp = (FtBitmapGlyph) Marshal.PtrToStructure(glyph, typeof(FtBitmapGlyph));
            var glyphItemB = new Glyph(slot2, (Char) c);

            _maxHeight = Math.Max(_maxHeight, glyphItemB.Height);

            _charGlyphs[c] = glyphItemB;

            var size = glyph_bmp.bitmap.width * glyph_bmp.bitmap.rows;
            if (size <= 0)
            {
                _extentX[c] = 0;
                if (c == SPACE)
                {
                    GL.glNewList((UInt32) (_listBase + c), GL.COMPILE);
                    GL.glTranslatef(_fontSize >> 1, 0, 0);
                    _extentX[c] = (Int32) (_fontSize >> 1);
                    GL.glEndList();
                }

                return;
            }

            var bmp = new Byte[size];

            Marshal.Copy(glyph_bmp.bitmap.buffer, bmp, 0, bmp.Length);

            var width = GetSideLength(glyph_bmp.bitmap.width);
            var height = GetSideLength(glyph_bmp.bitmap.rows);
            var expanded = new Byte[2 * width * height];
            for (var j = 0; j < height; j++)
            for (var i = 0; i < width; i++)
                expanded[2 * (i + j * width)] = expanded[2 * (i + j * width) + 1] =
                    i >= glyph_bmp.bitmap.width || j >= glyph_bmp.bitmap.rows
                        ? (Byte) 0
                        : bmp[i + glyph_bmp.bitmap.width * j];

            GL.glBindTexture(GL.TEXTURE_2D, _textures[c]);
            GL.glTexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MAG_FILTER, (Int32) GL.LINEAR);
            GL.glTexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, (Int32) GL.LINEAR);

            GL.glTexImage2D(GL.TEXTURE_2D, 0, GL.RGBA, width, height,
                0, GL.LUMINANCE_ALPHA, GL.UNSIGNED_BYTE, expanded);

            GL.glNewList((UInt32) (_listBase + c), GL.COMPILE);
            GL.glBindTexture(GL.TEXTURE_2D, _textures[c]);

            GL.glTranslatef(glyph_bmp.left, 0, 0);
            GL.glPushMatrix();
            GL.glTranslatef(0, glyph_bmp.top - glyph_bmp.bitmap.rows, 0);
            var x = glyph_bmp.bitmap.width / (Single) width;
            var y = glyph_bmp.bitmap.rows / (Single) height;


            GL.glBegin(GL.QUADS);
            GL.glTexCoord2d(0, 0);
            GL.glVertex2f(0, glyph_bmp.bitmap.rows);
            GL.glTexCoord2d(0, y);
            GL.glVertex2f(0, 0);
            GL.glTexCoord2d(x, y);
            GL.glVertex2f(glyph_bmp.bitmap.width, 0);
            GL.glTexCoord2d(x, 0);
            GL.glVertex2f(glyph_bmp.bitmap.width, glyph_bmp.bitmap.rows);
            GL.glEnd();
            GL.glPopMatrix();

            GL.glTranslatef(glyph_bmp.bitmap.width, 0, 0);
            _extentX[c] = glyph_bmp.left + glyph_bmp.bitmap.width;
            GL.glEndList();
        }

        public override String ToString()
        {
            return Font.ToString();
        }


        private static Int32 GetSideLength(Int32 a)
        {
            var rval = 1;
            while (rval < a) rval <<= 1;
            return rval;
        }

        private const Int32 SPACE = 32;

        private readonly Dictionary<Int32, Glyph> _charGlyphs;
        private readonly IGLContext _context;
        private readonly IntPtr _facePtr;
        private readonly UInt32 _fontSize;

        private readonly FreeTypeFont _ftFont;
        private readonly Boolean _hasKerning;
        private readonly UInt32 _listBase;
        private readonly Dictionary<String, Rectangle> _wordRects;
        private readonly Int32[] _extentX;

        private Single _maxHeight;
        private readonly UInt32[] _textures;
    }
}