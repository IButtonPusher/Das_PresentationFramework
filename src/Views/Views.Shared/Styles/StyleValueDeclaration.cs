using System;
using Das.Views.Rendering;

namespace Das.Views.Styles
{
    public class StyleValueDeclaration : IStyleDeclaration
    {
        public StyleValueDeclaration(IDependencyProperty property, 
                                     StyleSetterType setterType, 
                                     Object? value, 
                                     Transition? transition)
        {
            Property = property;
            SetterType = setterType;
            Value = value;
            Transition = transition;
        }

        public IDependencyProperty Property { get; }

        public StyleSetterType SetterType { get; }

        public Object? Value { get; }

        public Transition? Transition { get; }
    }
}
