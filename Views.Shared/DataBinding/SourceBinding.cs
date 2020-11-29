using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using Das.Views.Mvvm;


namespace Das.Views.DataBinding
{
    /// <summary>
    ///     One way binding vm->ui
    /// </summary>
    public class SourceBinding : BaseBinding
    {
        public SourceBinding(INotifyPropertyChanged source,
                             String sourceProperty,
                             IBindableElement target,
                             String targetProperty)
        : this(source, 
            GetObjectPropertyOrDie(source, sourceProperty),
            target,
            GetObjectPropertyOrDie(target, targetProperty)) { }

        public SourceBinding(INotifyPropertyChanged source,
                             PropertyInfo srcProp,
                             IBindableElement target,
                             PropertyInfo targetProp)
        {
            _source = source;
            _sourcePropertyName = srcProp.Name;
            _target = target;
            _targetPropertyName = targetProp.Name;

            _sourceGetter = srcProp.GetGetMethod()
                            ?? throw new MissingMethodException(_sourcePropertyName);

            

            _targetSetter = targetProp.GetSetMethod();

            _targetGetter = targetProp.GetGetMethod()
                            ?? throw new MissingMethodException(_targetPropertyName);

            if (source is IViewModel vm)
                vm.PropertyValueChanged += OnSourcePropertyValueChanged;
            else
                source.PropertyChanged += OnSourcePropertyChanged;

            if (GetType() == typeof(SourceBinding) && 
                typeof(INotifyCollectionChanged).IsAssignableFrom(srcProp.PropertyType))
            {
                var srcValue = GetSourceValue();
                if (srcValue != null)
                    throw new InvalidOperationException(
                        $"Cannot use {GetType().FullName} for collections.  Use {nameof(OneWayCollectionBinding)}, etc");
            }
        }

       

        public override void Dispose()
        {
            if (_source is {} source)
                source.PropertyChanged -= OnSourcePropertyChanged;

            //if (_notifyingCollection is { } valid)
            //    valid.CollectionChanged -= OnSourceCollectionChanged;
        }

        public override Object? GetBoundValue(Object? dataContext)
            => GetSourceValue();

        public override void Evaluate()
        {
            var value = GetSourceValue();

            //switch (value)
            //{
            //    case INotifyCollectionChanged collection:
            //        if (_notifyingCollection != null)
            //        {
            //            _notifyingCollection.CollectionChanged -= OnSourceCollectionChanged;
            //            if (_notifyingCollection is IEnumerable itar)
            //                OnRemoveItarItems(itar);
            //        }

            //        collection.CollectionChanged += OnSourceCollectionChanged;
            //        _notifyingCollection = collection;
            //        if (_notifyingCollection is IEnumerable neuItar)
            //            OnAddItarItems(neuItar);
            //        break;

            //    case null:
            //        return; //todo: ?

                
            //}

            SetTargetValue(value);
        }

        protected void SetTargetValue(Object? value)
        {
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

        protected virtual Object? GetSourceValue() => _sourceGetter.Invoke(_source, EmptyObjectArray);

        public override void UpdateDataContext(Object? dataContext)
        {
            base.UpdateDataContext(dataContext);

            if (ReferenceEquals(dataContext, _source))
                return;

            if (_source is { } valid)
            {
                valid.PropertyChanged -= OnSourcePropertyChanged;
            }

            switch (dataContext)
            {
                case null:
                    _source = null;
                    break;

                case INotifyPropertyChanged source:
                    source.PropertyChanged += OnSourcePropertyChanged;
                    Evaluate();
                    break;

                default:
                    throw new NotSupportedException();
            }

        }


        //private void OnSourceCollectionChanged(Object sender,
        //                                       NotifyCollectionChangedEventArgs e)
        //{
        //    e.HandleCollectionChanges<Object>(OnRemoveItems, OnAddItems);
        //}

        //private void OnAddItems(IEnumerable<Object> objs)
        //{
        //    OnAddItarItems(objs);
        //}

        //private void OnAddItarItems(IEnumerable objs)
        //{
        //    var targetVal = GetTargetValue() as IList
        //                    ?? throw new NullReferenceException(_targetPropertyName);

        //    foreach (var obj in objs)
        //    {
        //        targetVal.Add(obj);
        //    }
        //}

        //private void OnRemoveItarItems(IEnumerable objs)
        //{
        //    var targetVal = GetTargetValue() as IList
        //                    ?? throw new NullReferenceException(_targetPropertyName);

        //    foreach (var obj in objs)
        //    {
        //        targetVal.Remove(obj);
        //    }
        //}

        //private void OnRemoveItems(IEnumerable<Object> obj)
        //{
        //    OnRemoveItarItems(obj);
        //}

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

        protected INotifyPropertyChanged? _source;
        protected readonly IBindableElement _target;

        private readonly MethodInfo _sourceGetter;
        private readonly MethodInfo _targetGetter;
        private readonly MethodInfo? _targetSetter;


        //private INotifyCollectionChanged? _notifyingCollection;
        //private readonly Boolean _isCollectionBinding;
        
        private readonly String _sourcePropertyName;
        protected readonly String _targetPropertyName;
        
        //private readonly PropertyInfo _targetProperty;
        
        
    }
}