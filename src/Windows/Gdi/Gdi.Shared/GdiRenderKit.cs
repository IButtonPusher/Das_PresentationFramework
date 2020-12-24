using System;
using System.Threading.Tasks;
using Das.Views;
using Das.Views.Core;
using Das.Views.Input;
using Das.Views.Rendering;

namespace Das.Gdi.Kits
{
    public class GdiRenderKit : BaseRenderKit, IRenderKit
    {
        public GdiRenderKit(IViewPerspective viewPerspective,
                            IWindowProvider<IVisualHost> windowProvider)
        {
            MeasureContext = new GdiMeasureContext();
            RenderContext = new GdiRenderContext(MeasureContext, 
                viewPerspective, MeasureContext.Graphics);

            _containedObjects[typeof(IImageProvider)] = RenderContext;

            windowProvider.WindowShown += OnWindowShown;
        }

        // ReSharper disable once NotAccessedField.Local
        private IInputContext? _inputContext;

        IMeasureContext IRenderKit.MeasureContext => MeasureContext;

        IRenderContext IRenderKit.RenderContext => RenderContext;

        public GdiMeasureContext MeasureContext { get; }

        public GdiRenderContext RenderContext { get; }

        private void OnWindowShown(IVisualHost window)
        {
            _inputContext = new InputContext(window, new BaseInputHandler(RenderContext));
        }
    }
}