using System.Runtime.InteropServices;

namespace Das.OpenGL.Text.FreeType
{
    [StructLayout(LayoutKind.Sequential)]
    public class FtBitmapGlyph
    {
        public GlyphRec root;
        public int left;
        public int top;
        public FtBitmap bitmap;
    }
}