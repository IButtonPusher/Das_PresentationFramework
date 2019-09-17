using System;

namespace Das.Views.DataBinding
{
    public class ObjectBinding<T> : BaseBinding<T>
    {
        private readonly T _bindingObject;

        public ObjectBinding(T bindingObject)
        {
            _bindingObject = bindingObject;
        }

        public override T GetValue(Object dataContext) => _bindingObject;
        public override IDataBinding<T> DeepCopy() => new InstanceBinding<T>();
    }
}