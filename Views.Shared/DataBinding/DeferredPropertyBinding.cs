using System;
using System.Linq;
using System.Reflection;

namespace Das.Views.DataBinding
{
    public class DeferredPropertyBinding<T> : BaseBinding<T>
    {
        private readonly PropertyInfo _prop;
        private readonly Type _parentClassType;

        public DeferredPropertyBinding(Type parentClassType, String propertyName)
        {
            _parentClassType = parentClassType;
            _prop = parentClassType.GetProperty(propertyName);
        }

        public DeferredPropertyBinding(PropertyInfo prop)
        {
            _prop = prop;
            _parentClassType = _prop.DeclaringType;
        }

        public override T GetValue(Object dataContext)
        {
            if (_prop.GetValue(dataContext, null) is T prop)
                return prop;
            return default;
        }

        public override IDataBinding<T> DeepCopy() =>
            new DeferredPropertyBinding<T>(_parentClassType, _prop.Name);

        public override IDataBinding ToSingleBinding()
        {
            var argh = _prop.PropertyType.GetGenericArguments().First();
            var itemType = typeof(InstanceBinding<>).MakeGenericType(argh);
            return (IDataBinding) Activator.CreateInstance(itemType, null);
        }
    }
}