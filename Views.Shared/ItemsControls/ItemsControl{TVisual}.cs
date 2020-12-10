using Das.Views.Rendering;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Das.Views.Collections;
using Das.Views.DataBinding;
using Das.Views.Defaults;
using Das.Views.Mvvm;

namespace Das.Views.Panels
{
    public abstract class ItemsControl<TVisual, TDataContext> : BasePanel<TVisual, TDataContext>,
                                                                IItemsControl<TVisual>
        where TVisual : IVisualElement
    {
        
        protected ItemsControl(IDataBinding<TDataContext>? binding, 
                               IVisualBootstrapper visualBootstrapper,
                               IVisualCollection<TVisual> children) 
            : base(binding, visualBootstrapper, children)
        {
            _defaultTemplate = new DefaultContentTemplate(visualBootstrapper, this);
            _itemTemplate = _defaultTemplate;
        }

        protected ItemsControl(IDataBinding<TDataContext>? binding,
                               IVisualBootstrapper visualBootstrapper)
            : this(binding, visualBootstrapper, new VisualCollection<TVisual>())
        {

        }
        
        
        protected ItemsControl(IVisualBootstrapper visualBootstrapper) 
            : this(null, visualBootstrapper)
        {
            
        }

        INotifyingCollection? IItemsControl.ItemsSource => ItemsSource;

        public INotifyingCollection<TVisual>? ItemsSource
        {
            get => _itemsSource;
            set => SetValue(ref _itemsSource, value,
                OnItemsSourceChanging);
        }

        public IDataTemplate? ItemTemplate
        {
            get => _itemTemplate;
            set => SetValue(ref _itemTemplate,
                value ?? _defaultTemplate);
        }
        
        protected virtual Boolean OnItemsSourceChanging(INotifyingCollection? oldValue,
                                                        INotifyingCollection? newValue)
        {
            if (oldValue is { } wasValid)
                wasValid.CollectionChanged -= OnItemsChanged;

            if (newValue is { } dothValid)
            {
                AddNewItems(dothValid.OfType<TVisual>());
                dothValid.CollectionChanged += OnItemsChanged;
            }

            InvalidateMeasure();

            return true;
        }

        protected abstract void OnItemsChanged(Object sender,
                                               NotifyCollectionChangedEventArgs e);

        protected abstract void AddNewItems(IEnumerable<TVisual> items);
        
        
        private INotifyingCollection<TVisual>? _itemsSource;
        private IDataTemplate _itemTemplate;
        private readonly IDataTemplate _defaultTemplate;
    }
}
