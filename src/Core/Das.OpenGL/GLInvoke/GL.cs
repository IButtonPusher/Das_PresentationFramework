using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Das.OpenGL;

[SuppressUnmanagedCodeSecurity]
public static partial class GL
{
   /////////////////////////////////////////////////////////////////


   public static UInt32 CreateVertexShader()
   {
      return CreateShader(GL_VERTEX_SHADER);
   }

   public static void GetShaderInfoLog(UInt32 shader,
                                       Int32 bufSize,
                                       IntPtr length,
                                       StringBuilder infoLog)
   {
      GetDelegateFor<glGetShaderInfoLog>()(shader, bufSize, length, infoLog);
   }


   public static void ShaderSource(UInt32 shader,
                                   String source)
   {
      //  Remember, the function takes an array of strings but concatenates them, so we should NOT split into lines!
      GetDelegateFor<glShaderSource>()(shader, 1, new[] {source}, new[] {source.Length});
   }

   public static UInt32 CreateShader(UInt32 type)
   {
      return GetDelegateFor<glCreateShader>()(type);
   }

   public static void CompileShader(UInt32 shader)
   {
      GetDelegateFor<glCompileShader>()(shader);
   }

   public static void GetShaderiv(UInt32 shader,
                                  UInt32 pname,
                                  Int32[] parameters)
   {
      GetDelegateFor<glGetShaderiv>()(shader, pname, parameters);
   }

   public static Boolean GetCompileStatus(UInt32 shader)
   {
      Int32[] parameters = {0};
      GetShaderiv(shader, GL_COMPILE_STATUS, parameters);
      return parameters[0] == GL_TRUE;
   }

   public static void BindAttribLocation(UInt32 program,
                                         UInt32 index,
                                         String name)
   {
      GetDelegateFor<glBindAttribLocation>()(program, index, name);
   }

   public static void DeleteShader(UInt32 shader)
   {
      GetDelegateFor<glDeleteShader>()(shader);
   }

   public static void GenVertexArrays(Int32 n,
                                      UInt32[] arrays)
   {
      GetDelegateFor<glGenVertexArrays>()(n, arrays);
   }

   public static void GenBuffers(Int32 n,
                                 UInt32[] buffers)
   {
      GetDelegateFor<glGenBuffers>()(n, buffers);
   }

   public static void BindVertexArray(UInt32 array)
   {
      GetDelegateFor<glBindVertexArray>()(array);
   }

   public static void BindBuffer(UInt32 target,
                                 UInt32 buffer)
   {
      GetDelegateFor<glBindBuffer>()(target, buffer);
   }

   public static void VertexAttribPointer(UInt32 index,
                                          Int32 size,
                                          UInt32 type,
                                          Boolean normalized,
                                          Int32 stride,
                                          IntPtr pointer)
   {
      GetDelegateFor<glVertexAttribPointer>()(index, size, type, normalized, stride, pointer);
   }

   public static void EnableVertexAttribArray(UInt32 index)
   {
      GetDelegateFor<glEnableVertexAttribArray>()(index);
   }

   public static Int32 GetAttribLocation(UInt32 program,
                                         String name)
   {
      return GetDelegateFor<glGetAttribLocation>()(program, name);
   }

   public static UInt32 CreateProgram()
   {
      return GetDelegateFor<glCreateProgram>()();
   }

   public static void GetProgram(UInt32 program,
                                 UInt32 pname,
                                 Int32[] parameters)
   {
      GetDelegateFor<glGetProgramiv>()(program, pname, parameters);
   }

   public static void UseProgram(UInt32 program)
   {
      GetDelegateFor<glUseProgram>()(program);
   }

   public static void AttachShader(UInt32 program,
                                   UInt32 shader)
   {
      GetDelegateFor<glAttachShader>()(program, shader);
   }

   public static void LinkProgram(UInt32 program)
   {
      GetDelegateFor<glLinkProgram>()(program);
   }

   public static void DeleteVertexArrays(Int32 n,
                                         UInt32[] arrays)
   {
      GetDelegateFor<glDeleteVertexArrays>()(n, arrays);
   }

