using System;

namespace Das.Views.DataBinding
{
    public interface IBindingSetter
    {
        void SetBoundValue(Object value);

        void SetDataContext(Object dataContext);
    }
}