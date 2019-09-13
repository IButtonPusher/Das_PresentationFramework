using Das.Views;
using Das.Views.Winforms;
using System;
using System.Windows.Forms;

namespace Das.OpenGL.Windows
{
    public class GLForm : ViewForm, IViewHost
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
    }
}
