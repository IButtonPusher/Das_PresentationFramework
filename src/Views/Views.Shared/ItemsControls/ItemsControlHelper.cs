using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Das.Mvvm;
using Das.Views.Mvvm;

namespace Das.Views.ItemsControls
{
    public class ItemsControlHelper : NotifyPropertyChangedBase
    {
        private readonly Action<IEnumerable<Object>> _onNewItems;
        private readonly Action<IEnumerable<Object>> _onRemovedItems;
        private readonly Action _onClearItems;

        public ItemsControlHelper(Action<IEnumerable<Object>> onNewItems,
                                  Action<IEnumerable<Object>> onRemovedItems,
                                  Action onClearItems,
                                  Func<INotifyingCollection?, INotifyingCollection?, Boolean>? onItemsSourceChanging,
                                  Action<IDataTemplate?>? onItemTemplateChanged)
        {
            _onNewItems = onNewItems;
            _onRemovedItems = onRemovedItems;
            _onClearItems = onClearItems;
            _onItemsSourceChanging = onItemsSourceChanging ?? DefaultOnItemsSourceChanging;
            _onItemTemplateChanged = onItemTemplateChanged ?? DefaultOnItemTemplateChanged;
        }

        private static void DefaultOnItemTemplateChanged(IDataTemplate? newValue) {}
        
        public INotifyingCollection? ItemsSource
        {
            get => _itemsSource;
            set => SetValue(ref _itemsSource, value,
                OnItemsSourceChanging);
        }
        
        private Boolean OnItemsSourceChanging(INotifyingCollection? oldValue, 
                                              INotifyingCollection? newValue)
        {
            if (_onItemsSourceChanging is { } valid && !valid(oldValue, newValue))
                return false;

            if (oldValue is { } ov)
                ov.CollectionChanged -= OnItemsCollectionChanged;

            if (newValue is { } nv)
            {
                nv.CollectionChanged += OnItemsCollectionChanged;
                _onNewItems(GetObjects(nv));
            }

            return true;
        }

        private static IEnumerable<Object> GetObjects(INotifyingCollection collection)
        {
            foreach (var item in collection)
                yield return item;
        }
        
        private void OnItemsCollectionChanged(Object sender, 
                                              NotifyCollectionChangedEventArgs e)
        {
            e.HandleCollectionChanges(_onRemovedItems, _onNewItems, _onClearItems);
        }

        private readonly Func<INotifyingCollection?, INotifyingCollection?, Boolean> _onItemsSourceChanging;
        private INotifyingCollection? _itemsSource;
        private static Boolean DefaultOnItemsSourceChanging(INotifyingCollection? oldValue,
                                                            INotifyingCollection? newValue) => true;
        
        public virtual IDataTemplate? ItemTemplate
        {
            get => _itemTemplate;
            set => SetValue(ref _itemTemplate, value, _onItemTemplateChanged);
        }

        private IDataTemplate? _itemTemplate;
        
        private readonly Action<IDataTemplate?> _onItemTemplateChanged;
    }

}