using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Das.Views.DataBinding
{
    public class PropertyBinding<T> : BaseBinding<T>
    {
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

        public override IDataBinding<T> DeepCopy()
        {
            return new PropertyBinding<T>(null, _prop);
        }

        public override T GetValue(Object dataContext)
        {
            if (_prop.GetValue(_dataContext.Value, null) is T prop)
                return prop;
            return default;
        }

        private readonly IDataContext _dataContext;
        private readonly PropertyInfo _prop;
    }
}