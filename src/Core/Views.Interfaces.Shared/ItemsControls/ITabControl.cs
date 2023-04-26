using System;
using Das.Views.Mvvm;

namespace Das.Views.ItemsControls;

public interface ITabControl : IItemsControl,
                               IContentPresenter
                                   
{
   INotifyingCollection TabItems { get; }


   IVisualElement? SelectedTab { get; }
}