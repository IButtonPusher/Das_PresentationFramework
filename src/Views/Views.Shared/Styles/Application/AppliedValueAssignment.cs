using System;
using System.Threading.Tasks;

namespace Das.Views.Styles.Application
{
    public class AppliedValueAssignment : IPropertyValueAssignment
    {
        public AppliedValueAssignment(IVisualElement visual,
                                      IDependencyProperty dependencyProperty,
                                      Object? value)
        {
            if (dependencyProperty.Name == "Background")
            {}

            Visual = visual;
            _dependencyProperty = dependencyProperty;
            _value = value;
        }

        public void Execute(Boolean isUpdate)
        {
            if (isUpdate)
                _dependencyProperty.SetValue(Visual, _value);
            else
                _dependencyProperty.SetValueNoTransitions(Visual, _value);
        }

        public Boolean DoOverlap(IStyleValueAssignment other)
        {
            return other is AppliedValueAssignment applied &&
                   applied._dependencyProperty.Equals(_dependencyProperty);
        }

        public override String ToString()
        {
            return Visual + "." + _dependencyProperty.Name + " = " + _value;
        }

        private readonly IDependencyProperty _dependencyProperty;
        private readonly Object? _value;

        public IVisualElement Visual { get; }

        public IDependencyProperty Property => _dependencyProperty;
    }
}