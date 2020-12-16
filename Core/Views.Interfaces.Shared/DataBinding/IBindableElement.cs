using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Das.Views.DataBinding
{
    public interface IBindableElement : IVisualElement,
                                        //IDataContext,
                                        IBindable
                                        //IBindingSetter,
    {

        //Boolean TryGetDataContextBinding(out IDataBinding dcBinding);

        //void AddBinding(IDataBinding binding);

        //IEnumerable<IDataBinding> GetBindings();


        ////IDataBinding? Binding { get; set; }

        //Object? DataContext { get; set; }

        ////Object? GetBoundValue(Object dataContext);
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