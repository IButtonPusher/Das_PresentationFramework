using System;
using System.Threading.Tasks;
using Das.Views;
using Das.Views.Core.Geometry;
using Das.Views.Styles;
using Das.Views.Windows;
using Rectangle = System.Drawing.Rectangle;

namespace Das.OpenGL.Windows
{
    public class GLWindowBuilder : GLNativeWindowBuilder,
                                   IWindowProvider<GLForm>
    {
        public GLWindowBuilder(String nativeWindowClassName,
                               IVisualBootstrapper visualBootstrapper)
            : base(nativeWindowClassName)
        {
            _visualBootstrapper = visualBootstrapper;
        }

        public GLForm Show<TRectangle>(IVisualElement view,
                                       TRectangle rect)
            where TRectangle : IRectangle
        {
            var control = new GLHostedElement(view,
                BaselineThemeProvider.Instance, _visualBootstrapper);
            var form = new GLForm(control);

            form.Bounds = new Rectangle(
                Convert.ToInt32(rect.Left),
                Convert.ToInt32(rect.Top),
                Convert.ToInt32(rect.Width),
                Convert.ToInt32(rect.Height));


            WindowShown?.Invoke(form);

            return form;
        }

        public GLForm Show(IVisualElement view)
        {
            var control = new GLHostedElement(view,
                BaselineThemeProvider.Instance, _visualBootstrapper);
            var form = new GLForm(control);

            WindowShown?.Invoke(form);

            return form;
        }

        public event Action<GLForm>? WindowShown;


        public static void ResizeNativeWindow(IntPtr hWnd,
                                              Int32 width,
                                              Int32 height)
        {
            Native.SetWindowPos(hWnd, IntPtr.Zero, 0, 0, width, height, ResizeFlags);
        }

        private const SetWindowPosFlags ResizeFlags = SetWindowPosFlags.SWP_NOACTIVATE |
                                                      SetWindowPosFlags.SWP_NOCOPYBITS |
                                                      SetWindowPosFlags.SWP_NOMOVE |
                                                      SetWindowPosFlags.SWP_NOOWNERZORDER;

        private readonly IVisualBootstrapper _visualBootstrapper;
    }
}
