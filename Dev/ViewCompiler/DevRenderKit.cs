using System;
using System.Drawing;
using System.Linq;
using System.Text;
using Das.Gdi;
using Das.Gdi.Kits;
using Das.Views;
using Das.Views.Rendering;
using Brush = Das.Views.Brush;
using FontStyle = Das.Views.FontStyle;

namespace ViewCompiler
{
    public class DevRenderKit : GdiRenderKit //ControlRenderKit
    {
        private readonly ViewWindow _window;
        private IRectangle _rightRectangle;
        private readonly StringBuilder _sbSelected;
        private IRenderedVisual _renderedVisual;
        private readonly IFont _font;
        private Boolean _isChanged;

        public DevRenderKit(ViewWindow window, IVisualElement visualElement, 
            IStyleContext styleContext)
          //  , IInputHandler inputHandler,IInputContext inputContext)
            : base(//window, 
                  visualElement, styleContext, 
                  new BasePerspective() )
                  //,inputHandler, inputContext)
        {
            _window = window;
            _sbSelected = new StringBuilder();
            _font = new Das.Views.Font(10, "Segoe UI", FontStyle.Regular);
        }

        public override bool IsChanged => _isChanged || base.IsChanged;

        public override void AcceptChanges()
        {
            base.AcceptChanges();
            _isChanged = false;
        }

        public void InterrogateElement(IRenderedVisual visual)
        {
            if (visual == null)
            {
                return;
            }

            _renderedVisual = visual;

            _isChanged = true;
        }

        protected /*override*/ void Render(Graphics g)
        {
            
            //base.Render(g);
           
            if (_isChanged)
            {
                _sbSelected.Clear();

                var element = _renderedVisual.Element;

                var measured = MeasureContext.MeasureElement(element, _window.RenderMargin);

                _sbSelected.AppendLine(element.ToString());
                _sbSelected.AppendLine("Measured: " + measured);
                _sbSelected.AppendLine("Arranged: " + _renderedVisual.Position);

                var nonDefaults = GetNonDefaultSetters(element).ToArray();

                foreach (var kvp in nonDefaults)
                {
                    _sbSelected.AppendLine(kvp.Key + ": " + kvp.Value);
                }
            }

            _rightRectangle = new Das.Views.Rectangle(_window.Width - _window.RenderMargin.Right,
                0, _window.RenderMargin.Width, _window.Height);
            RenderContext.FillRect(_rightRectangle, Brush.DarkGray);

            if (_renderedVisual?.Element == null)
                return;

            
            RenderContext.DrawString(_sbSelected.ToString(), _font, Brush.White, _rightRectangle);
        }
    }
}
