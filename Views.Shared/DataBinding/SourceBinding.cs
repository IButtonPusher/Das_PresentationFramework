using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using Das.ViewModels;
using Das.Views.Mvvm;


namespace Das.Views.DataBinding
{
    /// <summary>
    ///     One way binding vm->ui
    /// </summary>
    public class SourceBinding : BaseBinding,
                                 IVisualPropertySetter
    {
        public IValueConverter? ValueConverter { get; }

        public SourceBinding(Object source,
                             String sourceProperty,
                             IBindableElement target,
                             String targetProperty,
                             IValueConverter? valueConverter)
        : this(source, 
            GetObjectPropertyOrDie(source, sourceProperty),
            target,
            GetObjectPropertyOrDie(target, targetProperty), valueConverter) { }

        public SourceBinding(Object? source,
                             PropertyInfo srcProp,
                             IBindableElement target,
                             PropertyInfo targetProp,
                             IValueConverter? valueConverter)
        {
            ValueConverter = valueConverter;
            _source = source;
            _srcProp = srcProp;
            _sourcePropertyName = srcProp.Name;
            _target = target;
            _targetProp = targetProp;
            _targetPropertyName = targetProp.Name;

            _sourceGetter = srcProp.GetGetMethod()
                            ?? throw new MissingMethodException(_sourcePropertyName);

            

            _targetSetter = targetProp.GetSetMethod();

            _targetGetter = targetProp.GetGetMethod()
                            ?? throw new MissingMethodException(_targetPropertyName);
            
            AddChangeListenersToSource(source);

            if (GetType() == typeof(SourceBinding) && 
                typeof(INotifyCollectionChanged).IsAssignableFrom(srcProp.PropertyType))
            {
                var srcValue = GetSourceValue();
                if (srcValue != null)
                    throw new InvalidOperationException(
                        $"Cannot use {GetType().FullName} for collections.  Use {nameof(OneWayCollectionBinding)}, etc");
            }
        }

        private void AddChangeListenersToSource(Object? source)
        {
            switch (source)
            {
                case IViewModel vm:
                    vm.PropertyValueChanged += OnSourcePropertyValueChanged;
                    break;
                case INotifyPropertyChanged notifier:
                    notifier.PropertyChanged += OnSourcePropertyChanged;
                    break;
            }
        }
        
        
        private void RemoveChangeListenersFromSource(Object? source)
        {
            switch (source)
            {
                case IViewModel vm:
                    vm.PropertyValueChanged -= OnSourcePropertyValueChanged;
                    break;
                case INotifyPropertyChanged notifier:
                    notifier.PropertyChanged -= OnSourcePropertyChanged;
                    break;
            }
        }


        public override void Dispose()
        {
            RemoveChangeListenersFromSource(_source);
        }

        public override Object Clone()
        {
            return new SourceBinding(_source, _srcProp, _target, _targetProp,
                ValueConverter);
        }

        public override Object? GetBoundValue(Object? dataContext)
            => GetSourceValue();

        public override void Evaluate()
        {
            var value = GetSourceValue();


            SetTargetValue(value);
        }

        protected void SetTargetValue(Object? value)
        {
            if (_targetSetter == null)
                return;

            var targetType = _targetGetter.ReturnType;
            
            if (value is { } valueValue &&
                // ReSharper disable once UseMethodIsInstanceOfType
                !targetType.IsAssignableFrom(valueValue.GetType()))

            {
                //not trivial to set the target value to the source value
                
                var valType = valueValue.GetType();
                
                if (valueValue is IEnumerable valCollection)
                {
                    if (targetType.IsInterface && targetType.IsGenericType &&
                        targetType.GetGenericTypeDefinition() == typeof(INotifyingCollection<>))
                    {
                        //treat the target property type like a concrete version of a notifying collection

                        targetType = typeof(AsyncObservableCollection2<>).MakeGenericType(
                            targetType.GetGenericArguments()[0]);
                    }

                    foreach (var targetCtor in targetType.GetConstructors())
                    {
                        var ctorParams = targetCtor.GetParameters();
                        if (ctorParams.Length != 1)
                            continue;

                        var cp = ctorParams[0];
                        if (cp.ParameterType.IsAssignableFrom(valType))
                        {
                            var newObj = targetCtor.Invoke(new Object[] {valCollection});
                            _targetSetter?.Invoke(_target, new[] {newObj});
                            return;
                        }
                        
                        
                    }
                }
            }

            if (targetType == typeof(String) && value is { } validValue && validValue.GetType() != typeof(String))
                value = validValue.ToString();
            
            _targetSetter?.Invoke(_target, new[] {value});
        }

        protected Object? GetTargetValue() => _targetGetter.Invoke(_target, EmptyObjectArray);

        protected TValue GetTargetValue<TValue>()
        {
            var val = GetTargetValue();
            switch (val)
            {
                case TValue good:
                    return good;

                default:
                    throw new InvalidCastException(typeof(TValue).Name);
            }
        }

        protected Object? GetSourceValue()
        {
            return GetSourceValue(_source);
            
            //var val = _sourceGetter.Invoke(_source, EmptyObjectArray);
            //if (!(ValueConverter is { } converter))
            //    return val;

            //return converter.Convert(val);
        }
        
        public Object? GetSourceValue(Object? dataContext)
        {
            var val = _sourceGetter.Invoke(dataContext, EmptyObjectArray);
            if (!(ValueConverter is { } converter))
                return val;

            return converter.Convert(val);
        }

        public override void UpdateDataContext(Object? dataContext)
        {
            base.UpdateDataContext(dataContext);

            if (ReferenceEquals(dataContext, _source))
                return;

            RemoveChangeListenersFromSource(_source);

            _source = dataContext;
            AddChangeListenersToSource(_source);
            
            if (_source is {})
                Evaluate();
        }


       

        private void OnSourcePropertyChanged(Object sender,
                                             PropertyChangedEventArgs e)
        {
            if (e.PropertyName != _sourcePropertyName)
                return;

            Evaluate();
        }

        private void OnSourcePropertyValueChanged(String propertyName, 
                                                  Object? newValue)
        {
            if (propertyName != _sourcePropertyName)
                return;

            SetTargetValue(newValue);
        }

        //protected INotifyPropertyChanged? _source;
        protected Object? _source;
        private readonly PropertyInfo _srcProp;
        protected readonly IBindableElement _target;
        private readonly PropertyInfo _targetProp;

        private readonly MethodInfo _sourceGetter;
        private readonly MethodInfo _targetGetter;
        private readonly MethodInfo? _targetSetter;


        //private INotifyCollectionChanged? _notifyingCollection;
        //private readonly Boolean _isCollectionBinding;
        
        private readonly String _sourcePropertyName;
        protected readonly String _targetPropertyName;
        
        //private readonly PropertyInfo _targetProperty;


        public String PropertyName => _targetPropertyName;

        
    }
}