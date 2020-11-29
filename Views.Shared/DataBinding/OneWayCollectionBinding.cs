using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Das.Views.DataBinding
{
    public class OneWayCollectionBinding : SourceBinding
    {
        public OneWayCollectionBinding(INotifyPropertyChanged source, 
                                       String sourceProperty, 
                                       IBindableElement target, 
                                       String targetProperty) 
            : base(source, sourceProperty, target, targetProperty)
        {
        }

        public OneWayCollectionBinding(INotifyPropertyChanged source, 
                                       PropertyInfo srcProp, 
                                       IBindableElement target, 
                                       PropertyInfo targetProp) 
            : base(source, srcProp, target, targetProp)
        {
        }

        public override void Evaluate()
        {
            var sourceValue = GetSourceValue();

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
                        SetTargetValue(sourceValue);
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
