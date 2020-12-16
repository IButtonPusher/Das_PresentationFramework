using System;
using System.Collections.Generic;

namespace Das.Views.DataBinding
{
    public interface IBindable<TDataContext> : IBindable
    {
        TDataContext DataContext { get; set; }
        
        
        
        //new event Action<T> DataContextChanged;
        
        //new event Func<T, T, T> InterceptDataAContextChange;
    }
}
