using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Construction;
using Das.Views.Construction.Styles;
using Das.Views.Styles;
using Das.Views.Styles.Construction;
using Xunit;
// ReSharper disable All

namespace Dpf.Tests
{
    public class StyleInflaterTests : TestBase
    {
        [Fact]
        public void ParseMaterialSwitchCss()
        {
            var css = GetFileContents("material-switch.css");
            var nodes = CssNodeBuilder.GetMarkupNodes(css).ToArray();
            
            var rules = new List<IStyleRule>();

            var visualAliases = new VisualAliasProvider();
            var selectorBuilder = new CssStyleSelectorBuilder(visualAliases);
            var themeProvider = BaselineThemeProvider.Instance;
            
            var variableAccessor = new StyleVariableAccessor(themeProvider.ColorPalette);

            var ruleBuilder = new CssRuleBuilder(selectorBuilder, variableAccessor);

            var index = 0;

            foreach (var node in nodes)
            {
                var rule = ruleBuilder.GetRule(node);

                //var selector = selectorBuilder.GetSelector(node);
                if (rule == null)
                    throw new InvalidOperationException();

                // selectors.Add(selector);
                rules.Add(rule);

                index++;
            }
        }

        [Fact]
        public void ParseMaterialSwitchXml()
        {
            //var xml = GetResourceContents("abc");
            
            var inflater = new DefaultStyleInflater(Serializer.TypeInferrer,
                new StyleVariableAccessor(BaselineThemeProvider.Instance.ColorPalette));
            var provider = new VisualStyleProvider(inflater,
                new ConcurrentDictionary<Type, IEnumerable<IStyleRule>>());

            var bob = provider.GetStyleByNameAsync("mat-toggle-button").Result;

            //var rizzo = GetResourceContents("bob");
        }

        private static String GetResourceContents(String resourceName)
        {
            var thisExe = Assembly.GetExecutingAssembly();
            var names = thisExe.GetManifestResourceNames();
            throw new NotImplementedException();
        }

        private static readonly DasSerializer Serializer = new DasSerializer();

       
    }
}
