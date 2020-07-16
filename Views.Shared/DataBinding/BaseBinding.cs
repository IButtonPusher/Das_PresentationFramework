using System;
using System.Threading.Tasks;
#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#endif

namespace Das.Views.DataBinding
{
    public abstract class BaseBinding<T> : BaseBinding, IDataBinding<T>
    {
        public abstract T GetValue(Object dataContext);

        public virtual Task<T> GetValueAsync(Object dataContext)
        {
            var val = GetValue(dataContext);
            return TaskEx.FromResult(val);
        }

        public override Object? GetBoundValue(Object dataContext)
        {
            return GetValue(dataContext);
        }

        public abstract IDataBinding<T> DeepCopy();
    }

    public abstract class BaseBinding : IDataBinding
    {
        public Object DataContext { get; set; }

        public abstract Object? GetBoundValue(Object dataContext);

        public virtual IDataBinding ToSingleBinding()
        {
            throw new NotImplementedException();
        }
    }
}