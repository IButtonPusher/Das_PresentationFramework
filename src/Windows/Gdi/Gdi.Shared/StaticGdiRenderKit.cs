using System;
using System.Collections.Generic;
using Das.Serializer;
using Das.Views;
using Das.Views.Core.Geometry;
using Das.Views.Images;
using Das.Views.Layout;
using Das.Views.Rendering;
using Das.Views.Styles;
using Gdi.Shared;

#pragma warning disable CS0067

namespace Das.Gdi.Kits
{
    // ReSharper disable once UnusedType.Global
    // ReSharper disable once ClassNeverInstantiated.Global
    public class StaticGdiRenderKit : BaseRenderKit, 
                                      IRenderKit 
    { 
        //public StaticGdiRenderKit(IViewPerspective viewPerspective,
        //                          IMultiSerializer xmlSerializer)
        //: this(viewPerspective, xmlSerializer)
        //{}

        public StaticGdiRenderKit(IViewPerspective viewPerspective,
                                   IMultiSerializer xmlSerializer)
            : base(new GdiImageProvider(), xmlSerializer, 
                NullSvgBuilder.Instance, null)
        {
            var defaultSurrogates = new BaseSurrogateProvider();

            var lastMeasure = new Dictionary<IVisualElement, ValueSize>();
            var visualLineage = new VisualLineage();
            var layoutQueue = new LayoutQueue();

            MeasureContext = new GdiMeasureContext(defaultSurrogates, lastMeasure,
                BaselineThemeProvider.Instance, visualLineage, layoutQueue);

            RenderContext = new GdiRenderContext(viewPerspective,
                MeasureContext.Graphics, defaultSurrogates, lastMeasure,
                new Dictionary<IVisualElement, ValueCube>(), 
                BaselineThemeProvider.Instance, visualLineage, layoutQueue);
        }

        IMeasureContext IRenderKit.MeasureContext => MeasureContext;

        IRenderContext IRenderKit.RenderContext => RenderContext;

        public GdiMeasureContext MeasureContext { get; }

        public GdiRenderContext RenderContext { get; }
    }
}