using System;
using System.Runtime.InteropServices;

namespace Das.OpenGL
{
    public static partial class GL
    {
        public const UInt32 TEXTURE_2D = 0x0DE1;
        public const UInt32 TEXTURE_MAG_FILTER = 0x2800;
        public const UInt32 TEXTURE_MIN_FILTER = 0x2801;

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glGenTextures(Int32 n, UInt32[] textures);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glBindTexture(UInt32 target, UInt32 texture);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glTexParameteri(UInt32 target, UInt32 pname, Int32 param);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glTexImage2D(UInt32 target, Int32 level, UInt32 internalformat,
            Int32 width, Int32 height, Int32 border, UInt32 format, UInt32 type, Byte[] pixels);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glTexCoord2d(Double s, Double t);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glDeleteTextures(Int32 n, UInt32[] textures);
    }
}
