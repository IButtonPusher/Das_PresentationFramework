using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Das.Views.Collections;
using Das.Views.DataBinding;
using Das.Views.Defaults;
using Das.Views.Mvvm;

namespace Das.Views.Panels
{
    public abstract class ItemsControl<TDataContext, TItems> : BasePanel<TDataContext>,
                                   IItemsControl<TItems>
    {
        protected ItemsControl(
                               IVisualBootstrapper visualBootstrapper,
                               IVisualCollection children) 
            : base(//binding, 
                visualBootstrapper, children)
        {
            _defaultTemplate = new DefaultContentTemplate(visualBootstrapper, this);
            _itemTemplate = _defaultTemplate;
        }

        //protected ItemsControl(//IDataBinding<T>? binding,
        //                       IVisualBootstrapper visualBootstrapper)
        //    : this(//binding, 
        //        visualBootstrapper, new VisualCollection())
        //{

        //}
        
        protected ItemsControl(IVisualBootstrapper visualBootstrapper) 
            : this(visualBootstrapper, new VisualCollection())
        {
            //_defaultTemplate = new DefaultContentTemplate<T>(visualBootstrapper, this);
            //_itemTemplate = _defaultTemplate;
        }

        INotifyingCollection? IItemsControl.ItemsSource => ItemsSource;

        public INotifyingCollection<T>? ItemsSource
        {
            get => _itemsSource;
            set => SetValue(ref _itemsSource, value,
                OnItemsSourceChanging);
        }

        public IDataTemplate? ItemTemplate
        {
            get => _itemTemplate;
            set => SetValue(ref _itemTemplate,
                value ?? _defaultTemplate, OnItemTemplateChanged);
        }

        protected virtual void OnItemTemplateChanged(IDataTemplate? obj)
        {
            
        }

        protected virtual Boolean OnItemsSourceChanging(INotifyingCollection? oldValue,
                                                        INotifyingCollection? newValue)
        {
            if (oldValue is { } wasValid)
                wasValid.CollectionChanged -= OnItemsChanged;

            if (newValue is { } dothValid)
            {
                AddNewItems(dothValid.OfType<T>());
                dothValid.CollectionChanged += OnItemsChanged;
            }

            InvalidateMeasure();

            return true;
        }

        protected abstract void OnItemsChanged(Object sender,
                                               NotifyCollectionChangedEventArgs e);

        protected abstract void AddNewItems(IEnumerable<T> items);
        
        
        private INotifyingCollection<T>? _itemsSource;
        private IDataTemplate _itemTemplate;
        private readonly IDataTemplate _defaultTemplate;
    }
}
