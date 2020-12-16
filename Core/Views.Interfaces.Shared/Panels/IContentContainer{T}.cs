using System;
using Das.Views.DataBinding;

namespace Das.Views.Panels
{
    public interface IContentContainer<TDataContext> : IBindableElement
    {
        IVisualElement? Content { get; set; }
    }
}
