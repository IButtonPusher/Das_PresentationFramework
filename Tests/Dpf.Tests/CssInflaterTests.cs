using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Das.Views.Construction;
using Das.Views.Construction.Styles;
using Das.Views.Styles;
using Xunit;

namespace Dpf.Tests
{
    public class CssInflaterTests
    {
        [Fact]
        public void ParseMaterialSwitchCss()
        {
            var css = GetFileContents("material-switch.css");
            var nodes = CssNodeBuilder.GetMarkupNodes(css).ToArray();
            var selectors = new List<IStyleSelector>();
            var rules = new List<IStyleRule>();

            var styleContext = DefaultStyleContext.Instance;
            

            var visualAliases = new VisualAliasProvider();
            var selectorBuilder = new StyleSelectorBuilder(visualAliases);
            
            var variableAccessor = new StyleVariableAccessor();

            var defaultStyle = DefaultStyleContext.Instance;

            var ruleBuilder = new StyleRuleBuilder(selectorBuilder, variableAccessor);

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

        private static String GetFileContents(String fileName)
        {
            var fullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Files",
                fileName);

            return File.ReadAllText(fullName);

        }
    }
}
