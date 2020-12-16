using System;
using System.Threading.Tasks;
using Das.Views.DataBinding;
using Das.Views.Styles;

namespace Das.Views.Panels
{
    // ReSharper disable once UnusedTypeParameter
    public interface IView : IContentContainer,
        IBindableElement
        //,IBindingSetter<T>
    {
        IStyleContext StyleContext { get; }
    }

    //public interface IView : IContentContainer,
    //                         IBindableElement
    //                         //,IBindingSetter
    //{
    //    IStyleContext StyleContext { get; }
    //}
}