using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;


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
        {
            _source = source;
            _sourcePropertyName = sourceProperty;
            _target = target;
            _targetPropertyName = targetProperty;

            var srcProp = source.GetType().GetProperty(sourceProperty)
                          ?? throw new MissingMethodException(sourceProperty);

            _sourceGetter = srcProp.GetGetMethod()
                            ?? throw new MissingMethodException(sourceProperty);

            var targetProp = target.GetType().GetProperty(targetProperty)
                              ?? throw new MissingMethodException(targetProperty);

            _targetSetter = targetProp.GetSetMethod();

            _targetGetter = targetProp.GetGetMethod()
                            ?? throw new MissingMethodException(targetProperty);

            SetTargetFromSource();

            source.PropertyChanged += OnSourcePropertyChanged;
        }

        public override void Dispose()
        {
            if (_source is {} source)
                source.PropertyChanged -= OnSourcePropertyChanged;

            if (_notifyingCollection is { } valid)
                valid.CollectionChanged -= OnSourceCollectionChanged;
        }

        public override Object? GetBoundValue(Object? dataContext)
        {
            return _sourceGetter.Invoke(_source, EmptyObjectArray);
        }

        private void SetTargetFromSource()
        {
            var value = _sourceGetter.Invoke(_source, EmptyObjectArray);

            switch (value)
            {
                case INotifyCollectionChanged collection:
                    if (_notifyingCollection != null)
                    {
                        _notifyingCollection.CollectionChanged -= OnSourceCollectionChanged;
                        if (_notifyingCollection is IEnumerable itar)
                            OnRemoveItarItems(itar);
                    }

                    collection.CollectionChanged += OnSourceCollectionChanged;
                    _notifyingCollection = collection;
                    if (_notifyingCollection is IEnumerable neuItar)
                        OnAddItarItems(neuItar);
                    break;

                case null:
                    return; //todo: ?

                
            }

            _targetSetter?.Invoke(_target, new[] {value});
        }

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
                    SetTargetFromSource();
                    break;

                default:
                    throw new NotSupportedException();
            }

        }


        private void OnSourceCollectionChanged(Object sender,
                                               NotifyCollectionChangedEventArgs e)
        {
            e.HandleCollectionChanges<Object>(OnRemoveItems, OnAddItems);
        }

        private void OnAddItems(IEnumerable<Object> objs)
        {
            OnAddItarItems(objs);
        }

        private void OnAddItarItems(IEnumerable objs)
        {
            var targetVal = _targetGetter.Invoke(_target, EmptyObjectArray) as IList
                            ?? throw new NullReferenceException(_targetPropertyName);

            foreach (var obj in objs)
            {
                targetVal.Add(obj);
            }
        }

        private void OnRemoveItarItems(IEnumerable objs)
        {
            var targetVal = _targetGetter.Invoke(_target, EmptyObjectArray) as IList
                            ?? throw new NullReferenceException(_targetPropertyName);

            foreach (var obj in objs)
            {
                targetVal.Remove(obj);
            }
        }

        private void OnRemoveItems(IEnumerable<Object> obj)
        {
            OnRemoveItarItems(obj);
        }

        private void OnSourcePropertyChanged(Object sender,
                                             PropertyChangedEventArgs e)
        {
            if (e.PropertyName != _sourcePropertyName)
                return;

            SetTargetFromSource();
        }

        protected INotifyPropertyChanged? _source;
        protected readonly IBindableElement _target;

        private readonly MethodInfo _sourceGetter;
        protected readonly MethodInfo _targetGetter;

        private readonly MethodInfo? _targetSetter;


        private INotifyCollectionChanged? _notifyingCollection;
        //private readonly Boolean _isCollectionBinding;
        
        private readonly String _sourcePropertyName;
        protected readonly String _targetPropertyName;
        
        //private readonly PropertyInfo _targetProperty;
        
        
    }
}