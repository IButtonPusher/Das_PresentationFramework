using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Das.Views.Styles
{
    
    public class Style : StyleBase, 
                         IStyle
    {
        public virtual Object? this[StyleSetterType setterType]
        {
            get => TryGetValue(setterType, StyleSelector.None, out var found)
                ? found
                : default;
            set => AddSetterImpl(setterType, value);
        }

        public virtual Object? this[StyleSetterType setterType,
                                    StyleSelector selector]
        {
            get => TryGetValue(setterType, selector, out var found)
                ? found
                : default;
            set => AddImpl(setterType, selector, value);
        }

       
       

        public void AddMissingSetters(IStyle fromStyle)
        {
            foreach (var kvp in fromStyle)
                if (!_setters.ContainsKey(kvp))
                    _setters[kvp] = kvp.Value;
        }

        public virtual void Add(StyleSetterType setterType,
                              StyleSelector selector,
                              Object? value)
        {
            AddImpl(setterType, selector, value);
        }

        protected virtual void UpdateTransition(AssignedStyle style)
        {
            _setters[style] = style.Value;
        }

        public void AddOrUpdate(IStyle style)
        {
            foreach (var kvp in style)
            {
                var key = new AssignedStyle(kvp.SetterType, kvp.Selector, kvp.Value);
                _setters[key] = kvp;
            }
        }

        protected void AddOrUpdate(IEnumerable<AssignedStyle> newValues)
        {
            foreach (var value in newValues)
            {
                _setters[value] = value.Value;
            }
        }

        public virtual void AddSetter(StyleSetterType setterType,
                              Object? value)
        {
            Add(setterType, StyleSelector.None, value);
        }

    }
}