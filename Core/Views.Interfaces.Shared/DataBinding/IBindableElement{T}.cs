using System;
using System.ComponentModel;

namespace Das.Views.DataBinding
{
     public interface IBindableElement<TDataContext> : IBindable<TDataContext>,
                                                       IVisualElement,
                                                       IEquatable<IBindableElement<TDataContext>>
    {
        
        
        //new T DataContext { get; set; }

        //new IDataBinding<T>? Binding { get; set; }

        //new T GetBoundValue(Object dataContext);

        //void SetBoundValue(T value);

        //Task SetBoundValueAsync(T value);
    }
}
