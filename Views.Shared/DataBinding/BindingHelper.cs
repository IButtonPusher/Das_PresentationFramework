using System;

namespace Das.Views.DataBinding
{
    public class BindingHelper : BindingHelperBase, 
                                 IBindable
    {
        private readonly Func<Object?, Object?, Object?>? _interceptDataContextChange;
        private readonly Action<Object?>? _onDataContextChanged;

        public BindingHelper(IVisualElement visual,
                             Func<Object?, Object?, Object?>? interceptDataContextChange,
                             Action<Object?>? onDataContextChanged)
        : base(visual)
        {
            _interceptDataContextChange = interceptDataContextChange;
            _onDataContextChanged = onDataContextChanged;
        }
      
        private Object? _dataContext;

        public Object? DataContext
        {
            get => _dataContext;
            set => SetValue(ref _dataContext, value,
                OnInterceptDataContextChanging, OnDataContextChanged);
        }
        
        protected Object? OnInterceptDataContextChanging(Object? oldValue, 
                                                                  Object? newValue)
        {
            if (_interceptDataContextChange is { } valid)
            {
                var nowIWant = valid(oldValue, newValue);
                if (!Equals(nowIWant, newValue))
                    return nowIWant;
            }
            
            if (newValue is { } something)
            {
                lock (_lockBindings)
                {
                    foreach (var binding in _bindings)
                    {
                        if (binding is IVisualPropertySetter 
                            {PropertyName: nameof(DataContext)} setter)
                        {
                            var res = setter.GetSourceValue(something);
                            //  if (res != null)
                            return res;
                        }
                    }
                }
            }
            
            return newValue;
        }

        protected void OnDataContextChanged(Object? newValue)
        {
            RefreshBoundValues(newValue);

            if (_onDataContextChanged is { } dcc)
                dcc(newValue);
            
            _visual.InvalidateMeasure();
        }
        
       
    }
}
