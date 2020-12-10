using System;
using Das.Views.Mvvm;

namespace Das.Views
{
    public interface IItemsControl<out T> : IItemsControl
    {
        new INotifyingCollection<T>? ItemsSource { get; }
    }
}
