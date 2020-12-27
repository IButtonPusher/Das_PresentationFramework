using System;
using Das.Views;
using Das.Views.Styles;
using WinForms.Shared;

namespace Das.OpenGL.Windows
{
    public class GLHostedElement : HostedViewControl
    {
        public GLHostedElement(IVisualElement view, 
                               IStyleContext styleContext) 
            : base(view, styleContext)
        {
            IsLoaded = true;
        }

        public override Boolean IsLoaded { get; }

        public override Boolean IsChanged => true;
    }
}
