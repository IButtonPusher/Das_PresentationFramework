using System;
using System.Collections.Generic;
using Das.Serializer;
using Das.Views;

using Das.Views.Core.Geometry;
using Das.Views.Layout;
using Das.Views.Rendering;
using Das.Views.Styles;

#pragma warning disable CS0067

namespace Das.Gdi.Kits
{
    // ReSharper disable once UnusedType.Global
    // ReSharper disable once ClassNeverInstantiated.Global
    public class StaticGdiRenderKit : BaseRenderKit, 
                                      IRenderKit 
    { 
        public StaticGdiRenderKit(IViewPerspective viewPerspective,
                                  IStringPrimitiveScanner attributeScanner,
                                  ITypeInferrer typeInferrer,
                                  IPropertyProvider propertyProvider)
        : this(viewPerspective, attributeScanner, typeInferrer, propertyProvider, 
            new Dictionary<IVisualElement, ValueCube>())
        {}

        private StaticGdiRenderKit(IViewPerspective viewPerspective,
                                   IStringPrimitiveScanner attributeScanner,
                                   ITypeInferrer typeInferrer,
                                   IPropertyProvider propertyProvider,
                                   Dictionary<IVisualElement, ValueCube> renderPositions)
            : base(DefaultStyleContext.Instance,
                attributeScanner, typeInferrer, propertyProvider, renderPositions)
        {
            var defaultSurrogates = new BaseSurrogateProvider();

            var lastMeasure = new Dictionary<IVisualElement, ValueSize>();
            var visualLineage = new VisualLineage();
            var layoutQueue = new LayoutQueue();

            MeasureContext = new GdiMeasureContext(defaultSurrogates, lastMeasure,
                _styleContext, visualLineage);

            RenderContext = new GdiRenderContext(viewPerspective,
                MeasureContext.Graphics, defaultSurrogates, lastMeasure,
                new Dictionary<IVisualElement, ValueCube>(), 
                _styleContext, visualLineage, layoutQueue);
        }

        IMeasureContext IRenderKit.MeasureContext => MeasureContext;

        IRenderContext IRenderKit.RenderContext => RenderContext;

        public GdiMeasureContext MeasureContext { get; }

        public GdiRenderContext RenderContext { get; }
    }
}