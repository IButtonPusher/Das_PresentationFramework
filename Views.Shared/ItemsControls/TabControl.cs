using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Das.ViewModels;
using Das.Views.Controls;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Defaults;
using Das.Views.Mvvm;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;
// ReSharper disable UnusedMember.Global

namespace Das.Views.ItemsControls
{
    public class TabControl : BaseContainerVisual,
                              IContentPresenter,
                              IItemsControl

    {
        private readonly IVisualBootStrapper _templateResolver;

        public TabControl(IVisualBootStrapper templateResolver)
        : base(templateResolver)
        {
            _headerUses = ValueSize.Empty;
            _contentUses =ValueSize.Empty;
            _templateResolver = templateResolver;
            HeaderTemplate = new DefaultTabHeaderTemplate(_templateResolver, this);
            _headerTemplate = HeaderTemplate;
            _itemTemplate = new DefaultTabItemTemplate(templateResolver);
            _contentTemplate = new DefaultContentTemplate(templateResolver, this);

            TabItems = new ObservableRangeCollection<IVisualElement>();
        }


        public override ValueSize Measure(IRenderSize availableSpace, 
                                          IMeasureContext measureContext)
        {
            _headerUses = measureContext.MeasureElement(_headerTemplate.Template, availableSpace);
            var remaining = availableSpace.Minus(_headerUses);

            _contentUses = measureContext.MeasureElement(_itemTemplate.Template, remaining);

            return _headerUses.PlusVertical(_contentUses);
        }

        public override void Arrange(IRenderSize availableSpace, 
                                     IRenderContext renderContext)
        {

            renderContext.DrawElement(_headerTemplate.Template,
                new ValueRenderRectangle(0, 0, _headerUses, availableSpace.Offset));
            
            renderContext.DrawElement(_contentTemplate.Template,
                new ValueRenderRectangle(0, _headerUses.Height, _contentUses, 
                    availableSpace.Offset));
        }

        private IDataTemplate _contentTemplate;

        public IDataTemplate? ContentTemplate
        {
            get => _contentTemplate;
            set => SetValue(ref _contentTemplate,
                value ?? _contentTemplate);
        }

        private IDataTemplate _itemTemplate;

        public IDataTemplate? ItemTemplate
        {
            get => _itemTemplate;
            set => SetValue(ref _itemTemplate, 
                value ?? new DefaultTabItemTemplate(_templateResolver));
        }


        private IDataTemplate _headerTemplate;

        public IDataTemplate? HeaderTemplate
        {
            get => _headerTemplate;
            set => SetValue(ref _headerTemplate, 
                value ?? new DefaultTabHeaderTemplate(_templateResolver, this),
                OnHeaderTemplateChanging);
        }

        private Boolean OnHeaderTemplateChanging(IDataTemplate? oldValue, 
                                                 IDataTemplate? newValue)
        {
            if (oldValue?.Template is INotifyPropertyChanged oldValid)
                oldValid.PropertyChanged -= OnChildPropertyChanged;

            if (newValue?.Template is INotifyPropertyChanged newValid)
                newValid.PropertyChanged += OnChildPropertyChanged;

            return true;
        }



        private INotifyingCollection? _itemsSource;

        public INotifyingCollection? ItemsSource
        {
            get => _itemsSource;
            set => SetValue(ref _itemsSource, value,
                OnItemsSourceChanging);
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

            return true;
        }

        private void OnItemsChanged(Object sender, 
                                    NotifyCollectionChangedEventArgs e)
        {
            e.HandleCollectionChanges<Object>(RemoveOldItems, AddNewItems);
            
        }

        private void AddNewItems(IEnumerable<Object> obj)
        {
            foreach (var item in obj)
            {
                var visual = _itemTemplate.BuildVisual(item);
                if (visual is INotifyPropertyChanged notifier)
                    notifier.PropertyChanged += OnItemPropertyChanged;
                TabItems.Add(visual);
            }
        }

        private void OnItemPropertyChanged(Object sender, 
                                           PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IToggleButton.IsChecked) when sender is IToggleButton toggle && 
                    sender is IDataContext dc && ItemsSource != null && 
                    dc.Value != null:

                    foreach (var item in ItemsSource)
                    {
                        if (item == dc.Value)
                        {
                            SelectedItem = item;
                            if (sender is IVisualElement visual)
                                SelectedTab = visual;
                            break;
                        }
                    }
                    break;

                case nameof(IVisualElement.IsRequiresArrange) 
                    when sender is IVisualElement visual && visual.IsRequiresArrange:
                    IsRequiresArrange = true;
                    break;

                //case nameof(IChangeTracking.IsChanged) when sender is IChangeTracking changer && 
                //                                            changer.IsChanged:
                //    IsChanged = true;
                //    break;
            }
        }

        private void RemoveOldItems(IEnumerable<Object> obj)
        {
            foreach (var rip in obj)
            {
                var rem = TabItems.FirstOrDefault(t =>
                    t is IBindableElement bindable && bindable.DataContext == rip);
                if (rem != null)
                {
                    if (rem is INotifyPropertyChanged notifier)
                        notifier.PropertyChanged -= OnItemPropertyChanged;
                    TabItems.Remove(rem);
                    rem.Dispose();
                }
                
            }
        }


        private Object? _selectedItem;

        public Object? SelectedItem
        {
            get => _selectedItem;
            set => SetValue(ref _selectedItem, value, OnSelectedItemChanging, 
                OnSelectedItemChanged);
        }

        private void OnSelectedItemChanged(Object? newValue)
        {
            if (newValue != null)
            {
                foreach (var item in TabItems)
                {
                    if (item is IToggleButton toggle && toggle.DataContext == newValue)
                    {
                        SelectedTab = toggle;
                        break;
                    }
                }
            }

            OnChanged(newValue);
        }

        private Boolean OnSelectedItemChanging(Object? oldValue, 
                                               Object? newValue)
        {
            if (ItemsSource == null) 
                return true;

            foreach (var item in TabItems)
            {
                if (item is IDataContext dc && dc.Value == oldValue && 
                    item is IToggleButton toggle)
                {
                    toggle.IsChecked = false;
                    break;
                }
                
               
            }

            return true;
        }

        private void OnChanged(Object? obj)
        {
            IsChanged = true;
        }


        private IVisualElement? _selectedTab;

        public IVisualElement? SelectedTab
        {
            get => _selectedTab;
            set => SetValue(ref _selectedTab, value);
        }

        public ObservableRangeCollection<IVisualElement> TabItems { get; }

        


        

        
        private ValueSize _headerUses;
        private ValueSize _contentUses;
    }
}
