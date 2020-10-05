using System;
using Das.Views;
using Das.Views.Rendering;

namespace Das.Gdi.Kits
{
    // ReSharper disable once UnusedType.Global
    public class StaticGdiRenderKit : IRenderKit
    { 
        public StaticGdiRenderKit(IViewPerspective viewPerspective)
        {
            MeasureContext = new GdiMeasureContext();
            RenderContext = new GdiRenderContext(MeasureContext, 
                viewPerspective, MeasureContext.Graphics);
        }

        IMeasureContext IRenderKit.MeasureContext => MeasureContext;

        IRenderContext IRenderKit.RenderContext => RenderContext;

        public GdiMeasureContext MeasureContext { get; }

        public GdiRenderContext RenderContext { get; }
    }
}