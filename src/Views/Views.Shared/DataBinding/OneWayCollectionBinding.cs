using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using Das.Serializer;

namespace Das.Views.DataBinding
{
    public sealed class OneWayCollectionBinding : SourceBinding
    {
        public OneWayCollectionBinding(INotifyPropertyChanged source,
                                       String sourceProperty,
                                       IVisualElement target,
                                       String targetProperty,
                                       IValueConverter? converter,
                                       IPropertyAccessor sourcePropertyAccessor)
            : this(source,
                GetObjectPropertyOrDie(source, sourceProperty),
                target,
                GetObjectPropertyOrDie(target, targetProperty),
                converter, sourcePropertyAccessor)
        {
        }

        public OneWayCollectionBinding(INotifyPropertyChanged source, 
                                       PropertyInfo srcProp, 
                                       IVisualElement target, 
                                       PropertyInfo targetProp,
                                       IValueConverter? converter,
                                       IPropertyAccessor sourcePropertyAccessor) 
            : base(source, srcProp, target, targetProp, converter,sourcePropertyAccessor)
        {
            Evaluate();
        }

        public override void Evaluate()
        {
            var sourceValue = GetSourceValue();

            if (ReferenceEquals(_notifyingCollection, sourceValue))
                return;
            
            switch (sourceValue)
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

                    var targetValue = GetTargetValue();
                    if (targetValue == null)
                    {
                        SetTargetValue(sourceValue);
                    }

                    else if (_notifyingCollection is IEnumerable neuItar)
                        OnAddItarItems(neuItar);
                    break;

                case null:
                    return; //todo: ?

                
            }

            SetTargetValue(sourceValue);
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

        private void OnRemoveItems(IEnumerable<Object> obj)
        {
            OnRemoveItarItems(obj);
        }

        private void OnAddItarItems(IEnumerable objs)
        {
            var targetVal = GetTargetValue();

            switch (targetVal)
            {
                case null:
                    SetTargetValue(objs);
                    break;

                case IList list:
                    foreach (var obj in objs)
                        list.Add(obj);
                    
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            if (_notifyingCollection is { } valid)
                valid.CollectionChanged -= OnSourceCollectionChanged;
        }

        private void OnRemoveItarItems(IEnumerable objs)
        {
            var targetVal = GetTargetValue<IList>();

            foreach (var obj in objs)
            {
                targetVal.Remove(obj);
            }
        }

        private INotifyCollectionChanged? _notifyingCollection;
    }
}
