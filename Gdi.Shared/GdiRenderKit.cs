using System;
using System.Threading.Tasks;
using Windows.Shared.Input;
using Das.Views;
using Das.Views.Core.Geometry;
using Das.Views.Input;
using Das.Views.Rendering;

namespace Das.Gdi.Kits
{
    public class GdiRenderKit : IRenderKit
    {
        private readonly IWindowProvider<IViewHost> _windowProvider;

        public GdiRenderKit(IViewPerspective viewPerspective,
                            IWindowProvider<IViewHost> windowProvider)
        {
            _windowProvider = windowProvider;
            MeasureContext = new GdiMeasureContext();
            RenderContext = new GdiRenderContext(MeasureContext, viewPerspective);

             _windowProvider.WindowShown += OnWindowShown;

            //InputContext = new Win32InputContext();
        }

        private void OnWindowShown(IViewHost window)
        {
            
            var i = new InputContext(window, new BaseInputHandler(RenderContext));
            //var test = new Win32InputContext(window.Handle);
        }

        public GdiMeasureContext MeasureContext { get; }

        public GdiRenderContext RenderContext { get; }

        public IInputContext InputContext { get; } = null;

        IMeasureContext IRenderKit.MeasureContext => MeasureContext;

        IRenderContext IRenderKit.RenderContext => RenderContext;
    }
}