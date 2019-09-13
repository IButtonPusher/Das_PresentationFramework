using System;
using System.Runtime.InteropServices;

namespace Das.OpenGL
{
    public static partial class GL
    {
        public const string LIBRARY_OPENGL = "opengl32.dll";
        internal const string LIBRARY_GLU = "Glu32.dll";

        public const uint SMOOTH = 0x1D01;

        public const uint DEPTHTEST = 0x0B71;

        public const uint LEQUAL = 0x0203;

        public const uint PERSPECTIVE_CORRECTION_HINT = 0x0C50;

        public const uint NICEST = 0x1102;

        public const uint POINTS = 0x0000;
        public const uint LINES = 0x0001;
        public const uint LINE_LOOP = 0x0002;
        public const uint TRIANGLES = 0x0004;
        public const uint TRIANGLE_FAN = 0x0006;
        public const uint QUADS = 0x0007;
        public const uint POLYGON = 0x0009;

        public const uint COLOR_BUFFER_BIT = 0x00004000;
        public const uint DEPTH_BUFFER_BIT = 0x00000100;
        public const uint FRAMEBUFFER_EXT = 0x8D40;
        public const uint RENDERBUFFER_EXT = 0x8D41;
        public const uint RGBA = 0x1908;
        public const uint DEPTH_COMPONENT24 = 0x81A6;
        public const uint COLOR_ATTACHMENT0_EXT = 0x8CE0;
        public const uint BGRA = 0x80E1;
        public const uint UNSIGNED_BYTE = 0x1401;
        public const uint DEPTH_ATTACHMENT_EXT = 0x8D00;
        public const uint MODELVIEW = 0x1700;
        public const uint PROJECTION = 0x1701;
        public const uint VIEWPORT = 0x0BA2;
        public const uint BLEND = 0x0BE2;
        public const uint MODELVIEW_MATRIX = 0x0BA6;

        public const uint COMPILE = 0x1300;

        public const uint CURRENT_BIT = 0x00000001;
        public const uint LIST_BIT = 0x00020000;
        public const uint FILL = 0x1B02;
        public const uint ENABLE_BIT = 0x00002000;
        public const uint TRANSFORM_BIT = 0x00001000;

        public const uint LIGHTING = 0x0B50;
        
        public const uint LINEAR = 0x2601;
        public const uint DEPTH_TEST = 0x0B71;
        public const uint LUMINANCE_ALPHA = 0x190A;

        public const uint SRC_ALPHA = 0x0302;
        public const uint ONE_MINUS_SRC_ALPHA = 0x0303;


        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glShadeModel(uint mode);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glClearColor(float red, float green, float blue,
            float alpha);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glClear(uint mask);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glLoadIdentity();

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glClearDepth(double depth);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glEnable(uint cap);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glDepthFunc(uint func);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glHint(uint target, uint mode);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glVertex3f(float x, float y, float z);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glVertex2d(double x, double y);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glVertex2f(float x, float y);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glRectd(double x1, double y1, double x2, double y2);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glRectf(float x1, float y1, float x2, float y2);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glColor3f(float red, float green, float blue);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glColor3b(byte red, byte green, byte blue);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glEnd();

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glFlush();

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glBegin(uint mode);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glReadBuffer(uint mode);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glReadPixels(int x, int y, int width, int height,
            uint format, uint type, IntPtr pixels);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glViewport(int x, int y, int width, int height);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glMatrixMode(uint mode);

        [DllImport(LIBRARY_GLU, SetLastError = true)]
        public static extern void gluPerspective(double fovy, double aspect, double zNear, double zFar);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glTranslatef(float x, float y, float z);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glTranslated(double x, double y, double z);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glRotatef(float angle, float x, float y, float z);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glPushMatrix();

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glGetIntegerv(uint pname, int[] params_notkeyword);
        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glOrtho(double left, double right, double bottom,
            double top, double zNear, double zFar);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glRasterPos2i(int x, int y);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glPushAttrib(uint mask);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glDisable(uint cap);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern uint glGenLists(int range);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glListBase(uint base_notkeyword);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glCallLists(int n, uint type, byte[] lists);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glDeleteLists(uint list, int range);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glPopAttrib();

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glPopMatrix();

        

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glNewList(uint list, uint mode);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glEndList();


        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glBlendFunc(uint sfactor, uint dfactor);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glGetFloatv(uint pname, float[] params_notkeyword);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glMultMatrixf(float[] m);

        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern void glLineWidth(float width);

    }
}
