using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Das.Views.Defaults;
using Das.Views.Mvvm;
using Das.Views.Panels;
using Das.Views.Rendering;

namespace Das.Views
{
    public abstract class ItemsControl<T> : BasePanel<T>,
                                IItemsControl<T>
    {
        public ItemsControl(IVisualBootstrapper visualBootstrapper) 
            : base(visualBootstrapper)
        {
            _defaultTemplate = new DefaultContentTemplate(visualBootstrapper, null);
            _itemTemplate = _defaultTemplate;
        }

        private INotifyingCollection<T>? _itemsSource;

        public INotifyingCollection<T>? ItemsSource
        {
            get => _itemsSource;
            set => SetValue(ref _itemsSource, value,
                OnItemsSourceChanging);
        }

        private IDataTemplate _itemTemplate;
        private readonly IDataTemplate _defaultTemplate;

        INotifyingCollection? IItemsControl.ItemsSource => ItemsSource;

        public virtual IDataTemplate? ItemTemplate
        {
            get => _itemTemplate;
            set => SetValue(ref _itemTemplate,
                value ?? _defaultTemplate);
        }

        private Boolean OnItemsSourceChanging(INotifyingCollection<T>? oldValue, 
                                              INotifyingCollection<T>? newValue)
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

        protected virtual async void AddNewItems(IEnumerable<Object> items)
        {
            foreach (var item in items)
            {
                var visual = _itemTemplate.BuildVisual(item) ?? throw new NullReferenceException();

                if (visual is INotifyPropertyChanged notifier)
                    notifier.PropertyChanged += OnVisualChildPropertyChanged;
                await AddNewVisualAsync(visual);
            }
        }

        protected abstract void ClearVisuals();

        protected abstract Task AddNewVisualAsync(IVisualElement element);

        protected abstract IVisualElement? RemoveVisual(Object removing);

        protected virtual void RemoveOldItems(IEnumerable<Object> obj)
        {
            foreach (var rip in obj)
            {
                var rem = RemoveVisual(rip);

                //var rem = TabItems.FirstOrDefault(t =>
                //    t is IBindableElement bindable && bindable.DataContext == rip);
                if (rem != null)
                {
                    if (rem is INotifyPropertyChanged notifier)
                        notifier.PropertyChanged -= OnVisualChildPropertyChanged;
                    //TabItems.Remove(rem);
                    rem.Dispose();
                }
            }
        }

        protected void OnVisualChildPropertyChanged(Object sender, 
                                           PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {

                case nameof(IVisualElement.IsRequiresArrange) 
                    when sender is IVisualElement visual && visual.IsRequiresArrange:
                    IsRequiresArrange = true;
                    break;

                case nameof(IVisualElement.IsRequiresMeasure):
                    break;

            }
        }

       
    }
}
