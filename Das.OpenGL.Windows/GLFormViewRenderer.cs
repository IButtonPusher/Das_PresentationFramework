using Das.Views.Core.Geometry;


namespace Das.OpenGL.Windows
{
    public class GLFormViewRenderer : GLFormRenderer, IGLRenderer
    {
        public GLFormViewRenderer(GLHostedElement viewHost, OpenGLRenderKit kit,
            GLWindowsContext context)
            : base(viewHost, context)
        {
            _viewHost = viewHost;
            _measureContext = kit.MeasureContext;
            _renderContext = kit.RenderContext;
            _renderRect = new Rectangle(0, 0, 1, 1);
            
            _context = context;
        }

        private readonly GLHostedElement _viewHost;
        private readonly GLWindowsContext _context;
        private readonly GLMeasureContext _measureContext;
        private readonly GLRenderContext _renderContext;
        private readonly Rectangle _renderRect;

        public void DoRender()
        {
            if (_viewHost.View == null || !DoPreRender())
                return;

            var view = _viewHost.View;

            //Measure
            var available = _context.Size;
            if (available.IsEmpty)
                return;
            _measureContext.ViewState = _viewHost;
            var desired = _measureContext.MeasureElement(view, available);
            _renderRect.Size = desired;

            //arrange
            _renderContext.ViewState = _viewHost;
            //----------------------------
            _renderContext.DrawElement(view, _renderRect);
            //----------------------------
//            var _pen = new Pen(Color.White, 2);
//            _renderContext.DrawLine(_pen, new Point(10, 10), new Point(50, 50));
            //----------------------------
            DoPostRender();
        }
    }
}
