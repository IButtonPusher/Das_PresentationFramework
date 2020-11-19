using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Rendering;

namespace Das.Views.Styles
{
    public class Style : IStyle
    {
        public Style()
        {
            _setters = new Dictionary<AssignedStyle, Object?>();
            _transitions = new Dictionary<AssignedStyle, Transition>();
        }

        public virtual Object? this[StyleSetter setter]
        {
            get => TryGetValue(setter, StyleSelector.None, out var found)
                ? found
                : default;
            set => AddSetter(setter, value);
        }

        public virtual Object? this[StyleSetter setter,
                                    StyleSelector selector]
        {
            get => TryGetValue(setter, selector, out var found)
                ? found
                : default;
            set => Add(setter, selector, value);
        }

        public virtual Boolean TryGetValue(StyleSetter setter,
                                           StyleSelector selector,
                                           out Object val)
        {
            foreach (var k in GetUniqueFlags<StyleSelector>(selector))
            {
                var key = new AssignedStyle(setter, k);
                if (_setters.TryGetValue(key, out val!))
                    return true;
            }

            val = default!;
            return false;
        }

        private static IEnumerable<T> GetUniqueFlags<T>(Enum flags)
            where T : Enum
        {
            foreach (Enum value in Enum.GetValues(flags.GetType()))
                if (flags.HasFlag(value))
                    yield return (T)value;
        }

        public virtual Boolean TryGetValue(StyleSetter setter,
                                           StyleSelector selector,
                                           Object? dataContext,
                                           out Object val)
        {
            return TryGetValue(setter, selector, out val);
        }


        public virtual IEnumerator<AssignedStyle> GetEnumerator()
        {
            return _setters.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddMissingSetters(IStyle fromStyle)
        {
            foreach (var kvp in fromStyle)
                if (!_setters.ContainsKey(kvp))
                    _setters[kvp] = kvp.Value;
        }

        public virtual void Add(StyleSetter setter,
                              StyleSelector selector,
                              Object? value)
        {
            var key = new AssignedStyle(setter, selector, value);
            _setters[key] = value;
        }

        protected virtual void UpdateTransition(AssignedStyle style)
        {
            _setters[style] = style.Value;
        }

        public void AddOrUpdate(IStyle style)
        {
            foreach (var kvp in style)
            {
                var key = new AssignedStyle(kvp.Setter, kvp.Selector, kvp.Value);
                _setters[key] = kvp;
            }
        }

        public virtual void AddSetter(StyleSetter setter,
                              Object? value)
        {
            Add(setter, StyleSelector.None, value);
        }

        protected readonly Dictionary<AssignedStyle, Object?> _setters;
        protected readonly Dictionary<AssignedStyle, Transition> _transitions;
    }
}