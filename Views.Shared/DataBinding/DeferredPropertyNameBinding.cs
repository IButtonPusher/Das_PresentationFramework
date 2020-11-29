using System;

namespace Das.Views.DataBinding
{
    public class DeferredPropertyNameBinding<T> : BaseBinding<T>
    {
        private readonly String _propertyName;

        public DeferredPropertyNameBinding(String propertyName)
        {
            _propertyName = propertyName;
        }

        public override T GetValue(Object? dataContext)
        {
            throw new NotImplementedException();
        }

        public override IDataBinding<T> DeepCopy()
        {
            throw new NotImplementedException();
        }

        public override String ToString()
        {
            return _propertyName;
        }

    }
}
