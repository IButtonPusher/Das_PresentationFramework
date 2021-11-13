using System;
using Das.Views;
using Das.Views.Core.Geometry;
using Das.Views.Panels;
using Das.Views.Windows;
using Das.Views.Styles;

namespace Das.OpenGL.Windows
{
    public class GLWindowBuilder : GLNativeWindowBuilder,
                                   IWindowProvider<GLForm>
    {
       private readonly IVisualBootstrapper _visualBootstrapper;

        private const SetWindowPosFlags ResizeFlags = SetWindowPosFlags.SWP_NOACTIVATE |
                                                      SetWindowPosFlags.SWP_NOCOPYBITS |
                                                      SetWindowPosFlags.SWP_NOMOVE |
                                                      SetWindowPosFlags.SWP_NOOWNERZORDER;

        public GLWindowBuilder(String nativeWindowClassName,
                               IVisualBootstrapper visualBootstrapper)
        : base(nativeWindowClassName)
        {
           _visualBootstrapper = visualBootstrapper;
        }

        //public GLForm Show<TViewModel>(TViewModel viewModel, 
        //                               IBindableElement view)
        //    //where TViewModel : IViewModel
        //{
        //    //var styleContext = view.StyleContext;
        //    //var styleContext = DefaultStyleContext.Instance;

        //    var control = new GLHostedElement(view);
        //    var form = new GLForm(control);

        //    view.DataContext = viewModel;
        //    //view.SetDataContext(viewModel);

        //    WindowShown?.Invoke(form);

        //    return form;
        //}

        public GLForm Show<TRectangle>(IVisualElement view,
                                       TRectangle rect) 
            where TRectangle : IRectangle
        {
           // var styleContext = view.StyleContext;

            var control = new GLHostedElement(view,
                BaselineThemeProvider.Instance, _visualBootstrapper);
            var form = new GLForm(control);

            form.Bounds = new System.Drawing.Rectangle(
                Convert.ToInt32(rect.Left),
                Convert.ToInt32(rect.Top),
                Convert.ToInt32(rect.Width),
                Convert.ToInt32(rect.Height));
            

            WindowShown?.Invoke(form);

            return form;
        }
        
        //public GLForm Show<TViewModel>(TViewModel viewModel, 
        //                               IVisualElement view)
        //{
        //   var control = new GLHostedElement(view, BaselineThemeProvider.Instance,
        //       _visualBootstrapper);
        //    var form = new GLForm(control);

        //    view.DataContext = viewModel;

        //    WindowShown?.Invoke(form);

        //    return form;
        //}

        public GLForm Show(IVisualElement view)
        {
           var control = new GLHostedElement(view,
              BaselineThemeProvider.Instance, _visualBootstrapper);
           var form = new GLForm(control);

            //return Show(view.DataContext, view);

            WindowShown?.Invoke(form);

            return form;
        }

        public event Action<GLForm>? WindowShown;


        public static void ResizeNativeWindow(IntPtr hWnd, Int32 width, Int32 height)
        {
            Native.SetWindowPos(hWnd, IntPtr.Zero, 0, 0, width, height, ResizeFlags);
        }

    }
}
