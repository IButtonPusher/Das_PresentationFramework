using System;
using System.Threading.Tasks;
using Das.Views.Rendering;

namespace Das.Views.Styles
{
    public class StyleValueDeclaration<T> : IStyleDeclaration<T>
    {
        public StyleValueDeclaration(IDependencyProperty property, 
                                     StyleSetterType setterType, 
                                     T value,
                                     Transition? transition)
        {
            _value = value;
            Property = property;
            SetterType = setterType;
            Transition = transition;
        }

        public IDependencyProperty Property { get; }

        public StyleSetterType SetterType { get; }

        T IStyleDeclaration<T>.Value => _value;

        Object? IStyleDeclaration.Value => _value;

        public Transition? Transition { get; }

        private readonly T _value;
    }
}