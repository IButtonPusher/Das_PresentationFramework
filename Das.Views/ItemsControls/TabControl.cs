using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
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
    public interface ITabControl : IItemsControl
                                   
    {
        INotifyingCollection TabItems { get; }

        //AsyncObservableCollection2<IVisualElement> TabItems { get; }

        IVisualElement? SelectedTab { get; }
    }

    public interface ITabControl<T> : IItemsControl<T>,
                                      ITabControl
    {
        new AsyncObservableCollection2<IBindableElement<T>> TabItems { get; }

        new IBindableElement<T>? SelectedTab { get; }
    }

    public class TabControl<T> : SelectorVisual<T>,
                                 IContentPresenter,
                                 ITabControl<T> 
        where T : IEquatable<T>

    {
        private readonly IVisualBootstrapper _visualBootstrapper;

        public TabControl(IVisualBootstrapper visualBootstrapper)
        : base(visualBootstrapper)
        {
            _headerUses = ValueSize.Empty;
            _contentUses =ValueSize.Empty;
            _visualBootstrapper = visualBootstrapper;

            TabItems = new AsyncObservableCollection2<IBindableElement<T>>();
            TabItems.CollectionChanged += OnTabItemsChanged;
            
            HeaderTemplate = new DefaultTabHeaderTemplate(_visualBootstrapper, this);
            _headerTemplate = HeaderTemplate;
            _defaultItemTemplate = new DefaultTabItemTemplate<T>(visualBootstrapper, this);
            _itemTemplate = _defaultItemTemplate;
            _contentTemplate = new DefaultContentTemplate(visualBootstrapper, this);
        }

        private async void OnTabItemsChanged(Object sender, 
                                       NotifyCollectionChangedEventArgs e)
        {
            if (SelectedTab != null)
                return;

            SelectedTab = await TabItems.FirstOrDefaultAsync();
        }


        public override ValueSize Measure(IRenderSize availableSpace, 
                                          IMeasureContext measureContext)
        {
            if (!(_headerPanel is {} header))
                return ValueSize.Empty;


            if (SelectedTab is { } visual)
            {
                //TODO:
                _contentUses = measureContext.MeasureElement(visual, availableSpace);
                var remaining = availableSpace.Minus(_contentUses);
                _headerUses = measureContext.MeasureElement(header, remaining);

                return _headerUses;
            }

            _contentUses = ValueSize.Empty;
            _headerUses = measureContext.MeasureElement(header, availableSpace);
            return _headerUses;
        }

        public override void Arrange(IRenderSize availableSpace, 
                                     IRenderContext renderContext)
        {
            if (!(_headerPanel is {} header))
                return;

            renderContext.DrawElement(header,
                new ValueRenderRectangle(0, 0, 
                    availableSpace.Width,
                    availableSpace.Height, 
                    availableSpace.Offset));
                //new ValueRenderRectangle(0, 0, _headerUses, availableSpace.Offset));
        }

        private IDataTemplate _contentTemplate;

        public IDataTemplate? ContentTemplate
        {
            get => _contentTemplate;
            set => SetValue(ref _contentTemplate,
                value ?? _contentTemplate);
        }

        private IDataTemplate _defaultItemTemplate;
        private IDataTemplate _itemTemplate;

        INotifyingCollection? IItemsControl.ItemsSource => ItemsSource;

        public IDataTemplate? ItemTemplate
        {
            get => _itemTemplate;
            set => SetValue(ref _itemTemplate, 
                value ?? _defaultItemTemplate);
        }


        private IDataTemplate _headerTemplate;

        public IDataTemplate? HeaderTemplate
        {
            get => _headerTemplate;
            set => SetValue(ref _headerTemplate, 
                value ?? new DefaultTabHeaderTemplate(_visualBootstrapper, this),
                OnHeaderTemplateChanging, OnHeaderTemplateChanged);
        }

        private void OnHeaderTemplateChanged(IDataTemplate? newValue)
        {
            if (newValue is { } valid)
            {
                var bob = valid.BuildVisual(DataContext);

                _headerPanel = bob;
                if (_headerPanel is {} hp)
                    AddChild(hp);
            }
        }

        private Boolean OnHeaderTemplateChanging(IDataTemplate? oldValue, 
                                                 IDataTemplate? newValue)
        {
            if (_headerPanel is {} oldValid)
            {
                Children.Remove(oldValid);
                //oldValid.PropertyChanged -= OnChildPropertyChanged;
            }

            //if (newValue is {} valid)
            //    valid.BuildVisual(DataContext)

            //if (newValue?.Template is INotifyPropertyChanged newValid)
            //    newValid.PropertyChanged += OnChildPropertyChanged;

            return true;
        }



        private INotifyingCollection<T>? _itemsSource;

        public INotifyingCollection<T>? ItemsSource
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
                AddNewItems(dothValid.OfType<T>());
                dothValid.CollectionChanged += OnItemsChanged;
            }

            InvalidateMeasure();

            return true;
        }

        private void OnItemsChanged(Object sender, 
                                    NotifyCollectionChangedEventArgs e)
        {
            e.HandleCollectionChanges<T>(RemoveOldItems, AddNewItems, ClearTabs);
            InvalidateMeasure();
        }

        

        private async void AddNewItems(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                var visual = _itemTemplate.BuildVisual<IBindableElement<T>>(item) ?? throw new NullReferenceException();

                if (visual is INotifyPropertyChanged notifier)
                    notifier.PropertyChanged += OnItemPropertyChanged;
                await TabItems.AddAsync(visual);
            }
        }

        private void OnItemPropertyChanged(Object sender, 
                                           PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IToggleButton.IsChecked) when sender is IToggleButton toggle && 
                    sender is IDataContext dc && ItemsSource != null && 
                    toggle.IsChecked == true && dc.Value != null:

                    foreach (var item in ItemsSource)
                    {
                        //if (item == dc.Value)
                        if (Equals(item, dc.Value))
                        {
                            SelectedItem = item;
                            if (sender is IBindableElement<T> visual)
                                SelectedTab = visual;
                            break;
                        }
                    }
                    break;

                case nameof(IVisualElement.IsRequiresArrange) 
                    when sender is IVisualElement visual && visual.IsRequiresArrange:
                    IsRequiresArrange = true;
                    break;

                case nameof(IVisualElement.IsRequiresMeasure):
                    break;

            }
        }

        private void RemoveOldItems(IEnumerable<T> obj)
        {
            foreach (var rip in obj)
            {
                var rem = TabItems.FirstOrDefault(t =>
                    t is IBindableElement<T> bindable && Equals(bindable.DataContext, rip));
                if (rem != null)
                {
                    if (rem is INotifyPropertyChanged notifier)
                        notifier.PropertyChanged -= OnItemPropertyChanged;
                    TabItems.Remove(rem);
                    rem.Dispose();
                }
                
            }
        }

        private void ClearTabs()
        {
            TabItems.Clear();
        }


        public override IBindableElement<T>? SelectedVisual
        {
            get => SelectedTab;
            set => SelectedTab = value;
        }

        protected override void OnSelectedItemChanged(T oldValue,
                                                      T newValue)
        {
            if (newValue != null)
            {
                foreach (var item in TabItems)
                {
                    if (item is IBindableElement<T> toggle && Equals(toggle.DataContext, newValue))
                    {
                        SelectedTab = toggle;
                        break;
                    }
                }
            }
        }

        protected override Boolean OnSelectedItemChanging(T oldValue, 
                                                          T newValue)
        {
            if (ItemsSource == null) 
                return true;


            foreach (var item in TabItems)
            {
                if (item is IDataContext dc && Equals(dc.Value, oldValue) && 
                    item is IToggleButton toggle)
                {
                    toggle.IsChecked = false;
                    break;
                }
            }

            return true;
        }


        private IBindableElement<T>? _selectedTab;


        INotifyingCollection ITabControl.TabItems => TabItems;

        IVisualElement? ITabControl.SelectedTab => SelectedTab;

        public IBindableElement<T>? SelectedTab
        {
            get => _selectedTab;
            set => SetValue(ref _selectedTab, value, OnSelectedTabChanged);
        }

        private void OnSelectedTabChanged(IVisualElement? newValue)
        {
            Debug.WriteLine("selected tab changed: " + newValue);
            InvalidateMeasure();
        }

        public AsyncObservableCollection2<IBindableElement<T>> TabItems { get; }

        private IVisualElement? _headerPanel;
        private ValueSize _headerUses;
        private ValueSize _contentUses;
        
    }
}
