using System;
using System.Runtime.InteropServices;

namespace Das.OpenGL.Windows
{
    public static class GLWindows
    {
        public const String OpenGL32 = "opengl32.dll";

        [DllImport(OpenGL32, SetLastError = true)]
        public static extern Int32 wglDeleteContext(IntPtr hrc);

        [DllImport(OpenGL32, SetLastError = true)]
        public static extern IntPtr wglCreateContext(IntPtr hdc);

        [DllImport(OpenGL32, SetLastError = true)]
        public static extern Int32 wglMakeCurrent(IntPtr hdc, IntPtr hrc);

        [DllImport(OpenGL32, SetLastError = true)]
        public static extern Boolean wglUseFontBitmaps(IntPtr hDC, UInt32 first, 
            UInt32 count, UInt32 listBase);
    }

    public delegate void glRenderbufferStorageEXT(UInt32 target, UInt32 internalformat,
        Int32 width, Int32 height);

    public delegate void glBindRenderbufferEXT(UInt32 target, UInt32 renderbuffer);


    public delegate void glGenRenderbuffersEXT(UInt32 n, UInt32[] renderbuffers);

    public delegate void glGenFramebuffersEXT(UInt32 n, UInt32[] framebuffers);

    public delegate void glBindFramebufferEXT(UInt32 target, UInt32 framebuffer);

    public delegate void glFramebufferRenderbufferEXT(UInt32 target, UInt32 attachment,
        UInt32 renderbuffertarget, UInt32 renderbuffer);

    public delegate void glDeleteRenderbuffersEXT(UInt32 n, UInt32[] renderbuffers);
    public delegate void glDeleteFramebuffersEXT(UInt32 n, UInt32[] framebuffers);

    public delegate IntPtr wglCreateContextAttribsARB(IntPtr hDC, IntPtr hShareContext,
        Int32[] attribList);
}
