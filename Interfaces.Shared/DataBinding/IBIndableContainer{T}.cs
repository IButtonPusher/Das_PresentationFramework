using System;

namespace Das.Views.DataBinding
{
    public interface IBindableContainer<T> : IBindableElement<T>
    {
        void UpdateContentDataContext(T newValue);
    }
}
