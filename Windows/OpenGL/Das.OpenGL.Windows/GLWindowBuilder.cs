using System;
using System.Drawing;
using Das.Views;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Panels;
using Das.Views.Windows;
using Das.Views.Mvvm;
using Das.Views.Styles;

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

        public GLForm Show<TViewModel>(TViewModel viewModel, 
                                       IBindableElement view)
            //where TViewModel : IViewModel
        {
            //var styleContext = view.StyleContext;
            var styleContext = new DefaultStyleContext();

            var control = new GLHostedElement(view, styleContext);
            var form = new GLForm(control);

            view.DataContext = viewModel;
            //view.SetDataContext(viewModel);

            WindowShown?.Invoke(form);

            return form;
        }

        public GLForm Show<TRectangle>(IView view,
                                       TRectangle rect) 
            where TRectangle : IRectangle
        {
            var styleContext = view.StyleContext;

            var control = new GLHostedElement(view, styleContext);
            var form = new GLForm(control);

            form.Bounds = new System.Drawing.Rectangle(
                Convert.ToInt32(rect.Left),
                Convert.ToInt32(rect.Top),
                Convert.ToInt32(rect.Width),
                Convert.ToInt32(rect.Height));
            

            WindowShown?.Invoke(form);

            return form;
        }
        
        public GLForm Show<TViewModel>(TViewModel viewModel, 
                                       IView view)
        {
            var styleContext = view.StyleContext;

            var control = new GLHostedElement(view, styleContext);
            var form = new GLForm(control);

            view.DataContext = viewModel;
            //view.SetDataContext(viewModel);

            WindowShown?.Invoke(form);

            return form;
        }

        public GLForm Show(IView view) 
            //where TViewModel : IViewModel
        {
            return Show(view.DataContext, view);
        }

        public event Action<GLForm>? WindowShown;

        static GLWindowBuilder()
        {
            _wndProcDelegate = WndProc;
        }
        private static IntPtr WndProc(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam) 
            => Native.DefWindowProc(hWnd, msg, wParam, lParam);

        private static readonly UWndProc _wndProcDelegate;

        public IntPtr BuildNativeWindow(Int32 width, Int32 height)
        {
            var _ = GetGLWindowClass();
            var hwnd = Native.CreateWindowEx(0,
                _nativeWindowClassName,
                "",
                WindowStyles.WS_CLIPCHILDREN | WindowStyles.WS_CLIPSIBLINGS | WindowStyles.WS_POPUP,
                0, 0, width, height,
                IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

            return hwnd;
        }

        public static void ResizeNativeWindow(IntPtr hWnd, Int32 width, Int32 height)
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
            wndClass.lpszMenuName = null!;
            wndClass.lpszClassName = _nativeWindowClassName;
            wndClass.hIconSm = IntPtr.Zero;
            Native.RegisterClassEx(ref wndClass);

            return wndClass;
        }

        
    }
}
