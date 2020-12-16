using System;
using System.Collections.Generic;

namespace Das.Views.DataBinding
{
    public interface IBindable : IDisposable
    {
        ///// <summary>
        ///// If the DataContext of this element has a binding (rather than inheriting from the parent)
        ///// return it, so we can avoid improperly setting the DataContext with the inherited value
        ///// </summary>
        //Boolean TryGetDataContextBinding(out IDataBinding dcBinding);
        
        void AddBinding(IDataBinding binding);

        IEnumerable<IDataBinding> GetBindings();
        
        //void AddBinding(IDataBinding binding);

        //IEnumerable<IDataBinding> GetBindings();

        Object? DataContext { get; set; }

        //event Func<Object?, Object?, Object?> InterceptDataAContextChange;
        
        //event Action<Object?> DataContextChanged;
    }
}
