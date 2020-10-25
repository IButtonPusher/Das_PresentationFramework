using System.Runtime.InteropServices;
// ReSharper disable UnassignedField.Global
#pragma warning disable 8618

namespace Das.OpenGL.Text.FreeType
{
    [StructLayout(LayoutKind.Sequential)]
    public class FtBitmapGlyph
    {
        // ReSharper disable once UnusedMember.Global
        public GlyphRec root;
        public int left;
        public int top;
        public FtBitmap bitmap;
    }
}