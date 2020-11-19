using System;
using System.Threading.Tasks;
using Das.Views.DataBinding;
using Das.Views.Styles;

namespace Das.Views.Panels
{
    // ReSharper disable once UnusedTypeParameter
    public interface IView<T> : IView,
                                IBindingSetter<T>
    {
    }

    public interface IView : IContentContainer,
                             IBindingSetter
    {
        IStyleContext StyleContext { get; }
    }
}