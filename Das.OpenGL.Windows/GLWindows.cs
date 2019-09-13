using System;
using System.Runtime.InteropServices;

namespace Das.OpenGL.Windows
{
    public static class GLWindows
    {
        public const string OpenGL32 = "opengl32.dll";

        [DllImport(OpenGL32, SetLastError = true)]
        public static extern int wglDeleteContext(IntPtr hrc);

        [DllImport(OpenGL32, SetLastError = true)]
        public static extern IntPtr wglCreateContext(IntPtr hdc);

        [DllImport(OpenGL32, SetLastError = true)]
        public static extern int wglMakeCurrent(IntPtr hdc, IntPtr hrc);

        [DllImport(OpenGL32, SetLastError = true)]
        public static extern bool wglUseFontBitmaps(IntPtr hDC, uint first, 
            uint count, uint listBase);
    }

    public delegate void glRenderbufferStorageEXT(uint target, uint internalformat,
        int width, int height);

    public delegate void glBindRenderbufferEXT(uint target, uint renderbuffer);


    public delegate void glGenRenderbuffersEXT(uint n, uint[] renderbuffers);

    public delegate void glGenFramebuffersEXT(uint n, uint[] framebuffers);

    public delegate void glBindFramebufferEXT(uint target, uint framebuffer);

    public delegate void glFramebufferRenderbufferEXT(uint target, uint attachment,
        uint renderbuffertarget, uint renderbuffer);

    public delegate void glDeleteRenderbuffersEXT(uint n, uint[] renderbuffers);
    public delegate void glDeleteFramebuffersEXT(uint n, uint[] framebuffers);

    public delegate IntPtr wglCreateContextAttribsARB(IntPtr hDC, IntPtr hShareContext,
        int[] attribList);
}
