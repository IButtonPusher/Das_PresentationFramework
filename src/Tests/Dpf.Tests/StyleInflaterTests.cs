using System;
using System.Collections.Generic;
using System.IO;
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
    public class StyleInflaterTests
    {
        [Fact]
        public void ParseMaterialSwitchCss()
        {
            var css = GetFileContents("material-switch.css");
            var nodes = CssNodeBuilder.GetMarkupNodes(css).ToArray();
            //var selectors = new List<IStyleSelector>();
            var rules = new List<IStyleRule>();

            var styleContext = DefaultStyleContext.Instance;
            

            var visualAliases = new VisualAliasProvider();
            var selectorBuilder = new CssStyleSelectorBuilder(visualAliases);
            
            var variableAccessor = new StyleVariableAccessor();

            var defaultStyle = DefaultStyleContext.Instance;

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
            
            var inflater = new DefaultStyleInflater(Serializer.TypeInferrer);
            var provider = new VisualStyleProvider(inflater);

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

        private static String GetFileContents(String fileName)
        {
            var fullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Files",
                fileName);

            return File.ReadAllText(fullName);

        }
    }
}
