using Das.Views;
using Das.Views.Core.Writing;
using Das.Views.Input;
using Das.Views.Rendering;

namespace Das.OpenGL
{
    public class OpenGLRenderKit : IRenderKit
    {
        public OpenGLRenderKit(IFontProvider fontProvider, IGLContext glContext)
        {
            MeasureContext = new GLMeasureContext(fontProvider);
            RenderContext = new GLRenderContext(MeasureContext, new BasePerspective(),
                 glContext, fontProvider);
        }

        IMeasureContext IRenderKit.MeasureContext => MeasureContext;
        IRenderContext IRenderKit.RenderContext => RenderContext;

        public IInputContext InputContext { get; } = null;

        public GLMeasureContext MeasureContext { get; }

        public GLRenderContext RenderContext { get; }


    }
}
