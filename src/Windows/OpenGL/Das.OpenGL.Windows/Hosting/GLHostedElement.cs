using System;
using Das.Views;
using Das.Views.Colors;
using WinForms.Shared;

namespace Das.OpenGL.Windows
{
    public class GLHostedElement : HostedViewControl
    {
        public GLHostedElement(IVisualElement view,
                               IThemeProvider themeProvider,
                               IVisualBootstrapper visualBootstrapper) 
            : base(view, themeProvider, visualBootstrapper)
        {
            IsLoaded = true;
        }

        public override Boolean IsLoaded { get; }

        public override Boolean IsChanged => true;
    }
}
