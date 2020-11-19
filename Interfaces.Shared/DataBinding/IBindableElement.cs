using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.Views.Rendering;

namespace Das.Views.DataBinding
{
    public interface IBindableElement : IVisualElement,
                                        IDataContext,
                                        IBindingSetter,
                                        INotifyPropertyChanged
    {
        void AddBinding(IDataBinding binding);


        IDataBinding? Binding { get; set; }

        Object? DataContext { get; set; }

        Object? GetBoundValue(Object dataContext);
    }

    public interface IBindableElement<T> : IBindableElement
    {
        new IDataBinding<T>? Binding { get; set; }

        new T GetBoundValue(Object dataContext);

        void SetBoundValue(T value);

        Task SetBoundValueAsync(T value);
    }
}

//using System;
//using System.Threading.Tasks;
//using Das.Views.Rendering;

//namespace Das.Views.DataBinding
//{
//    public interface IBindableElement : IVisualElement, IDataContext, IBindingSetter
//    {
//        IDataBinding? Binding { get; set; }

//        Object? DataContext { get; set; }

//        Object? GetBoundValue(Object dataContext);
//    }

//    public interface IBindableElement<T> : IVisualElement, IDataContext, IBindingSetter
//    {
//        IDataBinding<T>? Binding { get; set; }

//        T GetBoundValue(Object dataContext);

//        void SetBoundValue(T value);

//        Task SetBoundValueAsync(T value);
//    }
//}