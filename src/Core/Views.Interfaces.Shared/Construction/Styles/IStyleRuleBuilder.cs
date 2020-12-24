using System;
using System.Collections.Generic;
using System.Text;
using Das.Views.Styles;

namespace Das.Views.Construction.Styles
{
    public interface IStyleRuleBuilder
    {
        IStyleRule? GetRule(IMarkupNode cssNode);
    }
}
