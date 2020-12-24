using System;
using Das.Views.Winforms;

namespace Das.OpenGL.Windows
{
    public class GLFormRenderer
    {
        public GLFormRenderer(HostedControl control, 
                              GLWindowsContext context)
        {
            _host = control;
            _context = context;
        }

        private readonly HostedControl _host;
        private readonly IGLContext _context;

        public void Initialize()
        {
            _context.Initialize();
        }

        protected Boolean DoPreRender()
        {
            if (!_host.IsLoaded)
                return false;

            _context.EnsureSurfaceSize();
            
            GLWindows.wglMakeCurrent(_context.DeviceContextHandle,
                _context.RenderContextHandle);
            GL.glClear(GL.COLOR_BUFFER_BIT | GL.DEPTH_BUFFER_BIT);
            GL.glLoadIdentity();

            GL.glTranslatef(0, 0.0f, -3.0f);
            return true;
        }

        protected void DoPostRender()
        {
            GL.glFlush();

            GL.glReadBuffer(GL.COLOR_ATTACHMENT0_EXT);

            if (_context.DeviceContextHandle == IntPtr.Zero)
                return;

            GL.glReadBuffer(GL.COLOR_ATTACHMENT0_EXT);

            _context.Flush();
        }
    }
}
