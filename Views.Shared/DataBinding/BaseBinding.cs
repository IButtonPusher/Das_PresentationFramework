using System;

namespace Das.Views.DataBinding
{
    public abstract class BaseBinding<T> : BaseBinding, IDataBinding<T>
    {
        public abstract T GetValue(Object dataContext);


//        public T GetLastValue() => GetValue(DataContext);

        public override Object GetBoundValue(Object dataContext)
            => GetValue(dataContext);

        public abstract IDataBinding<T> DeepCopy();
    }

    public abstract class BaseBinding : IDataBinding
    {
        public Object DataContext { get; set; }

        public abstract Object GetBoundValue(Object dataContext);

        public virtual IDataBinding ToSingleBinding() => throw new NotImplementedException();
    }
}