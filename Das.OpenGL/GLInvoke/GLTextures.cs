using System.Runtime.InteropServices;

namespace Das.OpenGL
{
    public static partial class GL
    {
        public const uint TEXTURE_2D = 0x0DE1;
        public const uint TEXTURE_MAG_FILTER = 0x2800;
        public const uint TEXTURE_MIN_FILTER = 0x2801;

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glGenTextures(int n, uint[] textures);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glBindTexture(uint target, uint texture);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glTexParameteri(uint target, uint pname, int param);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glTexImage2D(uint target, int level, uint internalformat,
            int width, int height, int border, uint format, uint type, byte[] pixels);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glTexCoord2d(double s, double t);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glDeleteTextures(int n, uint[] textures);
    }
}
