using System;

namespace Das.Views
{
    public interface IDependencyProperty<out TValue> : IDependencyProperty
    {
        new TValue DefaultValue {get;}

        TValue GetValue<TVisual>(TVisual visual) where TVisual : IVisualElement;
    }
}
