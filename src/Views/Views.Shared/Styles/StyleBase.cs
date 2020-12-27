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
        
        //public IEnumerable<IStyleRule> Rules => throw new NotImplementedException();

        public Boolean TryGetValue<T>(IVisualElement visual, out T value)
        {
            throw new NotImplementedException();
        }
        
        public virtual Boolean TryGetValue(StyleSetterType setterType,
                                           VisualStateType type,
                                           out Object val)
        {
            foreach (var k in GetUniqueFlags<VisualStateType>(type))
            {
                var key = new AssignedStyle(setterType, k);
                if (_setters.TryGetValue(key, out val!))
                    return true;
            }

            val = default!;
            return false;
        }
        
      
        
        protected static IEnumerable<T> GetUniqueFlags<T>(Enum flags)
            where T : Enum
        {
            foreach (Enum value in Enum.GetValues(flags.GetType()))
                if (flags.HasFlag(value))
                    yield return (T)value;
        }
        
        protected void AddSetterImpl(StyleSetterType setterType,
                                      Object? value)
        {
            AddImpl(setterType, VisualStateType.None, value);
        }
        
        protected void AddImpl(StyleSetterType setterType,
                                VisualStateType type,
                                Object? value)
        {
            var key = new AssignedStyle(setterType, type, value);
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
        
        public IEnumerable<AssignedStyle> Setters => _setters.Keys;
        
        protected readonly Dictionary<AssignedStyle, Object?> _setters;
        protected readonly Dictionary<AssignedStyle, Transition> _transitions;
    }
}
