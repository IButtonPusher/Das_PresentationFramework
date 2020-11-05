using Das.Views;
using Das.Views.Core.Writing;
using Das.Views.Rendering;

namespace Das.OpenGL
{
    public class OpenGLRenderKit : BaseRenderKit, 
                                   IRenderKit
                                   
    {
        public OpenGLRenderKit(IFontProvider fontProvider, IGLContext glContext)
        {
            MeasureContext = new GLMeasureContext(fontProvider, this);
            RenderContext = new GLRenderContext(new BasePerspective(),
                 glContext, fontProvider, this);
        }

        IMeasureContext IRenderKit.MeasureContext => MeasureContext;
        IRenderContext IRenderKit.RenderContext => RenderContext;

        public GLMeasureContext MeasureContext { get; }

        public GLRenderContext RenderContext { get; }
    }
}
