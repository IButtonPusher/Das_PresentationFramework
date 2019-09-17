using System;

namespace Das.Views.DataBinding
{
    public class InstanceBinding<T> : BaseBinding<T>
    {
        public override T GetValue(Object dataContext)
            => dataContext == null ? default : (T) dataContext;

        public override IDataBinding<T> DeepCopy() => new InstanceBinding<T>();
    }
}