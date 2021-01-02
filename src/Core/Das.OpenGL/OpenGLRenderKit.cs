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
            Serializer.TypeInferrer, Serializer.TypeManipulator,
            new Dictionary<IVisualElement, ValueCube>())
        {
            var lastMeasurements = new Dictionary<IVisualElement, ValueSize>();
            var lastRender = new Dictionary<IVisualElement, ValueCube>();
            var visualLineage = new VisualLineage();
            var layoutQueue = new LayoutQueue();
            
            MeasureContext = new GLMeasureContext(fontProvider, this, 
                lastMeasurements, styleContext, visualLineage, layoutQueue);
            
            RenderContext = new GLRenderContext(new BasePerspective(),
                 glContext, fontProvider, this, styleContext, 
                 visualLineage, lastRender, layoutQueue);
        }

        IMeasureContext IRenderKit.MeasureContext => MeasureContext;
        IRenderContext IRenderKit.RenderContext => RenderContext;

        public GLMeasureContext MeasureContext { get; }

        public GLRenderContext RenderContext { get; }

        protected static readonly DasSerializer Serializer = new DasSerializer();
    }
}
