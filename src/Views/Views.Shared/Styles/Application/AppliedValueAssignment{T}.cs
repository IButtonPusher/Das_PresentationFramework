using System;

namespace Das.Views.Styles.Application
{
    public class AppliedValueAssignment<TVisual, TValue> : IPropertyValueAssignment<TVisual, TValue>
        where TVisual : IVisualElement
    {
        public AppliedValueAssignment(TVisual visual,
                                      IDependencyProperty<TVisual, TValue> dependencyProperty,
                                      TValue value)
        {
            Visual = visual;
            Property = dependencyProperty;
            _value = value;
        }

        public void Execute(Boolean isUpdate)
        {
            if (isUpdate)
                Property.SetValue(Visual, _value);
            else
                Property.SetValueNoTransitions(Visual, _value);
        }

        public Boolean DoOverlap(IStyleValueAssignment other)
        {
            return other is AppliedValueAssignment applied &&
                   applied.Property.Equals(Property);
        }

        public TValue Value => _value;

        IVisualElement IStyleValueAssignment.Visual => Visual;

        public TVisual Visual { get; }

        public IDependencyProperty<TVisual, TValue> Property { get; }

        public override String ToString()
        {
            return Visual + "." + Property.Name + " = " + _value;
        }

        private readonly TValue _value;
    }
}
