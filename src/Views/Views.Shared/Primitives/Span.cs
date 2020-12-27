using System;
using Das.Views.Panels;

namespace Das.Views.Primitives
{
    public class Span : BasePanel,
                        ISpan
    {
        public Span(IVisualBootstrapper visualBootstrapper) 
            : base(visualBootstrapper)
        {
        }
    }
}
