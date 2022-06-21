using System;
using System.Threading.Tasks;
using Das.Views.DependencyProperties;
using Das.Views.Transitions;

namespace Das.Views
{
    public interface IDependencyProperty : IEquatable<IDependencyProperty>,
                                           INamedProperty
    {
        Object? GetValue(IVisualElement visual);


        void SetValue(IVisualElement visual,
                      Object? value);

        void SetValueNoTransitions(IVisualElement visual,
                                   Object? value);

        void SetValueFromStyle(IVisualElement visual,
                               Object? value);

        void ClearValue(IVisualElement visual);

        void SetComputedValueFromStyle(IVisualElement visual,
                                       Func<IVisualElement, Object?> value);

        void AddOnChangedHandler(IVisualElement visual,
                                 Action<IDependencyProperty> onChange);

        void AddTransition(IVisualElement visual,
                           ITransition transition);

        Object? DefaultValue { get; }

        Type PropertyType { get; }

        Type VisualType { get; }

        Boolean IsReadOnly { get; }
    }
}
