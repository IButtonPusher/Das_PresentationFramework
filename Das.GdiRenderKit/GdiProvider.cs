using System;
using System.Drawing;
using System.Windows.Forms;
using Das.Gdi.Controls;
using Das.Gdi.Kits;
using Das.Views;
using Das.Views.Panels;
using Das.Views.Rendering;

namespace Das.Gdi
{
    public class GdiProvider : IWindowProvider<ViewWindow>, IBootStrapper
    {
        protected readonly GdiRenderKit RenderKit;

        public GdiProvider(GdiRenderKit renderKit)
        {
            RenderKit = renderKit;
        }

        public GdiProvider() : this(GetKit())
        {
        }

        private static GdiRenderKit GetKit()
        {
            var perspective = new BasePerspective();
            var kit = new GdiRenderKit(perspective);
            return kit;
        }

        public ViewWindow Show<TViewModel>(TViewModel viewModel, IView view)
            where TViewModel : IViewModel
        {
            var styleContext = view.StyleContext;

            var control = new GdiHostedElement(view, styleContext);
            var form = new ViewWindow(control);
            Cook(form);

            view.SetDataContext(viewModel);

            return form;
        }

        public void Run<TViewModel>(TViewModel viewModel, IView view) 
            where TViewModel : IViewModel
        {
            var window = Show(viewModel, view);
            Application.Run(window);
        }

        // ReSharper disable once UnusedMember.Global
        public GdiHostedElement Host<TViewModel>(TViewModel viewModel, IView<TViewModel> view)
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

        private void Cook(IViewHost<Bitmap> form)
        {
            var renderer = new BitmapRenderer(form,
                RenderKit.MeasureContext, RenderKit.RenderContext);
            var _ = new LoopViewUpdater<Bitmap>(form, renderer);
        }
    }
}