using System;
using Das.Serializer;
using Das.Views.Construction;
using Das.Views.Construction.Styles;

namespace Das.Views.Styles.Construction
{
    public class DefaultStyleInflater : StyleInflater
    {
        public DefaultStyleInflater(ITypeInferrer typeInferrer) 
            : base(GetCssRuleBuilder(), 
            new XmlStyleRuleBuilder(), new VisualTypeResolver(typeInferrer))
        {
        }

        private static ICssRuleBuilder GetCssRuleBuilder()
        {
            var visualAliases = new VisualAliasProvider();
            var selectorBuilder = new CssStyleSelectorBuilder(visualAliases);

            var ruleBuilder = new CssRuleBuilder(selectorBuilder,
                new StyleVariableAccessor(BaselineThemeProvider.Instance.ColorPalette));

            return ruleBuilder;
        }
    }
}
