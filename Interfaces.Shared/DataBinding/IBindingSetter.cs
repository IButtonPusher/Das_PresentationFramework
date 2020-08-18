using System;
using System.Threading.Tasks;

namespace Das.Views.DataBinding
{
    public interface IBindingSetter<TDataContext>
    {
        void SetDataContext(TDataContext dataContext);
    }

    public interface IBindingSetter
    {
        void SetBoundValue(Object? value);

        Task SetBoundValueAsync(Object? value);

        void SetDataContext(Object? dataContext);

        Task SetDataContextAsync(Object dataContext);
    }
}