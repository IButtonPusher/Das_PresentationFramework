using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Das.Views.Styles
{
    [ContentProperty(nameof(StyleSetters))]
    public class StyleSheet : Style,
                              IStyleSheet
    {
        public StyleSheet(IEnumerable<IStyleRule> rules)
        {
            VisualTypeStyles = new ConcurrentDictionary<Type, IStyleSheet>();
            _rules = new List<IStyleRule>(rules);
        }

        public IEnumerable<IStyleRule> Rules => _rules;

        public IDictionary<Type, IStyleSheet> VisualTypeStyles { get; }

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

        protected readonly List<IStyleRule> _rules;
    }
}
