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
        : base(
            //new BaseStyleContext(DefaultStyle.Instance, new DefaultColorPalette()), 
            DefaultStyleContext.Instance,
            attributeScanner, typeInferrer, propertyProvider)
        {
            var defaultSurrogates = new BaseSurrogateProvider();
            //var imageProvider = new GdiImageProvider();

            var lastMeasure = new Dictionary<IVisualElement, ValueSize>();
            var visualLineage = new VisualLineage();

            MeasureContext = new GdiMeasureContext(defaultSurrogates,lastMeasure, 
                _styleContext, visualLineage);
            
            RenderContext = new GdiRenderContext(viewPerspective, 
                MeasureContext.Graphics, defaultSurrogates,lastMeasure,
                new Dictionary<IVisualElement, ValueCube>(), _styleContext, new VisualLineage());
        }

        IMeasureContext IRenderKit.MeasureContext => MeasureContext;

        IRenderContext IRenderKit.RenderContext => RenderContext;

        public GdiMeasureContext MeasureContext { get; }

        public GdiRenderContext RenderContext { get; }
    }
}