using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Das.Views.Mvvm;
using Das.Views.Templates;

namespace Das.Views.ItemsControls
{
    public class ItemsControlHelper<T> : NotifyPropertyChangedBase
    {
        private readonly Action<IEnumerable<T>> _onNewItems;
        private readonly Action<IEnumerable<T>> _onRemovedItems;
        private readonly Action _onClearItems;

        public ItemsControlHelper(Action<IEnumerable<T>> onNewItems,
                                  Action<IEnumerable<T>> onRemovedItems,
                                  Action onClearItems,
                                  Func<INotifyingCollection<T>?, INotifyingCollection<T>?, Boolean>? onItemsSourceChanging,
                                  Action<IDataTemplate<T>?>? onItemTemplateChanged)
        
        {
            _onNewItems = onNewItems;
            _onRemovedItems = onRemovedItems;
            _onClearItems = onClearItems;
            
            _onItemTemplateChanged = onItemTemplateChanged ?? DefaultOnItemTemplateChanged;
            _onItemsSourceChanging = onItemsSourceChanging ?? DefaultOnItemsSourceChanging;
        }
        
        private static void DefaultOnItemTemplateChanged(IDataTemplate? newValue) {}
        
        
        public virtual IDataTemplate<T>? ItemTemplate
        {
            get => _itemTemplate;
            set => SetValue(ref _itemTemplate, value, _onItemTemplateChanged);
        }
        
        private readonly Action<IDataTemplate<T>?> _onItemTemplateChanged;
        private IDataTemplate<T>? _itemTemplate;
        
        
        //public ItemsControlHelper(
        //    Func<INotifyingCollection<T>?, INotifyingCollection<T>?, Boolean>? onItemsSourceChanging,
        //    Action<IDataTemplate?>? onItemTemplateChanged) 
        //    : base(onItemTemplateChanged)
        //{
        //    _onItemsSourceChanging = onItemsSourceChanging ?? DefaultOnItemsSourceChanging;
        //}
        
        public INotifyingCollection<T>? ItemsSource
        {
            get => _itemsSource;
            set => SetValue(ref _itemsSource, value, OnItemsSourceChanging);
        }

        private Boolean OnItemsSourceChanging(INotifyingCollection<T>? oldValue, 
                                              INotifyingCollection<T>? newValue)
        {
            if (_onItemsSourceChanging is { } valid && !valid(oldValue, newValue))
                return false;

            if (oldValue is { } ov)
                ov.CollectionChanged -= OnItemsCollectionChanged;

            if (newValue is { } nv)
            {
                nv.CollectionChanged += OnItemsCollectionChanged;
                _onNewItems(nv);
            }

            return true;
        }

        private void OnItemsCollectionChanged(Object sender, 
                                              NotifyCollectionChangedEventArgs e)
        {
            e.HandleCollectionChanges(_onRemovedItems, _onNewItems, _onClearItems);
        }


        private readonly Func<INotifyingCollection<T>?, INotifyingCollection<T>?, Boolean> _onItemsSourceChanging;
        private INotifyingCollection<T>? _itemsSource;
        private static Boolean DefaultOnItemsSourceChanging(INotifyingCollection<T>? oldValue,
                                                            INotifyingCollection<T>? newValue) => true;
    } }
