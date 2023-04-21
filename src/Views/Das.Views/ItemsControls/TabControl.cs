using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
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

// ReSharper disable UnusedMember.Global

namespace Das.Views
{
    // ReSharper disable once UnusedType.Global
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

            //TabItems = new AsyncObservableCollection2<IVisualElement>();
            TabItems = new ObservableRangeCollection<IVisualElement>();
            TabItems.CollectionChanged += OnTabItemsChanged;

            HeaderTemplate = new DefaultTabHeaderTemplate(_visualBootstrapper, this);
            _headerTemplate = HeaderTemplate;
            _defaultItemTemplate = new DefaultTabItemTemplate(visualBootstrapper); //, this);
            _itemTemplate = _defaultItemTemplate;
            _contentTemplate = new DefaultContentTemplate(visualBootstrapper);
        }

        public IDataTemplate? ContentTemplate
        {
            get => _contentTemplate;
            set => SetValue(ref _contentTemplate, value);
        }


        public override ValueSize Measure<TRenderSize>(TRenderSize availableSpace,
                                                      IMeasureContext measureContext)
        {
            if (!(_headerPanel is { } header))
                return ValueSize.Empty;

            if (SelectedTab is { })
                _headerUses = measureContext.MeasureElement(header, availableSpace);
            else
                _headerUses = measureContext.MeasureElement(header, availableSpace);

            var remainingHeight = availableSpace.Height - _headerUses.Height;

            if (SelectedContent is { } contentVisual)
            {
                var available = new ValueRenderSize(availableSpace.Width, remainingHeight,
                    availableSpace.Offset);
                _contentUses = measureContext.MeasureElement(contentVisual, available);
            }
            else
            {
                _contentUses = ValueSize.Empty;
                return _headerUses;
            }


            return new ValueSize(Math.Max(_headerUses.Width, _contentUses.Width),
                _headerUses.Height + _contentUses.Height);
        }

        public override void Arrange<TRenderSize>(TRenderSize availableSpace,
                                                 IRenderContext renderContext)
        {
            //var targetRect = new ValueRenderRectangle(0, 0,
            //    availableSpace.Width,
            //    _headerUses.Height,
            //    availableSpace.Offset);

            //if (_headerPanel is { } header) renderContext.DrawElement(header, targetRect);

            //if (!(SelectedContent is { } selectedContent))
            //    return;

            ValueRenderRectangle targetRect;

            if ((SelectedContent is { } selectedContent))
            {
               targetRect = new ValueRenderRectangle(0,
                  _headerUses.Height,
                  availableSpace.Width,
                  availableSpace.Height - _headerUses.Height,
                  availableSpace.Offset);

               renderContext.DrawElement(selectedContent, targetRect);
            }

            ///////////////////////

            if (_headerPanel is { } header)
            {
               targetRect = new ValueRenderRectangle(0, 0,
                  availableSpace.Width,
                  _headerUses.Height,
                  availableSpace.Offset);

               //Debug.WriteLine("draw tab header");

               renderContext.DrawElement(header, targetRect);

               //Debug.WriteLine("drew tab header");
            }
        }

        INotifyingCollection? IItemsControl.ItemsSource => ItemsSource;


        INotifyingCollection ITabControl.TabItems => TabItems;

        IVisualElement? ITabControl.SelectedTab => SelectedTab;

        public IDataTemplate? HeaderTemplate
        {
            get => _headerTemplate;
            set => SetValue(ref _headerTemplate,
                value ?? new DefaultTabHeaderTemplate(_visualBootstrapper, this),
                OnHeaderTemplateChanging, OnHeaderTemplateChanged);
        }

        /// <summary>
        ///     The main contents.  Changes based on which tab is selected
        /// </summary>
        public IVisualElement? SelectedContent
        {
            get => _selectedContent;
            set => SetValue(ref _selectedContent, value, OnSelectedContentChanged);
        }

        public IVisualElement? SelectedTab
        {
            get => _selectedTab;
            set => SetValue(ref _selectedTab, value, OnSelectedTabChanged);
        }

        /// <summary>
        ///     The selected tab
        /// </summary>
        public override IVisualElement? SelectedVisual
        {
            get => SelectedTab;
            set => SelectedTab = value;
        }

        //public AsyncObservableCollection2<IVisualElement> TabItems { get; }

        public ObservableRangeCollection<IVisualElement> TabItems { get; }


        protected override void AddNewItems(IEnumerable<Object> items)
        {
            var itemTemplate = ItemTemplate ?? _defaultItemTemplate;

            foreach (var item in items)
            {
                var visual = itemTemplate.BuildVisual(item) ?? throw new NullReferenceException();

                if (visual is INotifyPropertyChanged notifier)
                    notifier.PropertyChanged += OnItemPropertyChanged;
                TabItems.Add(visual);
                //await TabItems.AddAsync(visual);
            }
        }

        protected override void ClearVisuals()
        {
            TabItems.Clear();
        }

        protected override void OnDistributeDataContextToChildren(Object? newValue)
        {
            if (_headerPanel is IBindableElement bindable)
                bindable.DataContext = newValue;
        }

        /// <summary>
        ///     The selected view model changed so try to update the main contents
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

            InvalidateMeasure();
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

        private void OnHeaderTemplateChanged(IDataTemplate? newValue)
        {
            if (newValue is { } valid)
            {
                _headerPanel = valid.BuildVisual(DataContext);
                if (_headerPanel is { } hp)
                    AddChild(hp);
            }
        }

        private Boolean OnHeaderTemplateChanging(IDataTemplate? oldValue,
                                                 IDataTemplate? newValue)
        {
            if (_headerPanel is { } oldValid)
            {
                _children.Remove(oldValid);
                oldValid.Dispose();
            }

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
                    if (Equals(item, dc.DataContext))
                    {
                       SelectedItem = item;
                       if (sender is IBindableElement visual)
                          SelectedTab = visual;
                       break;
                    }

                 break;


              case nameof(IVisualElement.IsRequiresMeasure):
                 break;
           }
        }

        private void OnSelectedContentChanged(IVisualElement? obj)
        {
            InvalidateMeasure();
        }


        private void OnSelectedTabChanged(IVisualElement? newValue)
        {
            InvalidateMeasure();
        }

        private void OnTabItemsChanged(Object sender,
                                             NotifyCollectionChangedEventArgs e)
        {
            if (SelectedTab != null)
                return;

            SelectedTab = TabItems.FirstOrDefault();
            //SelectedTab = await TabItems.FirstOrDefaultAsync();
        }

        private readonly IDataTemplate _defaultItemTemplate;
        private readonly Object _lockContentVisuals;
        private readonly Dictionary<Object, IVisualElement> _tabContentVisuals;
        private readonly Object _updateLock;

        private IDataTemplate? _contentTemplate;
        private ValueSize _contentUses;

        private IVisualElement? _headerPanel;


        private IDataTemplate _headerTemplate;
        private ValueSize _headerUses;


        private IVisualElement? _selectedContent;


        private IVisualElement? _selectedTab;
    }
}