using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using AsyncResults.ForEach;
using Das.Views.Styles;

namespace Das.Views.Construction
{
    public class VisualStyleProvider : IVisualStyleProvider
    {
        public VisualStyleProvider(IStyleInflater styleInflater)
        {
            _styleInflater = styleInflater;

            _lockStylesByClassName = new Object();
            _stylesByClassName = new Dictionary<String, HashSet<IStyleRule>>();

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
                    {
                        yield return style;
                    }

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

                    TryAddRuleByClassName(selectorClassName, rule);

                    if (String.Equals(selectorClassName, className))
                        yield return rule;
                }
            }
        }

        public async Task<IStyleSheet?> GetStyleByNameAsync(String name)
        {
            lock (_lockStylesByClassName)
            {
                if (_stylesByName.TryGetValue(name, out var style))
                    return style;
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

                var resourceStyle = await _styleInflater.InflateResourceXmlAsync(resource).ConfigureAwait(false);

                switch (resourceStyle)
                {
                    case NamedTargetedStyle named:

                        lock (_lockStylesByName)
                        {
                            _stylesByName.Add(named.Name, named);
                            foreach (var rule in named.Rules)
                            {
                                if (rule.Selector.TryGetClassName(out var className))
                                    TryAddRuleByClassName(className, rule);
                            }
                        }

                        if (String.Equals(named.Name, name))
                            return named;

                        break;
                }
            }

            return default;
        }

        public async Task<IStyleSheet?> GetStyleForVisualAsync(IVisualElement visual,
                                                               IAttributeDictionary attributeDictionary)
        {
            if (visual.Style is { } appliedStyle && appliedStyle.StyleTemplate is { } styleSheet)
                return styleSheet;

            if (attributeDictionary.TryGetAttributeValue("class", out var className))
            {
                if (String.IsNullOrEmpty(className))
                    return default;

                var rules = await GetStylesByClassNameAsync(className).ToArrayAsync();

                var res = new NamedStyle(className, rules);
                return res;
            }

            if (attributeDictionary.TryGetAttributeValue("Style", out var styleName))
                return await GetStyleByNameAsync(styleName);

            return default;
        }

        private void TryAddRuleByClassName(String className,
                                           IStyleRule rule)
        {
            lock (_lockStylesByClassName)
            {
                if (!_stylesByClassName.TryGetValue(className, out var classRules))
                {
                    classRules = new HashSet<IStyleRule>();
                    _stylesByClassName.Add(className, classRules);
                }

                classRules.Add(rule);
            }
        }

        private readonly Object _lockStylesByClassName;

        private readonly Object _lockStylesByName;
        private readonly Object _resourcesLock;

        private readonly HashSet<String> _resourcesRead;

        private readonly IStyleInflater _styleInflater;
        private readonly Dictionary<String, HashSet<IStyleRule>> _stylesByClassName;
        private readonly Dictionary<String, IStyleSheet?> _stylesByName;
    }
}
