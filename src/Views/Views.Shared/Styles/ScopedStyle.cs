using System;
using System.Threading.Tasks;

namespace Das.Views.Styles
{
    public class ScopedStyle
    {
        public ScopedStyle(IVisualElement? scope,
                           IStyle style)
        {
            Style = style;
            Scope = scope;
        }

        public IVisualElement? Scope { get; }

        public IStyle Style { get; }
    }
}