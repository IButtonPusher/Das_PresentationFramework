using System.Collections.Generic;
using Das.Serializer;
using Das.Views;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
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
            MeasureContext = new GLMeasureContext(fontProvider, this, 
                lastMeasurements, styleContext);
            RenderContext = new GLRenderContext(new BasePerspective(),
                 glContext, fontProvider, this, styleContext);
        }

        IMeasureContext IRenderKit.MeasureContext => MeasureContext;
        IRenderContext IRenderKit.RenderContext => RenderContext;

        public GLMeasureContext MeasureContext { get; }

        public GLRenderContext RenderContext { get; }

        protected static readonly DasSerializer Serializer = new DasSerializer();
    }
}
