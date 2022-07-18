using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.OpenGL;
using Das.OpenGL.Windows;
using Das.Views.Windows;

namespace OpenGLTests.Samples
{
   public class HelloBase
   {
      public HelloBase(Form form,
                       IntPtr handle)
      {
         _form = form;
         _requestedOpenGLVersion = OpenGLVersion.OpenGL4_4;
         _delegateCache = new DelegateCache();

         _windowBuilder = new GLNativeWindowBuilder("OpenGLSurface");
         _dibSection = new DIBSection();

         var _hostGraphics = Graphics.FromHwnd(handle);
         _hostDc = _hostGraphics.GetHdc();

         _windowHandle = _windowBuilder.BuildNativeWindow(_roundedWidth, _roundedHeight);

         _deviceContextHandle = Native.GetDC(_windowHandle);

         var pfd = new Pixelformatdescriptor();
         pfd.Init();
         pfd.nVersion = 1;
         pfd.dwFlags = Pixelformatdescriptor.PFD_DRAW_TO_WINDOW |
                       Pixelformatdescriptor.PFD_SUPPORT_OPENGL |
                       Pixelformatdescriptor.PFD_DOUBLEBUFFER;
         pfd.iPixelType = Pixelformatdescriptor.PFD_TYPE_RGBA;
         pfd.cColorBits = _bitDepth;
         pfd.cDepthBits = 16;
         pfd.cStencilBits = 8;
         pfd.iLayerType = Pixelformatdescriptor.PFD_MAIN_PLANE;

         Int32 iPixelformat;
         if ((iPixelformat = Native.ChoosePixelFormat(_deviceContextHandle, pfd)) == 0)
            throw new InvalidOperationException();

         if (Native.SetPixelFormat(_deviceContextHandle, iPixelformat, pfd) == 0)
            throw new InvalidOperationException();

         _renderContextHandle = GLWindows.wglCreateContext(_deviceContextHandle);

         GLWindows.wglMakeCurrent(_deviceContextHandle, _renderContextHandle);
         UpdateContextVersion();
         CreateDBOs();


         OnSizeChanged();

         form.SizeChanged += OnFormSizeChanged;
      }

      private void OnFormSizeChanged(Object sender,
                                     EventArgs e)
      {
         OnSizeChanged();
      }

      public IntPtr CreateContextAttribsARB(IntPtr hShareContext,
                                            Int32[] attribList)
      {
         return _delegateCache.Get<wglCreateContextAttribsARB>()(_deviceContextHandle,
            hShareContext, attribList);
      }

      protected void UpdateContextVersion()
      {
         //  If the request version number is anything up to and including 2.1, standard render contexts
         //  will provide what we need (as long as the graphics card drivers are up to date).
         var requestedVersionNumber = VersionAttribute.GetVersionAttribute(_requestedOpenGLVersion);
         if (requestedVersionNumber.IsAtLeastVersion(3, 0) == false) return;

         //  Now the none-trivial case. We must use the WGL_ARB_create_context extension to 
         //  attempt to create a 3.0+ context.
         try
         {
            Int32[] attributes =
            {
               GLWindowsContext.WGL_CONTEXT_MAJOR_VERSION_ARB, requestedVersionNumber.Major,
               GLWindowsContext.WGL_CONTEXT_MINOR_VERSION_ARB, requestedVersionNumber.Minor,
               GLWindowsContext.WGL_CONTEXT_FLAGS_ARB,
               GLWindowsContext.WGL_CONTEXT_FORWARD_COMPATIBLE_BIT_ARB,
               0
            };
            var hrc = CreateContextAttribsARB(IntPtr.Zero, attributes);
            GLWindows.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
            GLWindows.wglDeleteContext(_renderContextHandle);
            GLWindows.wglMakeCurrent(_deviceContextHandle, hrc);
            _renderContextHandle = hrc;
         }
         catch (Exception)
         {
            //  TODO: can we actually get the real version?
         }
      }

      private void OnSizeChanged()
      {
         SetSizeFromHost();
         var w = _roundedWidth;
         var h = _roundedHeight;

         GLWindowBuilder.ResizeNativeWindow(_windowHandle, w, h);

         _dibSection.Resize(w, h, _bitDepth);

         GL.glViewport(0, 0, w, h);
      }

      private void SetSizeFromHost()
      {
         var w = _form.Width;
         var h = _form.Height;

         _roundedWidth = Convert.ToInt32(w);
         _roundedHeight = Convert.ToInt32(h);

         _currentSize.Width = w;
         _currentSize.Height = h;
      }

      private void CreateDBOs()
      {
         _dibSectionDeviceContext = Native.CreateCompatibleDC(_deviceContextHandle);
         _dibSection.Create(_dibSectionDeviceContext, _roundedWidth,
            _roundedHeight, _bitDepth);
      }

      protected UInt32 EBO;
      protected UInt32 VAO;

      protected UInt32 VBO;

      protected readonly Byte _bitDepth = 32;

      protected readonly DelegateCache _delegateCache;
      protected readonly DIBSection _dibSection;

      protected readonly Form _form;

      protected readonly IntPtr _hostDc;
      protected readonly OpenGLVersion _requestedOpenGLVersion;
      protected readonly GLNativeWindowBuilder _windowBuilder;
      protected Size _currentSize;
      protected IntPtr _deviceContextHandle;
      protected IntPtr _dibSectionDeviceContext = IntPtr.Zero;
      protected IntPtr _renderContextHandle;
      protected Int32 _roundedHeight;
      protected Int32 _roundedWidth;
      protected IntPtr _windowHandle;
   }
}
