using System;
using System.Collections;
using System.Collections.Generic;
using Das.Views.Rendering;

namespace Das.Views.Styles
{
    public abstract class StyleBase : IEnumerable<AssignedStyle>
    {
        protected StyleBase()
        {
            _setters = new Dictionary<AssignedStyle, Object?>();
            _transitions = new Dictionary<AssignedStyle, Transition>();
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
        
        //public virtual Boolean TryGetValue(StyleSetter setter,
        //                                   StyleSelector selector,
        //                                   Object? dataContext,
        //                                   out Object val)
        //{
        //    return TryGetValue(setter, selector, out val);
        //}
        
        protected static IEnumerable<T> GetUniqueFlags<T>(Enum flags)
            where T : Enum
        {
            foreach (Enum value in Enum.GetValues(flags.GetType()))
                if (flags.HasFlag(value))
                    yield return (T)value;
        }
        
        protected void AddSetterImpl(StyleSetter setter,
                                      Object? value)
        {
            AddImpl(setter, StyleSelector.None, value);
        }
        
        protected void AddImpl(StyleSetter setter,
                                StyleSelector selector,
                                Object? value)
        {
            var key = new AssignedStyle(setter, selector, value);
            _setters[key] = value;
        }
        
        public virtual IEnumerator<AssignedStyle> GetEnumerator()
        {
            return _setters.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        protected readonly Dictionary<AssignedStyle, Object?> _setters;
        protected readonly Dictionary<AssignedStyle, Transition> _transitions;
    }
}
