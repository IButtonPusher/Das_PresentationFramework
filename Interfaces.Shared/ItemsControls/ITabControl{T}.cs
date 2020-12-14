using System;
using Das.Views.DataBinding;
using Das.Views.Mvvm;

namespace Das.Views.ItemsControls
{
    public interface ITabControl<TDataContext, TItems> : ITabControl<TDataContext>
                                                         
    {
        new IAsyncObservableCollection<IBindableElement<TItems>> TabItems { get; }

        new IBindableElement<TItems>? SelectedTab { get; }
    }

    public interface ITabControl<TDataContext> : IBindableElement<TDataContext>,
                                                 ITabControl
    {
        //new IAsyncObservableCollection<IBindableElement<TItems>> TabItems { get; }

        //new IBindableElement<TItems>? SelectedTab { get; }
    }


}
