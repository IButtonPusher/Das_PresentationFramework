using System;

namespace Das.Views
{
    public interface IDependencyProperty<out TValue> : IDependencyProperty
    {
        new TValue DefaultValue {get;}

        new TValue GetValue(IVisualElement visual);
    }
}
