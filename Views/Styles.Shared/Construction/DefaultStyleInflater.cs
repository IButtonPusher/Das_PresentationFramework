using System;
using Das.Views.Construction;
using Das.Views.Construction.Styles;

namespace Das.Views.Styles.Construction
{
    public class DefaultStyleInflater : StyleInflater
    {
        public DefaultStyleInflater() : base(GetRuleBuilder())
        {
        }

        private static IStyleRuleBuilder GetRuleBuilder()
        {
            var visualAliases = new VisualAliasProvider();
            var selectorBuilder = new StyleSelectorBuilder(visualAliases);

            var ruleBuilder = new StyleRuleBuilder(selectorBuilder,
                DefaultStyleContext.Instance.VariableAccessor);

            return ruleBuilder;
        }
    }
}
