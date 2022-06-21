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
        public StyleInflater(ICssRuleBuilder cssRuleBuilder,
                             IXmlStyleRuleBuilder xmlRuleBuilder,
                             IVisualTypeResolver visualTypeResolver)
        {
            _cssRuleBuilder = cssRuleBuilder;
            _xmlRuleBuilder = xmlRuleBuilder;
            _visualTypeResolver = visualTypeResolver;
        }

        public ICascadingStyle InflateCss(String css)
        {
            var nodes = CssNodeBuilder.GetMarkupNodes(css);
            var rules = new List<IStyleRule>();

            foreach (var node in nodes)
            {
                var rule = _cssRuleBuilder.GetRule(node);

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

        public ICascadingStyle InflateResourceCss(String resourceName)
        {
            var css = GetStringFromResource(resourceName);
            return InflateCss(css);
        }

        public IStyleSheet InflateXml(String xml)
        {
            var node = GetRootNode(xml);

            if (!node.TryGetAttributeValue(nameof(NamedTargetedStyle.Name), out var styleName) || 
                !node.TryGetAttributeValue(nameof(NamedTargetedStyle.TargetType), out var targetTypeName))
                throw new NotImplementedException();

            var targetType = _visualTypeResolver.GetType(targetTypeName);

            var rules = GetStyleRules(node.Children, targetType);

            var namedStyle = new NamedTargetedStyle(styleName, targetType, rules);
            return namedStyle;
        }

        public async Task<IStyleSheet> InflateResourceXmlAsync(String resourceName)
        {
            var xml = await GetStringFromResourceAsync(resourceName).ConfigureAwait(false);
            return InflateXml(xml);
        }

        private IStyleRule? GetStyleRule(IMarkupNode setterNode,
                                         Type targetType)
        {
            var rule = _xmlRuleBuilder.GetRule(setterNode, targetType);
            return rule;
        }

        private IEnumerable<IStyleRule> GetStyleRules(IEnumerable<IMarkupNode> children,
                                                      Type targetType)
        {
            foreach (var child in children)
            foreach (var rule in GetStyleRules(child, targetType))
            {
                yield return rule;
            }
        }

        private IEnumerable<IStyleRule> GetStyleRules(IMarkupNode child,
                                                      Type targetType)
        {
            switch (child.Name)
            {
                case nameof(IStyle.Setters):
                    foreach (var setterNode in child.Children)
                    foreach (var rule in GetStyleRules(setterNode, targetType))
                    {
                        yield return rule;
                    }

                    break;

                case "Setter":

                    var styleRule = GetStyleRule(child, targetType);
                    if (styleRule != null)
                        yield return styleRule;

                    break;

                case "Rules":
                    var css = child.InnerText;
                    if (css != null)
                        foreach (var cssRule in _cssRuleBuilder.GetRules(css))
                        {
                            yield return cssRule;
                        }

                    break;
            }
        }

        private readonly ICssRuleBuilder _cssRuleBuilder;
        private readonly IVisualTypeResolver _visualTypeResolver;
        private readonly IXmlStyleRuleBuilder _xmlRuleBuilder;
    }
}
