using Das.Views.Core.Geometry;
using System;
using System.Threading;
using Das.Views.Winforms;
using System.Drawing;
using Size = Das.Views.Core.Geometry.Size;

namespace Das.OpenGL.Windows
{
    public class GLWindowsContext : IGLContext
    {
        public GLWindowsContext(IWindowsHost viewHost,
            OpenGLVersion glVersion, GLWindowBuilder windowBuilder,
            Byte bitDepth = 32)
        {
            _dibSection = new DIBSection();
            _delegateCache = new DelegateCache();
            _currentSize = new Size(0,0);
            _windowBuilder = windowBuilder;

            _viewHost = viewHost;
            _viewHost.AvailableSizeChanged += OnHostSizeChanged;

            _bitDepth = bitDepth;
            _requestedOpenGLVersion = glVersion;
            _viewHost.HostCreated += OnHostHandleCreated;
        }

        public const Byte PFD_TYPE_RGBA = 0;
        public const UInt32 PFD_DOUBLEBUFFER = 1;
        public const UInt32 PFD_DRAW_TO_WINDOW = 4;
        public const UInt32 PFD_SUPPORT_OPENGL = 32;
        public const SByte PFD_MAIN_PLANE = 0;
        public const Int32 WGL_CONTEXT_MAJOR_VERSION_ARB = 0x2091;
        public const Int32 WGL_CONTEXT_MINOR_VERSION_ARB = 0x2092;
        public const Int32 WGL_CONTEXT_FLAGS_ARB = 0x2094;
        public const Int32 WGL_CONTEXT_FORWARD_COMPATIBLE_BIT_ARB = 0x0002;

        public IntPtr DeviceContextHandle => _deviceContextHandle;
        public IntPtr RenderContextHandle => _renderContextHandle;
        public ISize Size => _currentSize;

        protected IntPtr _windowHandle = IntPtr.Zero;
        protected IntPtr _deviceContextHandle = IntPtr.Zero;
        protected IntPtr _renderContextHandle = IntPtr.Zero;
        private readonly DelegateCache _delegateCache;
        private readonly IWindowsHost _viewHost;
        private readonly Size _currentSize;
        private Int32 _roundedWidth;
        private Int32 _roundedHeight;
        private readonly GLWindowBuilder _windowBuilder;
        private readonly Byte _bitDepth;
        private Int32 _resizesPending;
        
        protected IntPtr _dibSectionDeviceContext = IntPtr.Zero;
        protected readonly DIBSection _dibSection;
        protected readonly OpenGLVersion _requestedOpenGLVersion;
        private Graphics _hostGraphics;
        private IntPtr _hostDc;

        protected UInt32 _frameBufferID;
        protected UInt32 _colorRenderBufferID;
        protected UInt32 _depthRenderBufferID;

        public void Initialize()
        {
           SetSizeFromHost();

            _windowHandle = _windowBuilder.BuildNativeWindow(_roundedWidth, _roundedHeight);

            _deviceContextHandle = Native.GetDC(_windowHandle);

            var pfd = new Pixelformatdescriptor();
            pfd.Init();
            pfd.nVersion = 1;
            pfd.dwFlags = PFD_DRAW_TO_WINDOW | PFD_SUPPORT_OPENGL | PFD_DOUBLEBUFFER;
            pfd.iPixelType = PFD_TYPE_RGBA;
            pfd.cColorBits = _bitDepth;
            pfd.cDepthBits = 16;
            pfd.cStencilBits = 8;
            pfd.iLayerType = PFD_MAIN_PLANE;

            Int32 iPixelformat;
            if ((iPixelformat = Native.ChoosePixelFormat(_deviceContextHandle, pfd)) == 0)
                throw new InvalidOperationException();

            if (Native.SetPixelFormat(_deviceContextHandle, iPixelformat, pfd) == 0)
                throw new InvalidOperationException();

            _renderContextHandle = GLWindows.wglCreateContext(_deviceContextHandle);

            GLWindows.wglMakeCurrent(_deviceContextHandle, _renderContextHandle);
            UpdateContextVersion();
            CreateDBOs();


            GL.glShadeModel(GL.SMOOTH);
            GL.glClearColor(1.0f, 1.0f, 1.0f, 0.0f);
            GL.glClearDepth(1.0f);
            GL.glEnable(GL.DEPTHTEST);
            GL.glDepthFunc(GL.LEQUAL);
            GL.glHint(GL.PERSPECTIVE_CORRECTION_HINT, GL.NICEST);

            OnSizeChanged();
        }

        private void OnHostHandleCreated(Object sender, EventArgs e)
        {
            _viewHost.Invoke(() =>
            {
                _hostGraphics = Graphics.FromHwnd(_viewHost.Handle);
                _hostDc = _hostGraphics.GetHdc();
            });

            _viewHost.HostCreated -= OnHostHandleCreated;
        }

        private void OnHostSizeChanged(Object sender, EventArgs e)
        {
            Interlocked.Increment(ref _resizesPending);
        }

