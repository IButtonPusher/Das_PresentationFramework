using System;
using Das.Views.Transitions;

namespace Das.Views
{
    public interface IDependencyProperty : IEquatable<IDependencyProperty>
    {
        Object? GetValue(IVisualElement visual);

        
        void SetValue(IVisualElement visual,
                      Object? value);

        void SetValueNoTransitions(IVisualElement visual,
                      Object? value);

        void SetValueFromStyle(IVisualElement visual,
                               Object? value);
        
        void SetComputedValueFromStyle(IVisualElement visual,
                               Func<IVisualElement, Object?> value);

        void AddOnChangedHandler(IVisualElement visual,
                                 Action<IDependencyProperty> onChange);

        //ITransition BuildTransition(Double duration,
                                    //Double? delay);

        void AddTransition(IVisualElement visual,
                           ITransition transition);

        Object? DefaultValue { get; }

        String Name { get; }
        
        Type PropertyType { get; }
        
        Type VisualType { get; }
    }
}
