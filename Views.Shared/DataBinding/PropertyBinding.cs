using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Das.Views.DataBinding
{
    // ReSharper disable once UnusedType.Global
    public class PropertyBinding<T> : BaseBinding<T>
    {
        public PropertyBinding(IDataContext? dataContext,
                               PropertyInfo prop)
        {
            _dataContext = dataContext;
            _prop = prop;
        }

        public PropertyBinding(IDataContext dataContext,
                               String propertyName)
        {
            _dataContext = dataContext;
            _prop = dataContext.GetType().GetProperty(propertyName) ??
                    throw new MissingMemberException(dataContext.GetType().Name, propertyName);
        }

        public override IDataBinding<T> DeepCopy()
        {
            return new PropertyBinding<T>(null, _prop);
        }

        public override T GetValue(Object? dataContext)
        {
            if (_dataContext is {} dc &&
                _prop.GetValue(dc.Value, null) is T prop)
                return prop;
            return default!;
        }

        private readonly IDataContext? _dataContext;
        private readonly PropertyInfo _prop;

        public override void Dispose()
        {
            
        }
    }
}