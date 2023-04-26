using System;

namespace Das.Views;

public interface IContentPresenter : IVisualElement
{
   IDataTemplate? ContentTemplate { get; set; }
}