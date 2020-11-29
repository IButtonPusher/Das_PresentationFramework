using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Input;
using Das.Views.ItemsControls;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;
using Das.Views.Styles;

namespace Das.Views.Defaults
{
    public class DefaultTabHeaderTemplate : DefaultContentTemplate
        //, IHandleInput<DragEventArgs>
    {
        public DefaultTabHeaderTemplate(IVisualBootstrapper visualBootstrapper,
                                        ITabControl tabControl)
            : base(visualBootstrapper, tabControl)
        {
            _visualBootstrapper = visualBootstrapper;
            _itemsControl = tabControl;
            //tabControl.PropertyChanged += OnTabPropertyChanged;
            //_indicator = new HorizontalRule(visualBootStrapper);
            _indicatorRect = new RenderRectangle();
            _tabsUsed = Size.Empty;
            _indicatord = Size.Empty;

            _lastStyleContext = visualBootstrapper.StyleContext;
            //_tabPageRenderer = new SequentialRenderer();
            //_separator = new HorizontalRule(visualBootStrapper);
            //_lastStyleContext.RegisterStyleSetter(_indicator,
            //    StyleSetter.Transition, new[]
            //    {
            //        new Transition(StyleSetter.Margin, TimeSpan.FromSeconds(0.3),
            //            TimeSpan.Zero, TransitionTiming.Ease),
            //        new Transition(StyleSetter.Width, TimeSpan.FromSeconds(0.3),
            //            TimeSpan.Zero, TransitionTiming.Ease)
            //    });
            //_lastStyleContext.RegisterStyleSetter(_indicator,
            //    StyleSetter.HorizontalAlignment, HorizontalAlignments.Left);

            //_lastStyleContext.RegisterStyleSetter(_indicator,
            //    StyleSetter.Background, visualBootStrapper.StyleContext.GetCurrentAccentColor().ToBrush());
            //_lastStyleContext.RegisterStyleSetter(_separator,
            //    StyleSetter.Background, SolidColorBrush.LightGray);

            //var stackPanel = new StackPanel<Object>(visualBootStrapper)
            //{
            //    Orientation = Orientations.Horizontal
            //};
            //stackPanel.AddBinding(new SourceBinding(tabControl, nameof(tabControl.TabItems),
            //    stackPanel, nameof(StackPanel<Object>.Children)));

            //_scrollPanel = new ScrollPanel<Object>(visualBootStrapper)
            //{
            //    Content = stackPanel,
            //    ScrollMode = ScrollMode.Horizontal,
            //    IsScrollWithMouseDrag = true
            //};

            //_scrollPanel.VerticalAlignment = VerticalAlignments.Top;

            //_scrollPanel.PropertyChanged += OnScrollPropertyChanged;
        }

        public override IVisualElement? BuildVisual(Object? dataContext)
        {
            return new DefaultTabHeaderPanel(_itemsControl, _visualBootstrapper);
        }

        private void OnScrollPropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ScrollPanel<Object>.HorizontalOffset):
                case nameof(ScrollPanel<Object>.VerticalOffset):
                    _itemsControl.InvalidateMeasure();
                    //InvalidateMeasure();
                    break;

                //case nameof(IChangeTracking.IsChanged) when sender is IChangeTracking changer &&
                //    changer.IsChanged:
                //case nameof(IsRequiresMeasure) when sender is IVisualRenderer renderer &&
                //                                    renderer.IsRequiresMeasure:
                //    InvalidateMeasure();
                //    break;

                //case nameof(IsRequiresArrange) when sender is IVisualRenderer renderer &&
                //                                    renderer.IsRequiresArrange:
                //    InvalidateArrange();
                    
