﻿using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Das.Views.DataBinding
{
    public class DeferredPropertyBinding<T> : BaseBinding<T>
    {
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

        public override IDataBinding<T> DeepCopy()
        {
            return new DeferredPropertyBinding<T>(_parentClassType, _prop.Name);
        }

        public override T GetValue(Object dataContext)
        {
            if (_prop.GetValue(dataContext, null) is T prop)
                return prop;
            return default!;
        }

        public override IDataBinding ToSingleBinding()
        {
            var argh = _prop.PropertyType.GetGenericArguments().First();
            var itemType = typeof(InstanceBinding<>).MakeGenericType(argh);
            return (IDataBinding) Activator.CreateInstance(itemType, null);
        }

        private readonly Type _parentClassType;
        private readonly PropertyInfo _prop;
    }
}