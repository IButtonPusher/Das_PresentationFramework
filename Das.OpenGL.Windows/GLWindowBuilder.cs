using System;
using System.Runtime.InteropServices;
using Das.Views;
using Das.Views.Panels;
using Das.Views.Windows;
using Das.Views.Winforms;
using Das.ViewsModels;

namespace Das.OpenGL.Windows
{
    public class GLWindowBuilder : IWindowProvider<GLForm>
    {
        private readonly String _nativeWindowClassName;
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
        private static IntPtr WndProc(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam) 
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
            var wndClass = new WNDCLASSEX();
            wndClass.Init();
            wndClass.style = ClassStyles.HorizontalRedraw | ClassStyles.VerticalRedraw 
                | ClassStyles.OwnDC;
            wndClass.lpfnWndProc = _wndProcDelegate;
            wndClass.cbClsExtra = 0;
            wndClass.cbWndExtra = 0;
            wndClass.hInstance = IntPtr.Zero;
            wndClass.hIcon = IntPtr.Zero;
            wndClass.hCursor = IntPtr.Zero;
            wndClass.hbrBackground = IntPtr.Zero;
            wndClass.lpszMenuName = null;
            wndClass.lpszClassName = _nativeWindowClassName;
            wndClass.hIconSm = IntPtr.Zero;
            Native.RegisterClassEx(ref wndClass);

            return wndClass;
        }

        [DllImport(Native.User32, SetLastError = true)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, UInt32 uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport(Native.User32, SetLastError = true)]
        public static extern IntPtr CreateWindowEx(
            WindowStylesEx dwExStyle,
            String lpClassName,
            String lpWindowName,
            WindowStyles dwStyle,
            Int32 x,
            Int32 y,
            Int32 nWidth,
            Int32 nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam);

    }
}
