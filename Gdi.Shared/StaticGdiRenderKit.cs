using System;
using System.Collections.Generic;
using Das.Views;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Styles;
using Gdi.Shared;

#pragma warning disable CS0067

namespace Das.Gdi.Kits
{
    // ReSharper disable once UnusedType.Global
    public class StaticGdiRenderKit : BaseRenderKit, 
                                      IRenderKit 
    { 
        public StaticGdiRenderKit(IViewPerspective viewPerspective)
        : base(new BaseStyleContext(new DefaultStyle(),
            new DefaultColorPalette()))
        {
            var defaultSurrogates = new BaseSurrogateProvider();
            var imageProvider = new GdiImageProvider();

            var lastMeasure = new Dictionary<IVisualElement, ValueSize>();

            MeasureContext = new GdiMeasureContext(defaultSurrogates,lastMeasure);
            RenderContext = new GdiRenderContext(viewPerspective, 
                MeasureContext.Graphics, defaultSurrogates, imageProvider);
        }

        IMeasureContext IRenderKit.MeasureContext => MeasureContext;

        IRenderContext IRenderKit.RenderContext => RenderContext;

        public GdiMeasureContext MeasureContext { get; }

        public GdiRenderContext RenderContext { get; }
    }
}