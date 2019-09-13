using System;
using System.Runtime.InteropServices;
using Das.Views;
using Das.Views.Panels;
using Das.Views.Winforms;

namespace Das.OpenGL.Windows
{
    public class GLWindowBuilder : IWindowProvider<GLForm>
    {
        private readonly string _nativeWindowClassName;
        private const SetWindowPosFlags ResizeFlags = SetWindowPosFlags.SWP_NOACTIVATE |
                                                      SetWindowPosFlags.SWP_NOCOPYBITS |
                                                      SetWindowPosFlags.SWP_NOMOVE |
                                                      SetWindowPosFlags.SWP_NOOWNERZORDER;

        public GLWindowBuilder(String nativeWindowClassName)
        {
            _nativeWindowClassName = nativeWindowClassName;
        }

        public GLForm Show<TViewModel>(TViewModel viewModel, IView view)
            where TViewModel : IViewModel
        {
            var styleContext = view.StyleContext;

            var control = new GLHostedElement(view, styleContext);
            var form = new GLForm(control);

            view.SetDataContext(viewModel);

            return form;
        }

        static GLWindowBuilder()
        {
            _wndProcDelegate = WndProc;
        }
        private static IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam) 
            => DefWindowProc(hWnd, msg, wParam, lParam);

        private static readonly WndProc _wndProcDelegate;

        public IntPtr BuildNativeWindow(Int32 width, Int32 height)
        {
            var _ = GetGLWindowClass();
            var hwnd = CreateWindowEx(0,
                _nativeWindowClassName,
                "",
                WindowStyles.WS_CLIPCHILDREN | WindowStyles.WS_CLIPSIBLINGS | WindowStyles.WS_POPUP,
                0, 0, width, height,
                IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

            return hwnd;
        }

        public void ResizeNativeWindow(IntPtr hWnd, Int32 width, Int32 height)
        {
            Native.SetWindowPos(hWnd, IntPtr.Zero, 0, 0, width, height, ResizeFlags);
        }

        public WNDCLASSEX GetGLWindowClass()
        {
            var _wndClass = new WNDCLASSEX();
            _wndClass.Init();
            _wndClass.style = ClassStyles.HorizontalRedraw | ClassStyles.VerticalRedraw 
                | ClassStyles.OwnDC;
            _wndClass.lpfnWndProc = _wndProcDelegate;
            _wndClass.cbClsExtra = 0;
            _wndClass.cbWndExtra = 0;
            _wndClass.hInstance = IntPtr.Zero;
            _wndClass.hIcon = IntPtr.Zero;
            _wndClass.hCursor = IntPtr.Zero;
            _wndClass.hbrBackground = IntPtr.Zero;
            _wndClass.lpszMenuName = null;
            _wndClass.lpszClassName = _nativeWindowClassName;
            _wndClass.hIconSm = IntPtr.Zero;
            Native.RegisterClassEx(ref _wndClass);

            return _wndClass;
        }

        [DllImport(Native.User32, SetLastError = true)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport(Native.User32, SetLastError = true)]
        public static extern IntPtr CreateWindowEx(
            WindowStylesEx dwExStyle,
            string lpClassName,
            string lpWindowName,
            WindowStyles dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam);

    }
}
