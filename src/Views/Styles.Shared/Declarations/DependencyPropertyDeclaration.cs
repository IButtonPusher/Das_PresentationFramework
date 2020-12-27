using System;
using System.Threading.Tasks;
using Das.Views.Rendering;
using Das.Views.Styles.Declarations;

namespace Das.Views.Styles
{
    public class DependencyPropertyDeclaration<TVisual, TValue> :
        IStyleDeclaration
        where TVisual : IVisualElement
    {
        public DependencyPropertyDeclaration(IDependencyProperty<TVisual, TValue> dependencyProperty,
                                             TValue value,
                                             Transition? transition,
                                             DeclarationProperty property)
        {
            _dependencyProperty = dependencyProperty;
            _value = value;
            _transition = transition;
            Property = property;
        }

        public DeclarationProperty Property { get; }

        public void AssignValueToVisual(IVisualElement visual)
        {
            if (!(visual is TVisual valid))
                return;

            _dependencyProperty.SetValue(valid, _value);
        }

        private readonly IDependencyProperty<TVisual, TValue> _dependencyProperty;
        private readonly Transition? _transition;
        private readonly TValue _value;
    }
}