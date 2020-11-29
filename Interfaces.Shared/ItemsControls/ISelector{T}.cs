using Das.Views.DataBinding;
using System;
using System.Collections.Generic;
using System.Text;
using Das.Views.Mvvm;
using Das.Views.Rendering;

namespace Das.Views
{
    public interface ISelector<T> : IBindableElement<T>
        where T : IEquatable<T>
    {
        T SelectedItem { get; set; }

        IBindableElement<T>? SelectedVisual { get; set; }
    }
}
