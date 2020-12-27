using System;

namespace Das.Views.Primitives
{
    public class SpanSlim : VisualElement,
                            ISpan
    {
        public SpanSlim(IVisualBootstrapper visualBootstrapper) 
            : base(visualBootstrapper)
        {
        }
    }
}
