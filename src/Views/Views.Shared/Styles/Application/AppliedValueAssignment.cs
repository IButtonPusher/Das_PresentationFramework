using System;
using System.Threading.Tasks;

namespace Das.Views.Styles.Application
{
    public class AppliedValueAssignment : IStyleValueAssignment
    {
        public AppliedValueAssignment(IVisualElement visual,
                                      IDependencyProperty dependencyProperty,
                                      Object? value)
        {
            _visual = visual;
            _dependencyProperty = dependencyProperty;
            _value = value;
        }

        public void Execute()
        {
            _dependencyProperty.SetValue(_visual, _value);
        }

        public override String ToString()
        {
            return _visual + "." + _dependencyProperty.Name + " = " + _visual;
        }

        private readonly IDependencyProperty _dependencyProperty;
        private readonly Object? _value;
        private readonly IVisualElement _visual;
    }
}