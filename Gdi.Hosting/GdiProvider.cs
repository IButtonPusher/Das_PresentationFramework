using System;
using System.Drawing;
using System.Windows.Forms;
using Das.Container;
using Das.Gdi.Controls;
using Das.Gdi.Kits;
using Das.Views;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Mvvm;
using Das.Views.Styles;

namespace Das.Gdi
{
    public class GdiProvider : IWindowProvider<ViewWindow>, 
                               IBootStrapper
    {
        public GdiRenderKit RenderKit { get; }

        // ReSharper disable once UnusedMember.Global
        public GdiProvider(GdiRenderKit renderKit)
        {
            RenderKit = renderKit;
            VisualBootstrapper = renderKit.VisualBootstrapper;
        }

        public GdiProvider(IResolver container)
        {
            RenderKit = GetKit(this, container);
            VisualBootstrapper = RenderKit.VisualBootstrapper;
        }

        public GdiProvider() : this(new BaseResolver())
        {
            
        }

        private static GdiRenderKit GetKit(IWindowProvider<IVisualHost> windowProvider,
                                           IResolver container)
        {
            var perspective = new BasePerspective();
            var kit = new GdiRenderKit(perspective, windowProvider,
                new BaseStyleContext(new DefaultStyle(), new DefaultColorPalette()),
                container);
            return kit;
        }

        public ViewWindow Show<TViewModel>(TViewModel viewModel, 
                                           IView view)
            where TViewModel : IViewModel
        {
            var styleContext = view.StyleContext;

            var control = new GdiHostedElement(view, styleContext);
            var form = new ViewWindow(control);
            Cook(form);

            //var myScreen = Screen.FromControl(form);
            //var area = myScreen.WorkingArea;
            //var size = new ValueSize(area.Width, area.Height);

            //var iWant = renderer.GetContentSize(size);

            //var hwnd = new WindowInteropHelper( this ).EnsureHandle();
            //var monitor = NativeMethods.MonitorFromWindow( hwnd, NativeMethods.MONITOR_DEFAULTTONEAREST );


            view.SetDataContext(viewModel);

            WindowShown?.Invoke(form);

            return form;
        }

        public ViewWindow Show<TViewModel>(TViewModel viewModel, 
                                           IView<TViewModel> view) 
            where TViewModel : IViewModel
        {
            var styleContext = view.StyleContext;

            var control = new GdiHostedElement(view, styleContext);
            var form = new ViewWindow(control);
            Cook(form);

            //System.Windows.SystemParameters.PrimaryScreenWidth System.Windows.SystemParameters.PrimaryScreenHeight

            //renderer.GetContentSize()

            view.SetDataContext(viewModel);

            WindowShown?.Invoke(form);

            return form;
        }

        public ViewWindow Show<TViewModel>(IView<TViewModel> view) 
            where TViewModel : IViewModel
        {
            return Show(view.DataContext, view);
        }

        //public VisualForm Show(IVisualRenderer visual)
        //{
        //    var control = new HostedVisualControl(visual);
        //    var form = new VisualForm(visual, control);
        //    Cook(form);

        //    return form;
        //}

        public event Action<ViewWindow>? WindowShown;

        public void Run<TViewModel>(TViewModel viewModel, 
                                    IView view) 
            where TViewModel : IViewModel
        {
            var window = Show(viewModel, view);
            
            Application.Run(window);
        }

        public IVisualBootstrapper VisualBootstrapper { get; }

        // ReSharper disable once UnusedMember.Global
        public GdiHostedElement Host<TViewModel>(TViewModel viewModel, 
                                                 IView<TViewModel> view)
            where TViewModel : IViewModel
        {
            var styleContext = view.StyleContext;
            var control = new GdiHostedElement(view, styleContext);
            control.DataContext = viewModel;
            Cook(control);

            view.SetDataContext(viewModel);

            return control;
        }

        // ReSharper disable once UnusedMember.Global
        public GdiHostedElement HostStatic<TViewModel>(TViewModel viewModel, IView<TViewModel> view)
        {
            var styleContext = view.StyleContext;
            var control = new GdiHostedElement(view, styleContext);
            var renderer = new BitmapRenderer(control,
                RenderKit.MeasureContext, RenderKit.RenderContext);

            view.SetDataContext(viewModel);

            control.BackingBitmap = renderer.DoRender();
            control.DataContextChanged += (o, e) => { control.BackingBitmap = renderer.DoRender(); };

            return control;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private IRenderer<Bitmap> Cook(IViewHost<Bitmap> form)
        {
            var renderer = new BitmapRenderer(form,
                RenderKit.MeasureContext, RenderKit.RenderContext);
            var _ = new LoopViewUpdater<Bitmap>(form, renderer);
            return renderer;

        }
    }
}