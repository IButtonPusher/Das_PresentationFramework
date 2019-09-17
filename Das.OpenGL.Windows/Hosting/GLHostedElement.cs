using Das.Views.Panels;
using Das.Views.Styles;
using Das.Views.Winforms;
using WinForms.Shared;

namespace Das.OpenGL.Windows
{
    public class GLHostedElement : HostedViewControl
    {
        public GLHostedElement(IView view, IStyleContext styleContext) 
            : base(view, styleContext)
        {
            IsLoaded = true;
        }

        public override bool IsLoaded { get; }

        public override bool IsChanged => true;
    }
}
