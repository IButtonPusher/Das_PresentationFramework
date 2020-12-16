using System;
using Das.Views.Mvvm;
using Das.Views.Templates;

namespace Das.Views
{
    public interface IItemsControl<T> : IItemsControl
    {
        new INotifyingCollection<T>? ItemsSource { get; }
        
        new IDataTemplate<T>? ItemTemplate { get; }
    }
}
