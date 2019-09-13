using System.Collections.Generic;
using Das.Views.DataBinding;

namespace Das.Views.Panels
{
    public interface IRepeaterPanel<T> : IRepeaterPanel
    {
        new IBindableElement<IEnumerable<T>> Content { get; }
    }

    public interface IRepeaterPanel : ISequentialPanel, IContentContainer
    {
    }
}