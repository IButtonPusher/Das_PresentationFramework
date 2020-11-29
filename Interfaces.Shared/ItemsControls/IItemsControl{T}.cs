using Das.Views.Panels;
using System;
using System.Collections.Generic;
using System.Text;
using Das.Views.Mvvm;

namespace Das.Views
{
    public interface IItemsControl<out T> : IItemsControl
    {
        new INotifyingCollection<T>? ItemsSource { get; }
    }
}
