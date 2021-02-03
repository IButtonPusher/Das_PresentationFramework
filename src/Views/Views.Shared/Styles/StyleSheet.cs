using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Das.Views.Styles
{
    [ContentProperty(nameof(StyleSetters))]
    public class StyleSheet : Style,
                              IStyleSheet
    {
        public StyleSheet(IEnumerable<IStyleRule> rules)
        {
            VisualTypeStyles = new ConcurrentDictionary<Type, IStyleSheet>();
            _rules = new HashSet<IStyleRule>(rules);

            IsEmpty = _rules.Count == 0;
        }

        public static readonly StyleSheet Empty = new(Enumerable.Empty<IStyleRule>());

        public IEnumerable<IStyleRule> Rules => _rules;

        public IDictionary<Type, IStyleSheet> VisualTypeStyles { get; }

        public IStyleSheet AddDefaultRules(IEnumerable<IStyleRule> rules)
        {
            var anyNew = false;
            var newRules = new HashSet<IStyleRule>(_rules);
            foreach (var rule in rules)
            {
                if (newRules.Add(rule))
                    anyNew = true;
            }

            if (!anyNew)
                return this;

            return new StyleSheet(newRules);
        }

        public Boolean IsEmpty { get; }

        public IEnumerable<IStyleSetter> StyleSetters
        {
            get => StyleSheetHelper.GetAllSetters(this);
            set => UpdateSetters(value);
        }

        protected void UpdateSetters(IEnumerable<IStyleSetter> setters)
        {
            _setters.Clear();
            VisualTypeStyles.Clear();

            throw new NotImplementedException();
        }

        protected readonly HashSet<IStyleRule> _rules;
    }
}
