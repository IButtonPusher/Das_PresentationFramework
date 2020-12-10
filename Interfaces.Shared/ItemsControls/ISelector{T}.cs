using Das.Views.DataBinding;
using System;

namespace Das.Views
{
    public interface ISelector<T> : IBindableElement<T>
        where T : IEquatable<T>
    {
        T SelectedItem { get; set; }

        IBindableElement<T>? SelectedVisual { get; set; }
    }
}