   public static void GetProgramInfoLog(UInt32 program,
                                        Int32 bufSize,
                                        IntPtr length,
                                        StringBuilder infoLog)
   {
      GetDelegateFor<glGetProgramInfoLog>()(program, bufSize, length, infoLog);
   }


   public static void UniformMatrix4(Int32 location,
                                     Int32 count,
                                     Boolean transpose,
                                     Single[] value)
   {
      GetDelegateFor<glUniformMatrix4fv>()(location, count, transpose, value);
   }

   public static void BufferData(UInt32 target,
                                 Single[] data,
                                 UInt32 usage)
   {
      var p = Marshal.AllocHGlobal(data.Length * sizeof(Single));
      Marshal.Copy(data, 0, p, data.Length);
      GetDelegateFor<glBufferData>()(target, data.Length * sizeof(Single), p, usage);
      Marshal.FreeHGlobal(p);
   }

   public static Int32 GetUniformLocation(UInt32 program,
                                          String name)
   {
      return GetDelegateFor<glGetUniformLocation>()(program, name);
   }

   public static void BufferData(UInt32 target,
                                 UInt16[] data,
                                 UInt32 usage)
   {
      var dataSize = data.Length * sizeof(UInt16);
      var p = Marshal.AllocHGlobal(dataSize);
      var shortData = new Int16[data.Length];
      Buffer.BlockCopy(data, 0, shortData, 0, dataSize);
      Marshal.Copy(shortData, 0, p, data.Length);
      GetDelegateFor<glBufferData>()(target, dataSize, p, usage);
      Marshal.FreeHGlobal(p);
   }

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern IntPtr wglGetProcAddress(String name);


   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glDrawElements(UInt32 mode,
                                            Int32 count,
                                            UInt32 type,
                                            IntPtr indices);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glDrawArrays(UInt32 mode,
                                          Int32 first,
                                          Int32 count);


   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glShadeModel(UInt32 mode);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glClearColor(Single red,
                                          Single green,
                                          Single blue,
                                          Single alpha);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glClear(UInt32 mask);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glLoadIdentity();

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glClearDepth(Double depth);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glEnable(UInt32 cap);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glDepthFunc(UInt32 func);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glHint(UInt32 target,
                                    UInt32 mode);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glVertex3f(Single x,
                                        Single y,
                                        Single z);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glVertex2d(Double x,
                                        Double y);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glVertex2f(Single x,
                                        Single y);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glRectd(Double x1,
                                     Double y1,
                                     Double x2,
                                     Double y2);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glRectf(Single x1,
                                     Single y1,
                                     Single x2,
                                     Single y2);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glColor3f(Single red,
                                       Single green,
                                       Single blue);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glColor3b(Byte red,
                                       Byte green,
                                       Byte blue);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glEnd();

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glFlush();

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glBegin(UInt32 mode);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glReadBuffer(UInt32 mode);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glReadPixels(Int32 x,
                                          Int32 y,
                                          Int32 width,
                                          Int32 height,
                                          UInt32 format,
                                          UInt32 type,
                                          IntPtr pixels);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glViewport(Int32 x,
                                        Int32 y,
                                        Int32 width,
                                        Int32 height);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glMatrixMode(UInt32 mode);

   [DllImport(LIBRARY_GLU, SetLastError = true)]
   public static extern void gluPerspective(Double fovy,
                                            Double aspect,
                                            Double zNear,
                                            Double zFar);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glTranslatef(Single x,
                                          Single y,
                                          Single z);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glTranslated(Double x,
                                          Double y,
                                          Double z);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glRotatef(Single angle,
                                       Single x,
                                       Single y,
                                       Single z);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glPushMatrix();

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glGetIntegerv(UInt32 pname,
                                           Int32[] params_notkeyword);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glOrtho(Double left,
                                     Double right,
                                     Double bottom,
                                     Double top,
                                     Double zNear,
                                     Double zFar);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glRasterPos2i(Int32 x,
                                           Int32 y);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glPushAttrib(UInt32 mask);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glDisable(UInt32 cap);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern UInt32 glGenLists(Int32 range);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glListBase(UInt32 base_notkeyword);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glCallLists(Int32 n,
                                         UInt32 type,
                                         Byte[] lists);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glDeleteLists(UInt32 list,
                                           Int32 range);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glPopAttrib();

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glPopMatrix();


   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glNewList(UInt32 list,
                                       UInt32 mode);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glEndList();


   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glBlendFunc(UInt32 sfactor,
                                         UInt32 dfactor);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glGetFloatv(UInt32 pname,
                                         Single[] params_notkeyword);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glMultMatrixf(Single[] m);

