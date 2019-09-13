using System;

namespace Das.Views.DataBinding
{
    public abstract class BaseBinding<T> : BaseBinding, IDataBinding<T>
    {
        public abstract T GetValue(object dataContext);


        public T GetLastValue() => GetValue(DataContext);

        public override object GetBoundValue(Object dataContext)
            => GetValue(dataContext);

        public abstract IDataBinding<T> DeepCopy();
    }

    public abstract class BaseBinding : IDataBinding
    {
        public object DataContext { get; set; }

        public abstract object GetBoundValue(object dataContext);

        public virtual IDataBinding ToSingleBinding() => throw new NotImplementedException();
    }
}