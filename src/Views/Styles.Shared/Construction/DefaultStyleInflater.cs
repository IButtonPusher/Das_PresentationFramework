using System;
using Das.Serializer;
using Das.Views.Construction;
using Das.Views.Construction.Styles;

namespace Das.Views.Styles.Construction
{
    public class DefaultStyleInflater : StyleInflater
    {
        public DefaultStyleInflater(ITypeInferrer typeInferrer,
                                    IStyleVariableAccessor variableAccessor) 
            : base(GetCssRuleBuilder(variableAccessor), 
            new XmlStyleRuleBuilder(), new VisualTypeResolver(typeInferrer))
        {
        }

        private static ICssRuleBuilder GetCssRuleBuilder(IStyleVariableAccessor variableAccessor)
        {
            var visualAliases = new VisualAliasProvider();
            var selectorBuilder = new CssStyleSelectorBuilder(visualAliases);

            var ruleBuilder = new CssRuleBuilder(selectorBuilder, variableAccessor);

            return ruleBuilder;
        }
    }
}
