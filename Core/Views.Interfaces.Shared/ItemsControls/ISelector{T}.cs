using Das.Views.DataBinding;
using System;

namespace Das.Views
{
    public interface ISelector<TDataContext, TItems> : IBindableElement<TDataContext>,
                                                       ISelector<TItems>
        where TItems : IEquatable<TItems>
    {
        
    }

    public interface ISelector<TItems> : IVisualElement
        where TItems : IEquatable<TItems>
    {
        TItems SelectedItem { get; set; }

        IBindableElement<TItems>? SelectedVisual { get; set; }
    }
}
