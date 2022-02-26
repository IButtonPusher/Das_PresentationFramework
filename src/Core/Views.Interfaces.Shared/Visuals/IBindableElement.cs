using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.DependencyProperties;

namespace Das.Views.DataBinding
{
    public interface IBindableElement : IVisualElement,
                                        IBindable
    {
        void SetValue<T>(IDependencyProperty property,
                         T value);

        Object? ReadLocalValue(IDependencyProperty dp);

        void ClearValue(IDependencyProperty dp);

        T GetValue<T>(IDependencyProperty<T> dp);

        Object? GetValue(IDependencyProperty dp);

        IEnumerable<LocalValueEntry> GetLocalValues();
    }
}
