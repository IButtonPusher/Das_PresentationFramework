using System.Collections.Generic;
using Das.Serializer;
using Das.Views;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Layout;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.OpenGL
{
    public class OpenGLRenderKit : BaseRenderKit, 
                                   IRenderKit
                                   
    {
        public OpenGLRenderKit(IFontProvider fontProvider, 
                               IGLContext glContext,
                               IStyleContext styleContext)
        : base(styleContext, Serializer.AttributeParser, 
            Serializer.TypeInferrer, Serializer.TypeManipulator)
        {
            var lastMeasurements = new Dictionary<IVisualElement, ValueSize>();
            var visualLineage = new VisualLineage();
            
            MeasureContext = new GLMeasureContext(fontProvider, this, 
                lastMeasurements, styleContext, visualLineage);
            
            RenderContext = new GLRenderContext(new BasePerspective(),
                 glContext, fontProvider, this, styleContext, visualLineage);
        }

        IMeasureContext IRenderKit.MeasureContext => MeasureContext;
        IRenderContext IRenderKit.RenderContext => RenderContext;

        public GLMeasureContext MeasureContext { get; }

        public GLRenderContext RenderContext { get; }

        protected static readonly DasSerializer Serializer = new DasSerializer();
    }
}
