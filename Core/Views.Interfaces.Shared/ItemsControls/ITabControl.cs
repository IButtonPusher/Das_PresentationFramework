using System;
using Das.Views.DataBinding;
using Das.Views.Mvvm;
using Das.Views.Rendering;

namespace Das.Views.ItemsControls
{

    public interface ITabControl : IItemsControl,
                                   IContentPresenter
                                   
    {
        INotifyingCollection TabItems { get; }


        IVisualElement? SelectedTab { get; }
    }
}
