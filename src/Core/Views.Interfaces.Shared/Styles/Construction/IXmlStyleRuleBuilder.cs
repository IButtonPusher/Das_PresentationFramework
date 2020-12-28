using System;
using Das.Views.Styles;

namespace Das.Views.Construction.Styles
{
    public interface IXmlStyleRuleBuilder
    {
        IStyleRule? GetRule(IMarkupNode markupNode,
                            Type targetType);
    }
}
