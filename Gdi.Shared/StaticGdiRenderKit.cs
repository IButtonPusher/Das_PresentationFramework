﻿using System;
using Das.Views;
using Das.Views.Rendering;

namespace Das.Gdi.Kits
{
    // ReSharper disable once UnusedType.Global
    public class StaticGdiRenderKit : BaseRenderKit, IRenderKit
    { 
        public StaticGdiRenderKit(IViewPerspective viewPerspective)
        {
            var defaultSurrogates = new BaseSurrogateProvider();
            MeasureContext = new GdiMeasureContext(defaultSurrogates);
            RenderContext = new GdiRenderContext(MeasureContext, 
                viewPerspective, MeasureContext.Graphics, defaultSurrogates);
        }

        IMeasureContext IRenderKit.MeasureContext => MeasureContext;

        IRenderContext IRenderKit.RenderContext => RenderContext;

        public GdiMeasureContext MeasureContext { get; }

        public GdiRenderContext RenderContext { get; }
    }
}