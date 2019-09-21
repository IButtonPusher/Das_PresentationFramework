using System;
using Das.OpenGL.Text;
using Das.Views.Core.Writing;
using Das.Views.Winforms;

namespace Das.OpenGL.Windows
{
    public class WindowsFontProvider : FontProvider
    {
        private readonly IGLContext _context;

        public WindowsFontProvider(IGLContext context) 
            : base(context)
        {
            _context = context;
        }

        protected override FontBitmapEntry CreateFontBitmapEntry(IFont font)
        {
            var dcHandle = _context.DeviceContextHandle;
            var r = _context.RenderContextHandle;

            GLWindows.wglMakeCurrent(dcHandle, r);

            var fontHeight = (Int32)(font.Size * (20.0f / 12.0f));
            var weight = GetFontWeight(font);

            //  Create the font based on the face name.
            var hFont = Native.CreateFont(fontHeight, 0, 0, 0, (UInt32)weight, 0, 0, 0, 
                Native.DEFAULT_CHARSET,
                Native.OUT_OUTLINE_PRECIS, Native.CLIP_DEFAULT_PRECIS, Native.CLEARTYPE_QUALITY,
                Native.VARIABLE_PITCH, font.FamilyName);

            
            var hOldObject = Native.SelectObject(dcHandle, hFont);
            var listBase = GL.glGenLists(1);
            GLWindows.wglUseFontBitmaps(dcHandle, 0, 255, listBase);
            
            Native.SelectObject(dcHandle, hOldObject);
            Native.DeleteObject(hFont);

            var fbe = new FontBitmapEntry(font, _context, listBase, 255);

            return fbe;
        }

        private static FontWeights GetFontWeight(IFont font)
        {
            switch (font.FontStyle)
            {
                case FontStyle.Regular:
                    return FontWeights.FW_NORMAL;
                case FontStyle.Bold:
                    return FontWeights.FW_BOLD;
                default:
                    throw new NotImplementedException();
            }
        }

    }
}
