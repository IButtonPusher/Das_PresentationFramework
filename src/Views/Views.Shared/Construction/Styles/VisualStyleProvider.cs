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

            _lockStylesByName = new Object();
            _stylesByName = new Dictionary<String, IStyleSheet?>();
            
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

                var resourceStyles = await _styleInflater.InflateResourceCssAsync(name).
                                                          ConfigureAwait(false);

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
        }

        public async Task<IStyleSheet?> GetStyleByNameAsync(String name)
        {
            lock (_lockStylesByClassName)
            {
                if (_stylesByName.TryGetValue(name, out var style))
                {
                    return style;
                }
            }
            
            var thisExe = Assembly.GetExecutingAssembly();
            var resourceNames = thisExe.GetManifestResourceNames();

            foreach (var resource in resourceNames)
            {
                if (!resource.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                    continue;

                lock (_resourcesLock)
                {
                    if (!_resourcesRead.Add(resource))
                        continue;
                }

                var resourceStyle = await _styleInflater.InflateResourceXmlAsync(resource).
                                                          ConfigureAwait(false);

                switch (resourceStyle)
                {
                    case NamedStyle named:

                        lock (_lockStylesByName)
                        {
                            _stylesByName.Add(named.Name, named);
                        }

                        if (String.Equals(named.Name, name))
                            return named;
                        
                        break;
                }
            }

            return default;
        }

        private readonly IStyleInflater _styleInflater;
        
        private readonly HashSet<String> _resourcesRead;
        private readonly Object _resourcesLock;

        private readonly Object _lockStylesByClassName;
        private readonly Dictionary<String, List<IStyleRule>> _stylesByClassName;
        
        private readonly Object _lockStylesByName;
        private readonly Dictionary<String, IStyleSheet?> _stylesByName;
    }
}