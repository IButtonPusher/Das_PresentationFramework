using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Das.Container;
using Das.ViewModels.Collections;
using Das.Views.Styles;

namespace Das.Views.Construction
{
    public class VisualStyleProvider : IVisualStyleProvider
    {
        [ContainerConstructor]
        // ReSharper disable once UnusedMember.Global
        public VisualStyleProvider(IStyleInflater styleInflater)
        : this(styleInflater, new Dictionary<Type, IEnumerable<IStyleRule>>())
        {
            
        }

        public VisualStyleProvider(IStyleInflater styleInflater,
                                   IDictionary<Type, IEnumerable<IStyleRule>> visualTypeRules)
        {
            _styleInflater = styleInflater;
            _cachedTypeStyleRules = new ConcurrentDictionary<Type, HashSet<IStyleRule>>();
            _visualTypeRules = new ConcurrentDictionary<Type, List<IStyleRule>>();
            _cookedTypeStyles = new ConcurrentDictionary<Type, IStyleSheet>();

            foreach (var kvpp in visualTypeRules)
            {
                _visualTypeRules[kvpp.Key] = new List<IStyleRule>(kvpp.Value);
            }

            //_themeProvider = themeProvider;

            _lockStylesByClassName = new Object();
            _stylesByClassName = new Dictionary<String, HashSet<IStyleRule>>();

            _lockStylesByName = new Object();
            _stylesByName = new Dictionary<String, IStyleSheet?>();

            _resourcesLock = new Object();
            _resourcesRead = new HashSet<String>();

            _typeClassStyles = new DoubleConcurrentDictionary<Type, String, IStyleSheet>();
            //_typeStyleStyles = new DoubleConcurrentDictionary<Type, String, IStyleSheet>();
        }

        public async Task<IStyleSheet?> GetStyleForVisualAsync(IVisualElement visual,
                                                               IAttributeDictionary attributeDictionary)
        {
            if (TryGetExistingStyle(visual, out var existing))
                return existing;

            var vType = visual.GetType();

            if (attributeDictionary.TryGetAttributeValue("class", out var className) && 
                !String.IsNullOrEmpty(className))
            {
                var typeStyles = GetOrBuildTypeStyleRules(vType);

                var classRules = new HashSet<IStyleRule>();

                foreach (var r in GetStylesByClassName(className))
                    classRules.Add(r);

                foreach (var r in typeStyles)
                {
                    classRules.Add(r);
                }

                var res = new NamedStyle(className, classRules);
                _typeClassStyles[vType, className] = res;
                return res;
            }

            if (attributeDictionary.TryGetAttributeValue("Style", out var styleName))
            {
                var typeStyles = GetOrBuildTypeStyleRules(vType);

                var rules = await GetStyleByNameAsync(styleName);

                if (rules != null)
                {
                    if (typeStyles.Count > 0)
                        return rules.AddDefaultRules(typeStyles);

                    return rules;
                }
            }

            // the visual should have had no class/Style specified by the time we get here
            return GetOrBuildTypeStyle(vType);
        }

        private static Boolean TryGetExistingStyle(IVisualElement visual,
                                                   out IStyleSheet existing)
        {
            if (visual.Style is { } appliedStyle && appliedStyle.StyleTemplate is { } styleSheet)
            {
                // visual's style already populated.  Probably shouldn't be here but seems no harm for now...
                existing = styleSheet;
                return true;
            }

            existing = default!;
            return false;
        }

        public IStyleSheet? GetCoreStyleForVisual(IVisualElement visual)
        {
            if (TryGetExistingStyle(visual, out var existing))
                return existing;

            return GetOrBuildTypeStyle(visual.GetType());

        }


        private IStyleSheet GetOrBuildTypeStyle(Type type)
        {
            return _cookedTypeStyles.GetOrAdd(type, BuildTypeStyle);
        }

        private IStyleSheet BuildTypeStyle(Type type)
        {
            var rules = GetOrBuildTypeStyleRules(type);
            if (rules.Count == 0)
                return StyleSheet.Empty;

            return new StyleSheet(rules);
        }

        /// <summary>
        /// Includes base types' style rules
        /// </summary>
        private HashSet<IStyleRule> GetOrBuildTypeStyleRules(Type type)
        {
            return _cachedTypeStyleRules.GetOrAdd(type, BuildTypeStyleRules);
        }

        private HashSet<IStyleRule> BuildTypeStyleRules(Type type)
        {
            var typeStyles = new HashSet<IStyleRule>();
            var currentType = type;
            do
            {
                if (_visualTypeRules.TryGetValue(currentType, out var typeRules))
                {
                    foreach (var rule in typeRules)
                    {
                        //if (!typeStyles.Contains(rule))
                        typeStyles.Add(rule);
                    }
                }

                currentType = currentType.BaseType;

            } while (currentType != null &&
                     typeof(IVisualElement).IsAssignableFrom(currentType));

            return typeStyles;
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


        private IEnumerable<IStyleRule> GetStylesByClassName(String className)
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

                var resourceStyles = _styleInflater.InflateResourceCss(name);

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
        
        /// <summary>
        /// Includes type-inherited style rules
        /// </summary>
        private readonly ConcurrentDictionary<Type, HashSet<IStyleRule>> _cachedTypeStyleRules;

        /// <summary>
        /// cached style sheets for objects that have no affiliated class/style
        /// </summary>
        private readonly ConcurrentDictionary<Type, IStyleSheet> _cookedTypeStyles;

        private readonly IStyleInflater _styleInflater;
        private readonly Dictionary<String, HashSet<IStyleRule>> _stylesByClassName;
        private readonly Dictionary<String, IStyleSheet?> _stylesByName;
        private readonly DoubleConcurrentDictionary<Type, String, IStyleSheet> _typeClassStyles;
        
        
        /// <summary>
        /// specific to a type, no type inheritance factored in
        /// </summary>
        private readonly IDictionary<Type, List<IStyleRule>> _visualTypeRules;

        //private IThemeProvider _themeProvider;
        //private readonly DoubleConcurrentDictionary<Type, String, IStyleSheet> _typeStyleStyles;
    }
}
