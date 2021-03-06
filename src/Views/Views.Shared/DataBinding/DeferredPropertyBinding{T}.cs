﻿using System;
using System.Reflection;

namespace Das.Views.DataBinding
{
    public class DeferredPropertyBinding<T> : BaseBinding<T>
    {
        // ReSharper disable once UnusedMember.Global
        public DeferredPropertyBinding(Type parentClassType,
                                       String propertyName)
        {
            _prop = GetTypePropertyOrDie(parentClassType, propertyName);
                
                //parentClassType.GetProperty(propertyName) ??
                //    throw new MissingMemberException(parentClassType.Name, propertyName);
        }

        public DeferredPropertyBinding(PropertyInfo prop)
        {
            _prop = prop;
        }


        public override IDataBinding<T> DeepCopy()
        {
            return new DeferredPropertyBinding<T>(_prop);
        }

        public override T GetValue(Object? dataContext)
        {
            if (dataContext != null && _prop.GetValue(dataContext, null) is T prop)
                return prop;
            return default!;
        }

        //public override IDataBinding ToSingleBinding()
        //{
        //    var argh = _prop.PropertyType.GetGenericArguments().First();
        //    var itemType = typeof(InstanceBinding<>).MakeGenericType(argh);
        //    return (IDataBinding) Activator.CreateInstance(itemType, null);
        //}

        private readonly PropertyInfo _prop;

        public override Object Clone()
        {
            return new DeferredPropertyBinding<T>(_prop);
        }
    }
}
