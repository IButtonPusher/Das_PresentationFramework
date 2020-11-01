using Das.Views;
using Das.Views.Controls;
using Das.Views.Core.Writing;
using Das.Views.Rendering;

namespace Das.OpenGL
{
    public class OpenGLRenderKit : BaseRenderKit, 
                                   IRenderKit,
                                   IVisualSurrogateProvider
    {
        public OpenGLRenderKit(IFontProvider fontProvider, IGLContext glContext)
        {
            MeasureContext = new GLMeasureContext(fontProvider, this);
            RenderContext = new GLRenderContext(MeasureContext, new BasePerspective(),
                 glContext, fontProvider, this);
        }

        IMeasureContext IRenderKit.MeasureContext => MeasureContext;
        IRenderContext IRenderKit.RenderContext => RenderContext;

        public GLMeasureContext MeasureContext { get; }

        public GLRenderContext RenderContext { get; }

        public void EnsureSurrogate(ref IVisualElement element)
        {
            
        }
    }
}
