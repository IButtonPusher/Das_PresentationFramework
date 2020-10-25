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
            _setters = new Dictionary<AssignedStyle, Object?>();
            //Setters = new Dictionary<StyleSetter, Object?>();
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
            //var key = (Int32)setter + ((Int32)selector << 16);
            var key = new AssignedStyle(setter, selector);

            return _setters.TryGetValue(key, out val!);
        }

        public virtual Boolean TryGetValue(StyleSetter setter,
                                           StyleSelector selector,
                                           Object dataContext,
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

        public void Add(StyleSetter setter,
                              StyleSelector selector,
                              Object? value)
        {
            var key = new AssignedStyle(setter, selector, value);
            //var key = (Int32)setter + ((Int32)selector << 16);
            _setters[key] = value;
        }

        public void AddSetter(StyleSetter setter,
                              Object? value)
        {
            Add(setter, StyleSelector.None, value);
        }

        //private IDictionary<StyleSetter, Object?> Setters { get; protected set; }
        private readonly Dictionary<AssignedStyle, Object?> _setters;
    }
}