using System;
using Das.Views;
using Das.Views.Rendering;

namespace Das.Gdi.Kits
{
    public class GdiRenderKit : IRenderKit
    {
        IMeasureContext IRenderKit.MeasureContext => MeasureContext;
        IRenderContext IRenderKit.RenderContext => RenderContext;

        public GdiRenderContext RenderContext { get; }

        public GdiMeasureContext MeasureContext { get; }

        public GdiRenderKit(IViewPerspective viewPerspective)
        {
            MeasureContext = new GdiMeasureContext();
            RenderContext = new GdiRenderContext(MeasureContext, viewPerspective);
        }
    }
}