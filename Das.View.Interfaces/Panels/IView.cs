using Das.Views.DataBinding;
using Das.Views.Styles;

namespace Das.Views.Panels
{
    // ReSharper disable once UnusedTypeParameter
    public interface IView<T> : IView
    {
    }

    public interface IView : IContentContainer, IBindingSetter
    {
        IStyleContext StyleContext { get; }
    }
}