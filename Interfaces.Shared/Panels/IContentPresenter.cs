using System;

namespace Das.Views
{
    public interface IContentPresenter
    {
        IDataTemplate? ContentTemplate { get; }
    }
}