        protected void UpdateContextVersion()
        {
            //  If the request version number is anything up to and including 2.1, standard render contexts
            //  will provide what we need (as long as the graphics card drivers are up to date).
            var requestedVersionNumber = VersionAttribute.GetVersionAttribute(_requestedOpenGLVersion);
            if (requestedVersionNumber.IsAtLeastVersion(3, 0) == false)
            {
                return;
            }

            //  Now the none-trivial case. We must use the WGL_ARB_create_context extension to 
            //  attempt to create a 3.0+ context.
            try
            {
                Int32[] attributes =
                {
                    WGL_CONTEXT_MAJOR_VERSION_ARB, requestedVersionNumber.Major,
                    WGL_CONTEXT_MINOR_VERSION_ARB, requestedVersionNumber.Minor,
                    WGL_CONTEXT_FLAGS_ARB, WGL_CONTEXT_FORWARD_COMPATIBLE_BIT_ARB,
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

        public void EnsureSurfaceSize()
        {
            var changesPending = 0;
            changesPending = Interlocked.Exchange(ref _resizesPending, changesPending);
            if (changesPending > 0)
                OnSizeChanged();
        }

        private void SetSizeFromHost()
        {
            var w = _viewHost.AvailableSize.Width;
            var h = _viewHost.AvailableSize.Height;

            _roundedWidth = Convert.ToInt32(w);
            _roundedHeight = Convert.ToInt32(h);

            _currentSize.Width = w;
            _currentSize.Height = h;
        }

        private void OnSizeChanged()
        {
           SetSizeFromHost();
            var w = _roundedWidth;
            var h = _roundedHeight;

            _windowBuilder.ResizeNativeWindow(_windowHandle, w, h);

            _dibSection.Resize(w, h, _bitDepth);

            DestroyFramebuffers();

            BindBuffers();

            GL.glViewport(0, 0, w, h);

            if (w <= 0 || h <= 0)
                return;

            GL.glMatrixMode(GL.PROJECTION);
            GL.glLoadIdentity();

            GL.gluPerspective(45.0f, w / (Single)h, 0.1f, 100.0f);

            GL.glMatrixMode(GL.MODELVIEW);

            GL.glLoadIdentity();
        }

        private void CreateDBOs()
        {
            BindBuffers();

            _dibSectionDeviceContext = Native.CreateCompatibleDC(_deviceContextHandle);
            _dibSection.Create(_dibSectionDeviceContext, _roundedWidth, 
                _roundedHeight, _bitDepth);
        }

        private void DestroyFramebuffers()
        {
            _delegateCache.Get<glDeleteRenderbuffersEXT>()(2,
                new[] { _colorRenderBufferID, _depthRenderBufferID });

            _delegateCache.Get<glDeleteFramebuffersEXT>()(1, new[] { _frameBufferID });

            _colorRenderBufferID = 0;
            _depthRenderBufferID = 0;
            _frameBufferID = 0;
        }

        private void BindBuffers()
        {
            var ids = new UInt32[1];
            _delegateCache.Get<glGenFramebuffersEXT>()(1, ids);
            _frameBufferID = ids[0];
            _delegateCache.Get<glBindFramebufferEXT>()(GL.FRAMEBUFFER_EXT, _frameBufferID);


            _delegateCache.Get<glGenRenderbuffersEXT>()(1, ids);
            _colorRenderBufferID = ids[0];
            _delegateCache.Get<glBindRenderbufferEXT>()(GL.RENDERBUFFER_EXT,
                _colorRenderBufferID);
            _delegateCache.Get<glRenderbufferStorageEXT>()(GL.RENDERBUFFER_EXT,
                GL.RGBA, _roundedWidth, _roundedHeight);

            //	Create the depth render buffer and bind it, then allocate storage for it.
            _delegateCache.Get<glGenRenderbuffersEXT>()(1, ids);
            _depthRenderBufferID = ids[0];
            _delegateCache.Get<glBindRenderbufferEXT>()(GL.RENDERBUFFER_EXT,
                _depthRenderBufferID);
            _delegateCache.Get<glRenderbufferStorageEXT>()(GL.RENDERBUFFER_EXT,
                GL.DEPTH_COMPONENT24, _roundedWidth, _roundedHeight);

            //  Set the render buffer for colour and depth.
            _delegateCache.Get<glFramebufferRenderbufferEXT>()(GL.FRAMEBUFFER_EXT,
                GL.COLOR_ATTACHMENT0_EXT,
                GL.RENDERBUFFER_EXT, _colorRenderBufferID);
            _delegateCache.Get<glFramebufferRenderbufferEXT>()(GL.FRAMEBUFFER_EXT,
                GL.DEPTH_ATTACHMENT_EXT, GL.RENDERBUFFER_EXT, _depthRenderBufferID);
        }

        public void Flush()
        {
            var w = _roundedWidth;
            var h = _roundedHeight;

            GL.glReadPixels(0, 0, w, h, GL.BGRA,
                GL.UNSIGNED_BYTE, _dibSection.Bits);

            Native.BitBlt(_hostDc, 0, 0, w, h,
                _dibSectionDeviceContext, 0, 0, Native.SRCCOPY);
        }


        public IntPtr CreateContextAttribsARB(IntPtr hShareContext, Int32[] attribList) =>
            _delegateCache.Get<wglCreateContextAttribsARB>()(_deviceContextHandle,
                hShareContext, attribList);
    }
}
