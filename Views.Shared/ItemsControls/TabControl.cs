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
    public class TabControl : SelectorVisual,
                              IContentPresenter,
                              IItemsControl

    {
        private readonly IVisualBootStrapper _visualBootStrapper;

        public TabControl(IVisualBootStrapper visualBootStrapper)
        : base(visualBootStrapper)
        {
            _headerUses = ValueSize.Empty;
            _contentUses =ValueSize.Empty;
            _visualBootStrapper = visualBootStrapper;
            
            TabItems = new AsyncObservableCollection2<IVisualElement>();
            
            HeaderTemplate = new DefaultTabHeaderTemplate(_visualBootStrapper, this);
            _headerTemplate = HeaderTemplate;
            _itemTemplate = new DefaultTabItemTemplate(visualBootStrapper);
            _contentTemplate = new DefaultContentTemplate(visualBootStrapper, this);

            
        }


        public override ValueSize Measure(IRenderSize availableSpace, 
                                          IMeasureContext measureContext)
        {
            if (!(_headerPanel is {} header))
                return ValueSize.Empty;

            _headerUses = measureContext.MeasureElement(header, availableSpace);
            var remaining = availableSpace.Minus(_headerUses);

            if (SelectedTab is { } visual)
            {
                //TODO:
                _contentUses = measureContext.MeasureElement(visual, remaining);
                //return _headerUses.PlusVertical(_contentUses);
            }
            else
            {
                _contentUses = ValueSize.Empty;
                return _headerUses;
            }

            return _headerUses;
            
        }

        public override void Arrange(IRenderSize availableSpace, 
                                     IRenderContext renderContext)
        {
            if (!(_headerPanel is {} header))
                return;

            renderContext.DrawElement(header,
                new ValueRenderRectangle(0, 0, _headerUses, availableSpace.Offset));

            //TODO: 
            //if (SelectedTab is { } content)
            //{
            //    renderContext.DrawElement(content,
            //        new ValueRenderRectangle(0, _headerUses.Height, _contentUses,
            //            availableSpace.Offset));
            //}
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
                value ?? new DefaultTabItemTemplate(_visualBootStrapper));
        }


        private IDataTemplate _headerTemplate;

        public IDataTemplate? HeaderTemplate
        {
            get => _headerTemplate;
            set => SetValue(ref _headerTemplate, 
                value ?? new DefaultTabHeaderTemplate(_visualBootStrapper, this),
                OnHeaderTemplateChanging, OnHeaderTemplateChanged);
        }

        private void OnHeaderTemplateChanged(IDataTemplate? newValue)
        {
            if (newValue is { } valid)
            {
                _headerPanel = valid.BuildVisual(DataContext);
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

            InvalidateMeasure();

            return true;
        }

        private void OnItemsChanged(Object sender, 
                                    NotifyCollectionChangedEventArgs e)
        {
            e.HandleCollectionChanges<Object>(RemoveOldItems, AddNewItems, ClearTabs);
            InvalidateMeasure();
        }

        

        private async void AddNewItems(IEnumerable<Object> items)
        {
            foreach (var item in items)
            {
                var visual = _itemTemplate.BuildVisual(item) ?? throw new NullReferenceException();

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

                case nameof(IVisualElement.IsRequiresMeasure):
                    break;

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

        private void ClearTabs()
        {
            TabItems.Clear();
        }


        public override IVisualElement? SelectedVisual
        {
            get => SelectedTab;
            set => SelectedTab = value;
        }

        protected override void OnSelectedItemChanged(ISelector selector,
                                                      Object? oldValue,
                                                      Object? newValue)
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
        }

        protected override Boolean OnSelectedItemChanging(ISelector selector,
            Object? oldValue, 
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


        //public override void InvalidateMeasure()
        //{
        //    base.InvalidateMeasure();

        //    if (SelectedTab is {} tab)
        //        tab.InvalidateMeasure();

        //    _headerTemplate.Template.InvalidateMeasure();

        //    var _ = TabItems.RunOnEach(v => v.InvalidateMeasure()).ConfigureAwait(false);
        //}

        //public override Boolean IsRequiresMeasure
        //{
        //    get => base.IsRequiresMeasure || _headerTemplate.Template.IsRequiresMeasure;
        //    protected set => base.IsRequiresMeasure = value;
        //}

        //public override Boolean IsRequiresArrange
        //{
        //    get => base.IsRequiresArrange || _headerTemplate.Template.IsRequiresArrange;
        //    protected set => base.IsRequiresArrange = value;
        //}

        private IVisualElement? _selectedTab;

        public IVisualElement? SelectedTab
        {
            get => _selectedTab;
            set => SetValue(ref _selectedTab, value, OnSelectedTabChanged);
        }

        private void OnSelectedTabChanged(IVisualElement? newValue)
        {
            Debug.WriteLine("selected tab changed: " + newValue);
            InvalidateMeasure();
        }

        public AsyncObservableCollection2<IVisualElement> TabItems { get; }




        private IVisualElement? _headerPanel;
        private ValueSize _headerUses;
        private ValueSize _contentUses;
    }
}
