using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Das.Views.Styles
{
    public class Style : IStyle
    {
        public Style()
        {
            Setters = new Dictionary<StyleSetters, Object>();
        }

        public IDictionary<StyleSetters, Object> Setters { get; protected set; }

        public virtual Object this[StyleSetters setter]
            => TryGetValue(setter, out var found) ? found : default;

        public virtual Boolean TryGetValue(StyleSetters setter, out Object val)
        {
            return Setters.TryGetValue(setter, out val);
        }

        public virtual Boolean TryGetValue(StyleSetters setter, Object dataContext, out Object val)
        {
            return TryGetValue(setter, out val);
        }

        public virtual IEnumerator<KeyValuePair<StyleSetters, Object>> GetEnumerator()
        {
            foreach (var kvp in Setters)
                yield return kvp;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}