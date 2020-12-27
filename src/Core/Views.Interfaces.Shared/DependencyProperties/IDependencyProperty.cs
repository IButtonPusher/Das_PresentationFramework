using System;

namespace Das.Views
{
    public interface IDependencyProperty
    {
        Object? GetValue(IVisualElement visual);

        void SetValue(IVisualElement visual,
                      Object? value);

        void SetValueFromStyle(IVisualElement visual,
                               Object? value);
        
        void SetComputedValueFromStyle(IVisualElement visual,
                               Func<IVisualElement, Object?> value);

        String Name { get; }
        
        Type PropertyType { get; }
        
        Type VisualType { get; }
    }
}
