using System;

namespace Das.Views.Styles.Application
{
    public class ComputedValueAssignment : IStyleValueAssignment
    {
        private readonly IVisualElement _visual;
        private readonly IDependencyProperty _dependencyProperty;
        private readonly Func<IVisualElement, Object?> _valueBuilder;

        public ComputedValueAssignment(IVisualElement visual,
                                       IDependencyProperty dependencyProperty,
                                       Func<IVisualElement, Object?> valueBuilder)
        {
            _visual = visual;
            _dependencyProperty = dependencyProperty;
            _valueBuilder = valueBuilder;
        }

        public void Execute()
        {
            _dependencyProperty.SetComputedValueFromStyle(_visual, _valueBuilder);
        }
    }
}
