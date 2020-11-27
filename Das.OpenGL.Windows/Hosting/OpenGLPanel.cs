using System;
using System.Threading;
using System.Threading.Tasks;
using Das.Views;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.OpenGL.Windows
{
    public abstract class OpenGLPanel : GLHostedElement
    {
        public OpenGLPanel(IView view, 
                           IStyleContext styleContext)
            : base(view, styleContext)
        {
            var bldr = new GLWindowBuilder("OpenGLSurface");
            var boot = new GLBootStrapper(bldr);
            var context = boot.GetContext(this);

            var fontProvider = GLBootStrapper.GetFontProvider(context);
            var kit = new OpenGLRenderKit(fontProvider, context, styleContext);

            var renderer = new PanelRenderer(this, context);
            var _ = new PanelUpdater(Render, kit, renderer, boot.Scheduler);
        }

        protected abstract void Render(IRenderContext context);

        private class PanelUpdater : BaseLoopUpdater
        {
            private readonly Action<IRenderContext> _renderAction;
            private readonly OpenGLRenderKit _kit;
            private readonly PanelRenderer _renderer;

            public PanelUpdater(Action<IRenderContext> renderAction,
                OpenGLRenderKit kit, PanelRenderer renderer,
                TaskScheduler taskScheduler)
            {
                _renderAction = renderAction;
                _kit = kit;
                _renderer = renderer;

                Task.Factory.StartNew(GameOverload, CancellationToken.None,
                    TaskCreationOptions.LongRunning, taskScheduler);
            }

            private async Task GameOverload()
            {
                _renderer.Initialize();
                await GameLoop();
            }

            protected override Boolean IsChanged => true;
            protected override Boolean Update()
            {
                if (!_renderer.PreRender())
                    return false;
                _renderAction(_kit.RenderContext);
                _renderer.PostRender();

                return true;
            }
        }

        private class PanelRenderer : GLFormRenderer
        {
            public PanelRenderer(GLHostedElement control, GLWindowsContext context)
                : base(control, context)
            {
            }

            public Boolean PreRender() => DoPreRender();

            public void PostRender() => DoPostRender();
        }
    }
}