   [DllImport(LIBRARY_OPENGL, SetLastError = true)]
   public static extern void glLineWidth(Single width);


   /// <summary>
   ///    Returns a delegate for an extension function. This delegate  can be called to execute the extension function.
   /// </summary>
   /// <typeparam name="T">The extension delegate type.</typeparam>
   /// <returns>The delegate that points to the extension function.</returns>
   private static T GetDelegateFor<T>() where T : Delegate
   {
      //  Get the type of the extension function.
      Type delegateType = typeof(T);

      //  Get the name of the extension function.
      String name = delegateType.Name;

      // ftlPhysicsGuy - Better way
      if (_extensionFunctions.TryGetValue(name, out Delegate del) == false)
      {
         var proc = wglGetProcAddress(name);
         if (proc == IntPtr.Zero)
            throw new Exception("Extension function " + name + " not supported");

         //  Get the delegate for the function pointer.
         del = Marshal.GetDelegateForFunctionPointer(proc, delegateType);

         //  Add to the dictionary.
         _extensionFunctions.Add(name, del);
      }

      return (T) del;
   }


   public const UInt32 GL_COMPILE_STATUS = 0x8B81;
   public const UInt32 GL_TRUE = 1;
   public const UInt32 GL_FALSE = 0;
   public const UInt32 GL_ARRAY_BUFFER = 0x8892;
   public const UInt32 GL_FLOAT = 0x1406;
   public const UInt32 GL_LINK_STATUS = 0x8B82;
   public const UInt32 GL_TRIANGLES = 0x0004;
   public const UInt32 GL_UNSIGNED_INT = 0x1405;
   public const UInt32 GL_UNSIGNED_SHORT = 0x1403;
   public const UInt32 GL_INFO_LOG_LENGTH = 0x8B84;

   public const UInt32 GL_VERTEX_SHADER = 0x8B31;
   public const UInt32 GL_FRAGMENT_SHADER = 0x8B30;
   public const UInt32 GL_COLOR_BUFFER_BIT = 0x00004000;
   public const UInt32 GL_DEPTH_BUFFER_BIT = 0x00000100;
   public const UInt32 GL_STATIC_DRAW = 0x88E4;
   public const UInt32 GL_ELEMENT_ARRAY_BUFFER = 0x8893;
   public const UInt32 GL_STENCIL_BUFFER_BIT = 0x00000400;

   /////////////////////////////////////////////////////////////////


   public const String LIBRARY_OPENGL = "opengl32.dll";
   internal const String LIBRARY_GLU = "Glu32.dll";

   public const UInt32 SMOOTH = 0x1D01;

   public const UInt32 DEPTHTEST = 0x0B71;

   public const UInt32 LEQUAL = 0x0203;

   public const UInt32 PERSPECTIVE_CORRECTION_HINT = 0x0C50;

   public const UInt32 NICEST = 0x1102;

   public const UInt32 POINTS = 0x0000;
   public const UInt32 LINES = 0x0001;
   public const UInt32 LINE_LOOP = 0x0002;
   public const UInt32 TRIANGLES = 0x0004;
   public const UInt32 TRIANGLE_FAN = 0x0006;
   public const UInt32 QUADS = 0x0007;
   public const UInt32 POLYGON = 0x0009;

