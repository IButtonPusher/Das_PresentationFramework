using System;

namespace Das.Views.Styles.Application
{
    public class AppliedStyleCondition : IStyleCondition
    {
        public IVisualElement Visual { get; }

        public IDependencyProperty DependencyProperty { get; }

        public Boolean Value { get; }

        public AppliedStyleCondition(IVisualElement visual,
                                     IDependencyProperty dependencyProperty,
                                     Boolean value)
        {
            Visual = visual;
            DependencyProperty = dependencyProperty;
            Value = value;
        }

        public override String ToString()
        {
            return Visual + "." + DependencyProperty.Name + " == " + Value;
        }

        public Boolean CanExecute()
        {
            return Equals(DependencyProperty.GetValue(Visual), Value);
        }
    }
}
