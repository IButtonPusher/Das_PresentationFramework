using System.Collections;
using System.Collections.Generic;

namespace Das.Views.Styles
{
    public class Style : IStyle
    {
        public virtual object this[StyleSetters setter]
            => TryGetValue(setter, out var found) ? found : default;

        public virtual bool TryGetValue(StyleSetters setter, out object val)
            => Setters.TryGetValue(setter, out val);

        public virtual bool TryGetValue(StyleSetters setter, object dataContext, out object val)
            => TryGetValue(setter, out val);

        public IDictionary<StyleSetters, object> Setters { get; protected set; }

        public Style()
        {
            Setters = new Dictionary<StyleSetters, object>();
        }

        public virtual IEnumerator<KeyValuePair<StyleSetters, object>> GetEnumerator()
        {
            foreach (var kvp in Setters)
                yield return kvp;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}