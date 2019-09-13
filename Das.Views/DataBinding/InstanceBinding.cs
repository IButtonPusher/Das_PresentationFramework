using System;

namespace Das.Views.DataBinding
{
    public class InstanceBinding<T> : BaseBinding<T>
    {
        public override T GetValue(object dataContext)
            => dataContext == null ? default : (T) dataContext;

        public override IDataBinding<T> DeepCopy() => new InstanceBinding<T>();
    }
}