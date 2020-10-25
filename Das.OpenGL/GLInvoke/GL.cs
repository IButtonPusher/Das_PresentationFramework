using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Das.OpenGL
{
    [SuppressUnmanagedCodeSecurity]
    public static partial class GL
    {
        
        /////////////////////////////////////////////////////////////////


        public static uint CreateVertexShader() => CreateShader(GL_VERTEX_SHADER);
        

        public const uint GL_COMPILE_STATUS                      = 0x8B81;
        public const uint GL_TRUE                           = 1;
        public const uint GL_FALSE                          = 0;
        public const uint GL_ARRAY_BUFFER                            = 0x8892;
        public const uint GL_FLOAT                          = 0x1406;
        public const uint GL_LINK_STATUS                         = 0x8B82;
        public const uint GL_TRIANGLES                      = 0x0004;
        public const uint GL_UNSIGNED_INT                   = 0x1405;
        public const uint GL_UNSIGNED_SHORT                 = 0x1403;
        public const uint GL_INFO_LOG_LENGTH                     = 0x8B84;

        public static void GetShaderInfoLog (uint shader, int bufSize, IntPtr length, 
                                             StringBuilder infoLog)
        {
            GetDelegateFor<glGetShaderInfoLog>()(shader, bufSize, length, infoLog);

        }


        public static void ShaderSource (uint shader, string source)
        {
            //  Remember, the function takes an array of strings but concatenates them, so we should NOT split into lines!
            GetDelegateFor<glShaderSource>()(shader, 1, new[] { source }, new[] { source.Length });
        }

        public static uint CreateShader (uint type)
        {
            return (uint)GetDelegateFor<glCreateShader>()(type);
        }

        public static void CompileShader (uint shader)
        {
            GetDelegateFor<glCompileShader>()(shader);
        }

        public static void GetShaderiv (uint shader, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetShaderiv>()(shader, pname, parameters);
        }

        public static bool GetCompileStatus(uint shader)
        {
            int[] parameters = new int[] { 0 };
            GetShaderiv(shader, GL_COMPILE_STATUS, parameters);
            return parameters[0] == GL_TRUE;
        }

        public static void BindAttribLocation (uint program, uint index, string name)
        {
            GetDelegateFor<glBindAttribLocation>()(program, index, name);
        }

        public static void DeleteShader (uint shader)
        {
            GetDelegateFor<glDeleteShader>()(shader);
        }

        public static void GenVertexArrays(int n, uint[] arrays)
        {
            GetDelegateFor<glGenVertexArrays>()(n, arrays);
        }

        public static void GenBuffers(int n, uint[] buffers)
        {
            GetDelegateFor<glGenBuffers>()(n, buffers);
        }
        
        public static void BindVertexArray(uint array)
        {
            GetDelegateFor<glBindVertexArray>()(array);
        }

        public static void BindBuffer(uint target, uint buffer)
        {
            GetDelegateFor<glBindBuffer>()(target, buffer);
        }

        public static void VertexAttribPointer (uint index, int size, uint type, bool normalized, int stride, IntPtr pointer)
        {
            GetDelegateFor<glVertexAttribPointer>()(index, size, type, normalized, stride, pointer);
        }

        public static void EnableVertexAttribArray (uint index)
        {
            GetDelegateFor<glEnableVertexAttribArray>()(index);
        }

        public static int GetAttribLocation (uint program, string name)
        {
            return (int)GetDelegateFor<glGetAttribLocation>()(program, name);
        }

        public static uint CreateProgram ()
        {
            return (uint)GetDelegateFor<glCreateProgram>()();
        }

        public static void GetProgram (uint program, uint pname, int[] parameters)
        {
            GetDelegateFor<glGetProgramiv>()(program, pname, parameters);
        }

        public static void UseProgram (uint program)
        {
            GetDelegateFor<glUseProgram>()(program);
        }

        public static void AttachShader (uint program, uint shader)
        {
            GetDelegateFor<glAttachShader>()(program, shader);
        }

        public static void LinkProgram (uint program)
        {
            GetDelegateFor<glLinkProgram>()(program);
        }

        public static void DeleteVertexArrays(int n, uint[] arrays)
        {
            GetDelegateFor<glDeleteVertexArrays>()(n, arrays);
        }

        public static void GetProgramInfoLog(uint program, int bufSize, IntPtr length, StringBuilder infoLog)
        {
            GetDelegateFor<glGetProgramInfoLog>()(program, bufSize, length, infoLog);
        }


        public static void UniformMatrix4 (int location, int count, bool transpose, float[] value)
        {
            GetDelegateFor<glUniformMatrix4fv>()(location, count, transpose, value);
        }

        public static void BufferData(uint target, float[] data, uint usage)
        {
            IntPtr p = Marshal.AllocHGlobal(data.Length * sizeof(float));
            Marshal.Copy(data, 0, p, data.Length);
            GetDelegateFor<glBufferData>()(target, data.Length * sizeof(float), p, usage);
            Marshal.FreeHGlobal(p);
        }

        public static int GetUniformLocation (uint program, string name)
        {
            return (int)GetDelegateFor<glGetUniformLocation>()(program, name);
        }

        public static void BufferData(uint target, ushort[] data, uint usage)
        {
            var dataSize = data.Length * sizeof(ushort);
            IntPtr p = Marshal.AllocHGlobal(dataSize);
            var shortData = new short[data.Length];
            Buffer.BlockCopy(data, 0, shortData, 0, dataSize);
            Marshal.Copy(shortData, 0, p, data.Length);
            GetDelegateFor<glBufferData>()(target, dataSize, p, usage);
            Marshal.FreeHGlobal(p);
        }

        public const uint GL_VERTEX_SHADER                       = 0x8B31;
        public const uint GL_FRAGMENT_SHADER                     = 0x8B30;
        public const uint GL_COLOR_BUFFER_BIT               = 0x00004000;
        public const uint GL_DEPTH_BUFFER_BIT               = 0x00000100;
        public const uint GL_STATIC_DRAW                             = 0x88E4;
        public const uint GL_ELEMENT_ARRAY_BUFFER                    = 0x8893;
        public const uint GL_STENCIL_BUFFER_BIT             = 0x00000400;

        private delegate void glBindAttribLocation (uint program, uint index, string name);
        
        private delegate uint glCreateShader (uint type);
        private delegate void glDeleteShader (uint shader);
        private delegate void glShaderSource (uint shader, int count, string[] source, int[] length);
        private delegate void glCompileShader (uint shader);
        private delegate void glGetShaderiv (uint shader, uint pname, int[] parameters);
        private delegate void glGenVertexArrays(int n, uint[] arrays);
        private delegate uint glCreateProgram ();
        private delegate void glAttachShader (uint program, uint shader);
        private delegate void glLinkProgram (uint program);
        private delegate int glGetUniformLocation (uint program, string name);
        private delegate void glDeleteVertexArrays(int n, uint[] arrays);
        private delegate void glGenBuffers (int n, uint[] buffers);
        private delegate void glBindVertexArray(uint array);
        private delegate void glBindBuffer (uint target, uint buffer);
        private delegate void glBufferData(uint target, int size, IntPtr data, uint usage);
        private delegate void glVertexAttribPointer (uint index, int size, uint type, bool normalized, int stride, IntPtr pointer);
        private delegate void glEnableVertexAttribArray (uint index);
        private delegate void glUseProgram (uint program);
        private delegate void glGetProgramiv (uint program, uint pname, int[] parameters);
        private delegate void glGetShaderInfoLog (uint shader, int bufSize, IntPtr length, StringBuilder infoLog);
        private delegate int glGetAttribLocation (uint program, string name);
        private delegate void glGetProgramInfoLog(uint program, int bufSize, IntPtr length, StringBuilder infoLog);

        private delegate void glUniformMatrix4fv (int location, int count, bool transpose, float[] value);
        
        private static Dictionary<string, Delegate> extensionFunctions = new Dictionary<string, Delegate>();
        [DllImport(LIBRARY_OPENGL, SetLastError = true)]
        public static extern IntPtr wglGetProcAddress(string name);
        
        
        /// <summary>
        /// Returns a delegate for an extension function. This delegate  can be called to execute the extension function.
        /// </summary>
        /// <typeparam name="T">The extension delegate type.</typeparam>
        /// <returns>The delegate that points to the extension function.</returns>
        private static T GetDelegateFor<T>() where T : Delegate
        {
            //  Get the type of the extension function.
            Type delegateType = typeof(T);

            //  Get the name of the extension function.
            string name = delegateType.Name;

            // ftlPhysicsGuy - Better way
            if (extensionFunctions.TryGetValue(name, out Delegate del) == false)
            {
                IntPtr proc = wglGetProcAddress(name);
                if (proc == IntPtr.Zero)
                    throw new Exception("Extension function " + name + " not supported");

                //  Get the delegate for the function pointer.
                del = Marshal.GetDelegateForFunctionPointer(proc, delegateType);

                //  Add to the dictionary.
                extensionFunctions.Add(name, del);
            }

            return (T)del;
        }


        [DllImport(LIBRARY_OPENGL, SetLastError = true)] public static extern void glDrawElements(uint mode, int count, uint type, IntPtr indices);
        [DllImport(LIBRARY_OPENGL, SetLastError = true)] public static extern void glDrawArrays (uint mode, int first, int count);

        /////////////////////////////////////////////////////////////////


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
