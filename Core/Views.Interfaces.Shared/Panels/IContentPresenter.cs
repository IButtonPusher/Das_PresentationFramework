using System;
using Das.Views.Rendering;

namespace Das.Views
{
    public interface IContentPresenter : IVisualElement
    {
        IDataTemplate? ContentTemplate { get; set; }
    }
}
