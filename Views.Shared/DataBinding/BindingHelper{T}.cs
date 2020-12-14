using System;

namespace Das.Views.DataBinding
{
    public class BindingHelper<T> : BindingHelperBase,
                                    IBindable<T>
    {
        private readonly Func<T, T, T>? _interceptDataContextChange;
        private readonly Action<T>? _onDataContextChanged;

        public BindingHelper(IVisualElement visual,
                             Func<T, T, T>? interceptDataContextChange, 
                             Action<T>? onDataContextChanged) 
            : base(visual)
        {
            _interceptDataContextChange = interceptDataContextChange;
            _onDataContextChanged = onDataContextChanged;
            _dataContext = default!;
        }
        
        private T _dataContext;
        
        public virtual T DataContext
        {
            get => _dataContext;
            set => SetValue(ref _dataContext, value,
                OnInterceptDataContextChanging, OnDataContextChanged);
        }

        public Boolean TryGetDataContextBinding(out IDataBinding dcBinding)
        {
            lock (_lockBindings)
            {
                foreach (var binding in _bindings)
                {
                    if (binding.IsDataContextBinding)
                    {
                        dcBinding = binding;
                        return true;
                    }
                }
            }

            dcBinding = default!;
            return false;
        }

        protected virtual T OnInterceptDataContextChanging(T oldValue, 
                                                           T newValue)
        {
            if (_interceptDataContextChange is { } interceptor)
            {
                var nowIWant = interceptor(oldValue, newValue);
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
                            if (res is T valid)
                                return valid;
                            //  if (res != null)
                            //return res;
                        }
                    }
                }
            }
            
            return newValue;
        }
        
        protected virtual void OnDataContextChanged(T newValue)
        {
            RefreshBoundValues(newValue);
            
            if (_onDataContextChanged is { } dcc)
                dcc(newValue);
            
            _visual.InvalidateMeasure();
        }

        //Object? IBindable.DataContext
        //{
        //    get => DataContext;
        //    set
        //    {
        //        switch (value)
        //        {
        //            case null:
        //                DataContext = default!;
        //                break;
                    
        //            case T valid:
        //                DataContext = valid;
        //                break;
                    
        //            default:
        //                throw new InvalidCastException();
                        
        //        }
        //    }
        //}
    }
}
