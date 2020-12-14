using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Das.ViewModels;
using Das.Views.Controls;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Defaults;
using Das.Views.ItemsControls;
using Das.Views.Mvvm;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;
using Das.Views.Templates;

// ReSharper disable UnusedMember.Global

namespace Das.Views
{
    public class TabControl : SelectorVisual,
                                 ITabControl
    {
        public TabControl(IVisualBootstrapper visualBootstrapper)
            : base(visualBootstrapper)
        {
            _headerUses = ValueSize.Empty;
            _contentUses = ValueSize.Empty;

            _updateLock = new Object();
            _lockContentVisuals = new Object();
            _tabContentVisuals = new Dictionary<Object, IVisualElement>();

            TabItems = new AsyncObservableCollection2<IVisualElement>();
            TabItems.CollectionChanged += OnTabItemsChanged;

            HeaderTemplate = new DefaultTabHeaderTemplate(_visualBootstrapper, this);
            _headerTemplate = HeaderTemplate;
            _defaultItemTemplate = new DefaultTabItemTemplate(visualBootstrapper);//, this);
            ItemTemplate = _defaultItemTemplate;
            _contentTemplate = new DefaultContentTemplate(visualBootstrapper);
        }

        public IDataTemplate? ContentTemplate
        {
            get => _contentTemplate;
            set => SetValue(ref _contentTemplate,
                value);// ?? _contentTemplate);
        }


        public override ValueSize Measure(IRenderSize availableSpace,
                                          IMeasureContext measureContext)
        {
            //lock (_updateLock)
            {
                if (!(_headerPanel is { } header))
                    return ValueSize.Empty;

                if (SelectedTab is { })
                {
                    //_contentUses = measureContext.MeasureElement(visual, availableSpace);
                    //var remaining = availableSpace.Minus(_contentUses);
                    _headerUses = measureContext.MeasureElement(header, availableSpace);

                    //return _headerUses;
                }
                else
                {
                    _headerUses = measureContext.MeasureElement(header, availableSpace);
                }

                var remainingHeight = availableSpace.Height - _headerUses.Height;

                if (SelectedContent is { } contentVisual)
                {
                    var available = new ValueRenderSize(availableSpace.Width, remainingHeight,
                        availableSpace.Offset);
                    _contentUses = measureContext.MeasureElement(contentVisual, available);
                    //var remaining = availableSpace.Minus(_contentUses);
                }
                else
                {
                    _contentUses = ValueSize.Empty;
                    return _headerUses;
                }



                return new ValueSize(Math.Max(_headerUses.Width, _contentUses.Width),
                    _headerUses.Height + _contentUses.Height);
                
                //_contentUses = ValueSize.Empty;
                //_headerUses = measureContext.MeasureElement(header, availableSpace);
                //return _headerUses;
            }
        }

        public override void Arrange(IRenderSize availableSpace,
                                     IRenderContext renderContext)
        {
            //lock (_updateLock)
            {
                var targetRect = new ValueRenderRectangle(0, 0,
                    availableSpace.Width,
                    _headerUses.Height,
                    //availableSpace.Height,
                    availableSpace.Offset);
                
                if (_headerPanel is { } header)
                {
                    renderContext.DrawElement(header, targetRect);
                }

                if (!(SelectedContent is { } selectedContent))
                    return;

                targetRect = new ValueRenderRectangle(0,
                    _headerUses.Height,
                    availableSpace.Width,
                    availableSpace.Height - _headerUses.Height,
                    availableSpace.Offset);
                
                renderContext.DrawElement(selectedContent, targetRect);

                //new ValueRenderRectangle(0, 0, _headerUses, availableSpace.Offset));
            }
        }

        INotifyingCollection? IItemsControl.ItemsSource => ItemsSource;

        ///// <summary>
        ///// The template for individual tabs
        ///// </summary>
        //public IDataTemplate? ItemTemplate
        //{
        //    get => _itemTemplate;
        //    set => SetValue(ref _itemTemplate,
        //        value ?? _defaultItemTemplate);
        //}

        //public INotifyingCollection<T>? ItemsSource
        //{
        //    get => _itemsSource;
        //    set => SetValue(ref _itemsSource, value,
        //        OnItemsSourceChanging);
        //}


        INotifyingCollection ITabControl.TabItems => TabItems;

        IVisualElement? ITabControl.SelectedTab => SelectedTab;

        public IVisualElement? SelectedTab
        {
            get => _selectedTab;
            set => SetValue(ref _selectedTab, value, OnSelectedTabChanged);
        }

        //IAsyncObservableCollection<IBindableElement> ITabControl<TDataContext, TItems>.TabItems => TabItems;

        public IDataTemplate? HeaderTemplate
        {
            get => _headerTemplate;
            set => SetValue(ref _headerTemplate,
                value ?? new DefaultTabHeaderTemplate(_visualBootstrapper, this),
                OnHeaderTemplateChanging, OnHeaderTemplateChanged);
        }

        /// <summary>
        /// The selected tab 
        /// </summary>
        public override IVisualElement? SelectedVisual
        {
            get => SelectedTab;
            set => SelectedTab = value;
        }


        private IVisualElement? _selectedContent;

        /// <summary>
        /// The main contents.  Changes based on which tab is selected
        /// </summary>
        public IVisualElement? SelectedContent
        {
            get => _selectedContent;
            set => SetValue(ref _selectedContent, value, OnSelectedContentChanged);
        }

        private void OnSelectedContentChanged(IVisualElement? obj)
        {
            InvalidateMeasure();
        }

        public AsyncObservableCollection2<IVisualElement> TabItems { get; }

        /// <summary>
        /// The selected view model changed so try to update the main contents
        /// </summary>
        protected override void OnSelectedItemChanged(Object? oldValue,
                                                      Object? newValue)
        {
            lock (_updateLock)
            {
                if (newValue == null)
                {
                    SelectedContent = default;
                    return;
                }

                // Set the tab button as selected
                foreach (var item in TabItems)
                    if (item is IBindableElement toggle && Equals(toggle.DataContext, newValue))
                    {
                        SelectedTab = toggle;
                        break;
                    }

                // update the main contents
                lock (_lockContentVisuals)
                {
                    if (_tabContentVisuals.TryGetValue(newValue, out var visual))
                        SelectedContent = visual;
                    else if (ContentTemplate is { } contentTemplate)
                    {
                        visual = contentTemplate.BuildVisual(newValue);
                        if (visual != null)
                        {
                            _tabContentVisuals.Add(newValue, visual);
                            SelectedContent = visual;
                        }
                        else SelectedContent = default;
                    }
                }
            }
        }

        protected override Boolean OnSelectedItemChanging(Object? oldValue,
                                                          Object? newValue)
        {
            if (ItemsSource == null)
                return true;


            foreach (var item in TabItems)
                if (item is IBindableElement dc && Equals(dc.DataContext, oldValue) &&
                    item is IToggleButton toggle)
                {
                    toggle.IsChecked = false;
                    break;
                }

            return true;
        }


        protected override async void AddNewItems(IEnumerable<Object> items)
        {
            var itemTemplate = ItemTemplate ?? _defaultItemTemplate;
            
            foreach (var item in items)
            {
                var visual = itemTemplate.BuildVisual(item) ?? throw new NullReferenceException();

                if (visual is INotifyPropertyChanged notifier)
                    notifier.PropertyChanged += OnItemPropertyChanged;
                await TabItems.AddAsync(visual);
            }
        }

        protected override void ClearVisuals()
        {
            TabItems.Clear();
        }

        //private void ClearTabs()
        //{
        //    TabItems.Clear();
        //}

        private void OnHeaderTemplateChanged(IDataTemplate? newValue)
        {
            if (newValue is { } valid)
            {
                var bob = valid.BuildVisual(DataContext);

                _headerPanel = bob;
                if (_headerPanel is { } hp)
                    AddChild(hp);
            }
        }

        private Boolean OnHeaderTemplateChanging(IDataTemplate? oldValue,
                                                 IDataTemplate? newValue)
        {
            if (_headerPanel is { } oldValid)
                _children.Remove(oldValid);
            //oldValid.PropertyChanged -= OnChildPropertyChanged;

            //if (newValue is {} valid)
            //    valid.BuildVisual(DataContext)

            //if (newValue?.Template is INotifyPropertyChanged newValid)
            //    newValid.PropertyChanged += OnChildPropertyChanged;

            return true;
        }

        private void OnItemPropertyChanged(Object sender,
                                           PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IToggleButton.IsChecked) when sender is IToggleButton toggle &&
                                                          sender is IBindable dc && ItemsSource != null &&
                                                          toggle.IsChecked == true && dc.DataContext != null:

                    foreach (var item in ItemsSource)
                        //if (item == dc.Value)
                        if (Equals(item, dc.DataContext))
                        {
                            SelectedItem = item;
                            if (sender is IBindableElement visual)
                                SelectedTab = visual;
                            break;
                        }

                    break;

                //case nameof(IVisualElement.IsRequiresArrange)
                //    when sender is IVisualElement visual && visual.IsRequiresArrange:
                //    IsRequiresArrange = true;
                //    break;

                case nameof(IVisualElement.IsRequiresMeasure):
                    break;
            }
        }

        //protected override void OnItemsChanged(Object sender,
        //                            NotifyCollectionChangedEventArgs e)
        //{
        //    e.HandleCollectionChanges<Object>(RemoveOldItems, AddNewItems, ClearTabs);
        //    InvalidateMeasure();
        //}

        //protected override Boolean OnItemsSourceChanging(INotifyingCollection? oldValue,
        //                                                 INotifyingCollection? newValue)
        //{
        //    if (oldValue is { } wasValid)
        //        wasValid.CollectionChanged -= OnItemsChanged;

        //    if (newValue is { } dothValid)
        //    {
        //        AddNewItems(dothValid.OfType<T>());
        //        dothValid.CollectionChanged += OnItemsChanged;
        //    }

        //    InvalidateMeasure();

        //    return true;
        //}

        private void OnSelectedTabChanged(IVisualElement? newValue)
        {
            Debug.WriteLine("selected tab changed: " + newValue);

            InvalidateMeasure();
        }

        private async void OnTabItemsChanged(Object sender,
                                             NotifyCollectionChangedEventArgs e)
        {
            if (SelectedTab != null)
                return;

            SelectedTab = await TabItems.FirstOrDefaultAsync();
        }

        protected override void RemoveOldItems(IEnumerable<Object> obj)
        {
            foreach (var rip in obj)
            {
                var rem = TabItems.FirstOrDefault(t =>
                    t is IBindableElement bindable && Equals(bindable.DataContext, rip));
                if (rem != null)
                {
                    if (rem is INotifyPropertyChanged notifier)
                        notifier.PropertyChanged -= OnItemPropertyChanged;
                    TabItems.Remove(rem);
                    rem.Dispose();
                }
            }
        }

        private readonly IDataTemplate _defaultItemTemplate;
        private readonly Dictionary<Object, IVisualElement> _tabContentVisuals;
        private readonly Object _lockContentVisuals;
        private readonly Object _updateLock;

        private IDataTemplate? _contentTemplate;
        private ValueSize _contentUses;

        private IVisualElement? _headerPanel;


        private IDataTemplate _headerTemplate;
        private ValueSize _headerUses;


        //private INotifyingCollection<T>? _itemsSource;
        //private IDataTemplate _itemTemplate;


        private IVisualElement? _selectedTab;

        protected override void OnDistributeDataContextToChildren(Object? newValue)
        {
            if (_headerPanel is IBindableElement bindable)
                bindable.DataContext = newValue;
        }
    }
}