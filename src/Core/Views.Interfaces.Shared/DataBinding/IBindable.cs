using System;
using System.Collections.Generic;

namespace Das.Views.DataBinding
{
    public interface IBindable : IDisposable
    {
        void AddBinding(IDataBinding binding);

        IEnumerable<IDataBinding> GetBindings();

        Object? DataContext { get; set; }
    }
}
