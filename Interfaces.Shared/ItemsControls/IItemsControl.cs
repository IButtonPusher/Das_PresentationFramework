using System;
using Das.Views.Mvvm;
using Das.Views.Panels;

namespace Das.Views
{
    public interface IItemsControl : IContainerVisual
    {
        INotifyingCollection? ItemsSource { get; }

        IDataTemplate? ItemTemplate { get; }
    }
}
