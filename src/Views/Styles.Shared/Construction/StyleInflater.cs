using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Construction;
using Das.Views.Construction.Styles;

namespace Das.Views.Styles.Construction
{
    public class StyleInflater : InflaterBase,
                                 IStyleInflater
    {
        private readonly IStyleRuleBuilder _ruleBuilder;

        public StyleInflater(IStyleRuleBuilder ruleBuilder)
        {
            _ruleBuilder = ruleBuilder;
        }
        
        public ICascadingStyle InflateCss(String css)
        {
            var nodes = CssNodeBuilder.GetMarkupNodes(css);
            var rules = new List<IStyleRule>();
            
            foreach (var node in nodes)
            {
                var rule = _ruleBuilder.GetRule(node);

                if (rule == null)
                    throw new InvalidOperationException();

                rules.Add(rule);
            }

            return new CascadingStyle(rules);
        }

        public async Task<ICascadingStyle> InflateResourceCssAsync(String resourceName)
        {
            var css = await GetStringFromResourceAsync(resourceName);
            return InflateCss(css);
        }

        public ICascadingStyle InflateXml(String xml)
        {
            throw new NotImplementedException();
        }

        public Task<ICascadingStyle> InflateResourceXmlAsync(String resourceName)
        {
            throw new NotImplementedException();
        }
    }
}
