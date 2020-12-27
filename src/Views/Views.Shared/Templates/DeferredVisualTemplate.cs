using System;
using Das.Views.Construction;
using Das.Views.Controls;

namespace Das.Views.Templates
{
    public class DeferredVisualTemplate : IVisualTemplate
    {
        public IMarkupNode MarkupNode { get; }

        public DeferredVisualTemplate(IMarkupNode markup)
        {
            MarkupNode = markup;
        }

        public IVisualElement? Content { get; } = null;
    }
}