   public const UInt32 COLOR_BUFFER_BIT = 0x00004000;
   public const UInt32 DEPTH_BUFFER_BIT = 0x00000100;
   public const UInt32 FRAMEBUFFER_EXT = 0x8D40;
   public const UInt32 RENDERBUFFER_EXT = 0x8D41;
   public const UInt32 RGBA = 0x1908;
   public const UInt32 DEPTH_COMPONENT24 = 0x81A6;
   public const UInt32 COLOR_ATTACHMENT0_EXT = 0x8CE0;
   public const UInt32 BGRA = 0x80E1;
   public const UInt32 UNSIGNED_BYTE = 0x1401;
   public const UInt32 DEPTH_ATTACHMENT_EXT = 0x8D00;
   public const UInt32 MODELVIEW = 0x1700;
   public const UInt32 PROJECTION = 0x1701;
   public const UInt32 VIEWPORT = 0x0BA2;
   public const UInt32 BLEND = 0x0BE2;
   public const UInt32 MODELVIEW_MATRIX = 0x0BA6;

   public const UInt32 COMPILE = 0x1300;

   public const UInt32 CURRENT_BIT = 0x00000001;
   public const UInt32 LIST_BIT = 0x00020000;
   public const UInt32 FILL = 0x1B02;
   public const UInt32 ENABLE_BIT = 0x00002000;
   public const UInt32 TRANSFORM_BIT = 0x00001000;

   public const UInt32 LIGHTING = 0x0B50;

   public const UInt32 LINEAR = 0x2601;
   public const UInt32 DEPTH_TEST = 0x0B71;
   public const UInt32 LUMINANCE_ALPHA = 0x190A;

   public const UInt32 SRC_ALPHA = 0x0302;
   public const UInt32 ONE_MINUS_SRC_ALPHA = 0x0303;

   private static readonly Dictionary<String, Delegate> _extensionFunctions = new Dictionary<String, Delegate>();

   private delegate void glBindAttribLocation(UInt32 program,
                                              UInt32 index,
                                              String name);

   private delegate UInt32 glCreateShader(UInt32 type);

   private delegate void glDeleteShader(UInt32 shader);

   private delegate void glShaderSource(UInt32 shader,
                                        Int32 count,
                                        String[] source,
                                        Int32[] length);

   private delegate void glCompileShader(UInt32 shader);

   private delegate void glGetShaderiv(UInt32 shader,
                                       UInt32 pname,
                                       Int32[] parameters);

   private delegate void glGenVertexArrays(Int32 n,
                                           UInt32[] arrays);

   private delegate UInt32 glCreateProgram();

   private delegate void glAttachShader(UInt32 program,
                                        UInt32 shader);

   private delegate void glLinkProgram(UInt32 program);

   private delegate Int32 glGetUniformLocation(UInt32 program,
                                               String name);

   private delegate void glDeleteVertexArrays(Int32 n,
                                              UInt32[] arrays);

   private delegate void glGenBuffers(Int32 n,
                                      UInt32[] buffers);

   private delegate void glBindVertexArray(UInt32 array);

   private delegate void glBindBuffer(UInt32 target,
                                      UInt32 buffer);

   private delegate void glBufferData(UInt32 target,
                                      Int32 size,
                                      IntPtr data,
                                      UInt32 usage);

   private delegate void glVertexAttribPointer(UInt32 index,
                                               Int32 size,
                                               UInt32 type,
                                               Boolean normalized,
                                               Int32 stride,
                                               IntPtr pointer);

   private delegate void glEnableVertexAttribArray(UInt32 index);

   private delegate void glUseProgram(UInt32 program);

   private delegate void glGetProgramiv(UInt32 program,
                                        UInt32 pname,
                                        Int32[] parameters);

   private delegate void glGetShaderInfoLog(UInt32 shader,
                                            Int32 bufSize,
                                            IntPtr length,
                                            StringBuilder infoLog);

   private delegate Int32 glGetAttribLocation(UInt32 program,
                                              String name);

   private delegate void glGetProgramInfoLog(UInt32 program,
                                             Int32 bufSize,
                                             IntPtr length,
                                             StringBuilder infoLog);

   private delegate void glUniformMatrix4fv(Int32 location,
                                            Int32 count,
                                            Boolean transpose,
                                            Single[] value);
}