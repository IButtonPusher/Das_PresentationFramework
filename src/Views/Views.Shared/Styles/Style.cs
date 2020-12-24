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
            get => TryGetValue(setterType, VisualStateType.None, out var found)
                ? found
                : default;
            set => AddSetterImpl(setterType, value);
        }

        public virtual Object? this[StyleSetterType setterType,
                                    VisualStateType type]
        {
            get => TryGetValue(setterType, type, out var found)
                ? found
                : default;
            set => AddImpl(setterType, type, value);
        }

        public virtual void Add(StyleSetterType setterType,
                                VisualStateType type,
                                Object? value)
        {
            AddImpl(setterType, type, value);
        }

        public void AddOrUpdate(IStyle style)
        {
            foreach (var kvp in style)
            {
                var key = new AssignedStyle(kvp.SetterType, kvp.Type, kvp.Value);
                _setters[key] = kvp;
            }
        }

        public virtual void AddSetter(StyleSetterType setterType,
                                      Object? value)
        {
            Add(setterType, VisualStateType.None, value);
        }


        public void AddMissingSetters(IStyle fromStyle)
        {
            foreach (var kvp in fromStyle)
                if (!_setters.ContainsKey(kvp))
                    _setters[kvp] = kvp.Value;
        }

        protected void AddOrUpdate(IEnumerable<AssignedStyle> newValues)
        {
            foreach (var value in newValues) _setters[value] = value.Value;
        }

        protected virtual void UpdateTransition(AssignedStyle style)
        {
            _setters[style] = style.Value;
        }
    }
}