                //    break;
            }
        }

        //public override Boolean IsRequiresArrange 
        //{
        //    get
        //    {
        //        var res = base.IsRequiresArrange || _scrollPanel.IsRequiresArrange ||
        //                  _indicator.IsRequiresArrange || _separator.IsRequiresArrange;

        //        return res;
        //    }
        //    protected set => base.IsRequiresArrange = value;
        //}

        //public override Boolean IsRequiresMeasure
        //{
        //    get
        //    {
        //        var res = base.IsRequiresMeasure || _scrollPanel.IsRequiresMeasure ||
        //              _indicator.IsRequiresMeasure || _separator.IsRequiresMeasure;

        //        return res;
        //    }
        //    protected set => base.IsRequiresMeasure = value;
        //}

        public StyleSelector CurrentStyleSelector => StyleSelector.None;

        public InputAction HandlesActions => InputAction.MouseDrag;

        private const Int32 tabsLeft = 0;

        //public override void Arrange(IRenderSize availableSpace,
        //                             IRenderContext renderContext)
        //{
        //    _lastStyleContext = renderContext;
        //    _lastElementLocator = renderContext;

        //    // SCROLL PANEL
        //    var tabPageRect = new ValueRenderRectangle(
        //        tabsLeft, 0,
        //        _tabsUsed.Width,
        //        _tabsUsed.Height,
        //        new ValuePoint2D(0, 0));
        //    renderContext.DrawElement(_scrollPanel, tabPageRect);

        //    // BOTTOM SEPARATOR
        //    var separatorRect = new ValueRenderRectangle(
        //        /* X */ tabsLeft,
        //        /* Y */ _tabsUsed.Height + SEPARATOR_GAP_TOP,  //availableSpace.Height - (SEPARATOR_GAP_BOTTOM + SEPARATOR_LINE_HEIGHT),
        //        /* W */ _tabsUsed.Width,
        //        /* H */ SEPARATOR_LINE_HEIGHT,
        //        availableSpace.Offset);

        //    renderContext.DrawElement(_separator,separatorRect);


        //    // INDICATOR
        //    if (_indicatorRect.IsEmpty)
        //    {
        //        if (_itemsControl.SelectedTab != null)
        //        {
        //            MoveIndicatorRect();
        //            InvalidateMeasure();
        //        }
        //    }
        //    else
        //    {
        //        var indicatorOffset = new ValuePoint2D(availableSpace.Offset.X +
        //                                               _scrollPanel.HorizontalOffset,
        //            availableSpace.Offset.Y);

        //        var indicatorRect = new ValueRenderRectangle(0 - _scrollPanel.HorizontalOffset, 
        //            availableSpace.Height - (SEPARATOR_GAP_BOTTOM + INDICATOR_LINE_HEIGHT),
        //            _indicatord, indicatorOffset);

        //        renderContext.DrawElement(_indicator, indicatorRect);
        //    }
        //}

        //public override ValueSize Measure(IRenderSize availableSpace,
        //                                  IMeasureContext measureContext)
        //{
        //    _lastStyleContext = measureContext;

        //    if (!(_itemsControl is { } valid) ||
        //        !(valid.ItemsSource is { }))
        //        return base.Measure(availableSpace, measureContext);

        //    var tabsAvailable = new ValueRenderSize(availableSpace.Width - tabsLeft, availableSpace.Height,
        //        availableSpace.Offset);
        //    _tabsUsed = measureContext.MeasureElement(_scrollPanel, tabsAvailable);

        //    //_tabsUsed = _tabPageRenderer.Measure(valid, _itemsControl.TabItems,
        //    //    Orientations.Horizontal, availableSpace, measureContext);

        //    //_tabWidthDeficit = _tabsUsed.Width - availableSpace.Width;

        //    measureContext.MeasureElement(_separator,
        //        new ValueRenderSize(_indicatorRect.Width, SEPARATOR_LINE_HEIGHT,
        //            availableSpace.Offset));

        //    var indicatorRect = new ValueRenderRectangle(0, 
        //        availableSpace.Height - INDICATOR_LINE_HEIGHT,
        //        _indicatorRect.Width, INDICATOR_LINE_HEIGHT, 
        //        availableSpace.Offset);

        //    _indicatord = measureContext.MeasureElement(_indicator, indicatorRect);


        //    return new ValueSize(_tabsUsed.Width, _tabsUsed.Height +
        //                                          INDICATOR_LINE_HEIGHT +
        //                                          //SEPARATOR_GAP_TOP + 
        //                                          SEPARATOR_GAP_BOTTOM);
        //}

        public override void OnParentChanging(IContainerVisual? newParent)
        {
            base.OnParentChanging(newParent);
            _itemsControl = newParent as ITabControl ?? throw new InvalidOperationException();
        }


        //private void MoveIndicatorRect()
        //{
        //    //Debug.WriteLine("#Moving indicator rect");

        //    if (!(_itemsControl.SelectedTab is { } valid) ||
        //        //!_tabPageRenderer.TryGetRenderedElement(valid, out var pos))
        //        !(_lastElementLocator?.TryGetLastRenderBounds(valid) is {} pos))
        //        //Debug.WriteLine("#FAIL");
        //        return;

        //    _indicatorRect = new Rectangle(pos.Left + _scrollPanel.HorizontalOffset, 0, pos.Size);

        //    _lastStyleContext.RegisterStyleSetter(_indicator,
        //        StyleSetter.Margin, new Thickness(pos.Left + _scrollPanel.HorizontalOffset,
        //            0, 0, 0));
        //    _lastStyleContext.RegisterStyleSetter(_indicator,
        //        StyleSetter.Width, pos.Width);

        //    //Debug.WriteLine("#Moved to " + _indicatorRect);
        //}

        //private void OnTabPropertyChanged(Object sender,
        //                                  PropertyChangedEventArgs e)
        //{
        //    switch (e.PropertyName)
        //    {
        //        case nameof(TabControl.SelectedTab) when _lastStyleContext is { } styleContext:
        //            if (_itemsControl.SelectedTab is { } valid) MoveIndicatorRect();
        //            break;
        //    }
        //}

        private const Int32 SEPARATOR_LINE_HEIGHT = 1;
        private const Int32 INDICATOR_LINE_HEIGHT = 3;
        private const Int32 SEPARATOR_GAP_TOP = 10;
        private const Int32 SEPARATOR_GAP_BOTTOM = 5;
        //private readonly HorizontalRule _indicator;
        //private readonly HorizontalRule _separator;
        //private readonly SequentialRenderer _tabPageRenderer;
        private Size _indicatord;
        private Rectangle _indicatorRect;
        private readonly IVisualBootstrapper _visualBootstrapper;
        private ITabControl _itemsControl;

        private IStyleProvider _lastStyleContext;
        //private IElementLocator? _lastElementLocator;

        private Size _tabsUsed;
        
        //private readonly ScrollPanel<Object> _scrollPanel;
    }
}