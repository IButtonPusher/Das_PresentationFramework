using Das.Views.Rendering;

namespace Das.Views.Styles
{
    public class ScopedStyle
    {
        public IStyle Style { get; }

        public ScopedStyle(IVisualElement scope, IStyle style)
        {
            Style = style;
            Scope = scope;
        }

        public IVisualElement Scope { get; }
    }
}
