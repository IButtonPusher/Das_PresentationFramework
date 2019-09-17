using System;
using System.Collections;
using System.Collections.Generic;

namespace Das.Views.Styles
{
    public class Style : IStyle
    {
        public virtual Object this[StyleSetters setter]
            => TryGetValue(setter, out var found) ? found : default;

        public virtual Boolean TryGetValue(StyleSetters setter, out Object val)
            => Setters.TryGetValue(setter, out val);

        public virtual Boolean TryGetValue(StyleSetters setter, Object dataContext, out Object val)
            => TryGetValue(setter, out val);

        public IDictionary<StyleSetters, Object> Setters { get; protected set; }

        public Style()
        {
            Setters = new Dictionary<StyleSetters, Object>();
        }

        public virtual IEnumerator<KeyValuePair<StyleSetters, Object>> GetEnumerator()
        {
            foreach (var kvp in Setters)
                yield return kvp;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}