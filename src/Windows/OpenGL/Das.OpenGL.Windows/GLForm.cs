using System;
using System.Threading.Tasks;
using Das.Views.Winforms;
using System.Windows.Forms;
using Das.Views.Core.Geometry;
using Das.Views.Windows;

namespace Das.OpenGL.Windows
{
    public class GLForm : ViewForm,
                          IWindowsHost
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

        public Task<IntPtr> GraphicsDeviceContextPromise => _element.GraphicsDeviceContextPromise;
    }
}
