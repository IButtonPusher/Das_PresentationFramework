using System;
using System.Threading.Tasks;

namespace Das.Views.Styles.Application
{
    public class ComputedValueAssignment : IPropertyValueAssignment
    {
        public ComputedValueAssignment(IVisualElement visual,
                                       IDependencyProperty dependencyProperty,
                                       Func<IVisualElement, Object?> valueBuilder)
        {
            Visual = visual;
            Property = dependencyProperty;
            _valueBuilder = valueBuilder;
        }

        public IVisualElement Visual { get; }

        public Boolean DoOverlap(IStyleValueAssignment other)
        {
            return other is ComputedValueAssignment applied &&
                   applied.Property.Equals(Property);
        }

        public void Execute(Boolean isUpdate)
        {
            Property.SetComputedValueFromStyle(Visual, _valueBuilder);
        }

        public IDependencyProperty Property { get; }

        private readonly Func<IVisualElement, Object?> _valueBuilder;
    }
}