using System;
using Das.Views.DataBinding;
using Das.Views.Mvvm;

namespace Das.Views.ItemsControls
{
   public interface ITabControl<T> : IItemsControl<T>,
                                      ITabControl
    {
        new IAsyncObservableCollection<IBindableElement<T>> TabItems { get; }

        new IBindableElement<T>? SelectedTab { get; }
    }
}
