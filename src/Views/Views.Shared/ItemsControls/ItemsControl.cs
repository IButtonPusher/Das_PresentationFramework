using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Das.Views.Defaults;
using Das.Views.Mvvm;
using Das.Views.Panels;

namespace Das.Views
{
    public abstract class ItemsControl : BasePanel,
                                IItemsControl
    {
        protected ItemsControl(IVisualBootstrapper visualBootstrapper) 
            : base(visualBootstrapper)
        {
            _defaultTemplate = new DefaultContentTemplate(visualBootstrapper);
            _itemTemplate = _defaultTemplate;
        }

        private INotifyingCollection? _itemsSource;

        public INotifyingCollection? ItemsSource
        {
            get => _itemsSource;
            // ReSharper disable once UnusedMember.Global
            set => SetValue(ref _itemsSource, value,
                OnItemsSourceChanging);
        }

        protected IDataTemplate _itemTemplate;
        private readonly IDataTemplate _defaultTemplate;

        public virtual IDataTemplate? ItemTemplate
        {
            get => _itemTemplate;
            set => SetValue(ref _itemTemplate,
                value ?? _defaultTemplate);
        }

        private Boolean OnItemsSourceChanging(INotifyingCollection? oldValue, 
                                              INotifyingCollection? newValue)
        {
            if (oldValue is { } wasValid)
                wasValid.CollectionChanged -= OnItemsChanged;

            if (newValue is { } dothValid)
            {
                AddNewItems(dothValid.OfType<Object>());
                dothValid.CollectionChanged += OnItemsChanged;
            }

            InvalidateMeasure();

            return true;
        }

        private void OnItemsChanged(Object sender, 
                                    NotifyCollectionChangedEventArgs e)
        {
            e.HandleCollectionChanges<Object>(RemoveOldItems, AddNewItems, ClearVisuals);
            InvalidateMeasure();
        }
        
        protected abstract void AddNewItems(IEnumerable<Object> items);

        protected abstract void ClearVisuals();

        protected abstract void RemoveOldItems(IEnumerable<Object> obj);

        //protected void OnVisualChildPropertyChanged(Object sender, 
        //                                            PropertyChangedEventArgs e)
        //{
        //    switch (e.PropertyName)
        //    {

        //        case nameof(IVisualElement.IsRequiresArrange) 
        //            when sender is IVisualElement visual && visual.IsRequiresArrange:
        //            IsRequiresArrange = true;
        //            break;

        //        case nameof(IVisualElement.IsRequiresMeasure):
        //            break;

        //    }
        //}

       
    }
}
