using System;
using Das.Views.Winforms;
using System.Windows.Forms;
using Das.Views.Core.Geometry;

namespace Das.OpenGL.Windows
{
    public class GLForm : ViewForm
    {
        private readonly GLHostedElement _element;

        public GLForm(GLHostedElement element) : base(element)
        {
            _element = element;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            
        }

        public static implicit operator GLHostedElement(GLForm form)
            => form._element;

        public override IPoint2D GetOffset(IPoint2D input)
        {
            throw new NotImplementedException();
        }
    }
}
