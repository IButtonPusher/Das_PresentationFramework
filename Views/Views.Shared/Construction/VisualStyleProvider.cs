using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Das.Views.Styles;

namespace Das.Views.Construction
{
    public class VisualStyleProvider : IVisualStyleProvider
    {
        public VisualStyleProvider(IStyleInflater styleInflater)
        {
            _styleInflater = styleInflater;
            _lockStylesByClassName = new Object();
            _stylesByClassName = new Dictionary<String, List<IStyleRule>>();

            _resourcesLock = new Object();
            _resourcesRead = new HashSet<String>();
        }

        public async IAsyncEnumerable<IStyleRule> GetStylesByClassNameAsync(String className)
        {
            lock (_lockStylesByClassName)
            {
                if (_stylesByClassName.TryGetValue(className, out var styles))
                {
                    foreach (var style in styles)
                        yield return style;

                    yield break;
                }
            }

            var thisExe = Assembly.GetExecutingAssembly();
            var streamNames = thisExe.GetManifestResourceNames();

            foreach (var name in streamNames)
            {
                if (!name.EndsWith(".css", StringComparison.OrdinalIgnoreCase))
                    continue;

                lock (_resourcesLock)
                {
                    if (!_resourcesRead.Add(name))
                        continue;
                }

                var resourceStyles = await _styleInflater.InflateResourceCssAsync(name).ConfigureAwait(false);

                if (resourceStyles == null)
                    continue;

                foreach (var rule in resourceStyles.Rules)
                {
                    if (!rule.Selector.TryGetClassName(out var selectorClassName)) 
                        continue;
                    
                    lock (_lockStylesByClassName)
                    {
                        if (!_stylesByClassName.TryGetValue(selectorClassName, out var classRules))
                        {
                            classRules = new List<IStyleRule>();
                            _stylesByClassName[selectorClassName] = classRules;
                        }

                        classRules.Add(rule);
                    }

                    if (String.Equals(selectorClassName, className))
                    {
                        yield return rule;
                    }
                }
            }


            await Task.Yield();
        }

        private readonly Object _lockStylesByClassName;
        private readonly Object _resourcesLock;

        private readonly HashSet<String> _resourcesRead;
        private readonly IStyleInflater _styleInflater;

        private readonly Dictionary<String, List<IStyleRule>> _stylesByClassName;
    }
}