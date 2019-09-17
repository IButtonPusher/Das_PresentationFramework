using System;
using System.Reflection;

namespace Das.Views.DataBinding
{
    public class PropertyBinding<T> : BaseBinding<T>
    {
        private readonly IDataContext _dataContext;
        private readonly PropertyInfo _prop;

        public PropertyBinding(IDataContext dataContext, PropertyInfo prop)
        {
            _dataContext = dataContext;
            _prop = prop;
        }

        public PropertyBinding(IDataContext dataContext, String propertyName)
        {
            _dataContext = dataContext;
            _prop = dataContext.GetType().GetProperty(propertyName);
        }

        public override T GetValue(Object dataContext)
        {
            if (_prop.GetValue(_dataContext.Value, null) is T prop)
                return prop;
            return default;
        }

        public override IDataBinding<T> DeepCopy() => new PropertyBinding<T>(null, _prop);
    }
}