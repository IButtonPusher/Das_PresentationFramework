using System;
using System.Threading.Tasks;
using Das.Views;
using Das.Views.Rendering;

namespace Das.Gdi.Kits
{
    public class GdiRenderKit : IRenderKit
    {
        public GdiRenderKit(IViewPerspective viewPerspective)
        {
            MeasureContext = new GdiMeasureContext();
            RenderContext = new GdiRenderContext(MeasureContext, viewPerspective);
        }

        public GdiMeasureContext MeasureContext { get; }

        public GdiRenderContext RenderContext { get; }

        IMeasureContext IRenderKit.MeasureContext => MeasureContext;

        IRenderContext IRenderKit.RenderContext => RenderContext;
    }
}