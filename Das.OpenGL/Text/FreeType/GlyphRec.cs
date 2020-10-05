using Das.Views.Core.Geometry;
using System;
using System.Runtime.InteropServices;

namespace Das.OpenGL.Text.FreeType
{
    [StructLayout(LayoutKind.Sequential)]
    public class GlyphRec
    {
        public IntPtr library;
        public IntPtr clazz;
        public int format;
        public Vector advance;
    }
}
