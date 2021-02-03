using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.Container;
using Das.Gdi.Controls;
using Das.Gdi.Core;
using Das.Gdi.Kits;
using Das.Views;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Mvvm;
using Das.Views.Panels;
using Das.Views.Rendering;

namespace Das.Gdi
{
    public class GdiProvider : IWindowProvider<ViewWindow>,
                               IBootStrapper
    {
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

        // ReSharper disable once UnusedMember.Global
        public GdiProvider() : this(new BaseResolver())
        {
        }

        public void Run(IView view)
        {
            var window = Show(view);

            Application.Run(window);
        }

        public IVisualBootstrapper VisualBootstrapper { get; }


        public ViewWindow Show<TRectangle>(IView view,
                                           TRectangle rect)
            where TRectangle : IRectangle
        {
            //var styleContext = view.StyleContext;

            var control = new GdiHostedElement(view);
            var form = new ViewWindow(control);
            Cook(form);

            form.Bounds = GdiTypeConverter.GetRect(rect);

            WindowShown?.Invoke(form);

            return form;
        }

        public ViewWindow Show(IView view)
        {
            //var styleContext = view.StyleContext;

            var control = new GdiHostedElement(view);
            var form = new ViewWindow(control);
            Cook(form);

            var viewWidth = view.Width ?? 0;
            var viewHeight = view.Height ?? 0;

            if (viewWidth.IsNotZero() && viewHeight.IsNotZero())
                form.Size = GdiTypeConverter.GetSize(viewWidth, viewHeight);

            WindowShown?.Invoke(form);

            return form;
        }

        public event Action<ViewWindow>? WindowShown;

        public GdiRenderKit RenderKit { get; }

        // ReSharper disable once UnusedMember.Global
        public GdiHostedElement Host<TViewModel>(TViewModel viewModel,
                                                 IView view)
            where TViewModel : IViewModel
        {
            var control = new GdiHostedElement(view);
            
            Cook(control);

            view.DataContext = viewModel;
            

            return control;
        }

        // ReSharper disable once UnusedMember.Global
        public GdiHostedElement HostStatic<TViewModel>(TViewModel viewModel
                                                       , IView view)
        {
            var control = new GdiHostedElement(view);
            var renderer = new BitmapRenderer(control,
                RenderKit.MeasureContext, RenderKit.RenderContext);

            view.DataContext = viewModel;
            

            control.BackingBitmap = renderer.DoRender();

            return control;
        }

        public ViewWindow Show<TViewModel>(TViewModel viewModel,
                                           IBindableElement view)
            //where TViewModel : IViewModel
        {

            var control = new GdiHostedElement(view);
            var form = new ViewWindow(control);
            Cook(form);

            view.DataContext = viewModel;

            WindowShown?.Invoke(form);

            return form;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private IRenderer<Bitmap> Cook(IViewHost<Bitmap> form)
        {
            var renderer = new BitmapRenderer(form,
                RenderKit.MeasureContext, RenderKit.RenderContext);
            var _ = new LoopViewUpdater<Bitmap>(form, renderer, RenderKit.RenderContext.LayoutQueue);
            return renderer;
        }

        private static GdiRenderKit GetKit(IWindowProvider<IVisualHost> windowProvider,
                                           IResolver container)
        {
            var perspective = new BasePerspective();
            var kit = new GdiRenderKit(perspective, windowProvider);
                //BaselineThemeProvider.Instance,
                //container);
            return kit;
        }
    }
}