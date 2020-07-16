using System;
using System.Threading.Tasks;

namespace Das.Views.DataBinding
{
    public class ObjectBinding<T> : BaseBinding<T>
    {
        public ObjectBinding(T bindingObject)
        {
            _bindingObject = bindingObject;
        }

        public override IDataBinding<T> DeepCopy()
        {
            return new InstanceBinding<T>();
        }

        public override T GetValue(Object dataContext)
        {
            return _bindingObject;
        }

        private readonly T _bindingObject;
    }
}