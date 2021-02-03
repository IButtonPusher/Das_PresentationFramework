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
            Visual = visual;
            Property = dependencyProperty;
            _value = value;
        }

        public void Execute(Boolean isUpdate)
        {
            if (isUpdate)
                Property.SetValueFromStyle(Visual, _value);
            else
                Property.SetValueNoTransitions(Visual, _value);
        }

        public Boolean DoOverlap(IStyleValueAssignment other)
        {
            return other is AppliedValueAssignment applied &&
                   applied.Property.Equals(Property);
        }

        public IVisualElement Visual { get; }

        public IDependencyProperty Property { get; }

        public override String ToString()
        {
            return Visual + "." + Property.Name + " = " + _value;
        }

        private readonly Object? _value;
    }
}
