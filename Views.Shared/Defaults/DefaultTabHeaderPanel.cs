using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
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
    public class DefaultTabHeaderPanel : BasePanel
    {
        
        public DefaultTabHeaderPanel(TabControl tabControl,
                                     IVisualBootStrapper visualBootStrapper) : 
            base(visualBootStrapper)
        {
            _indicatorRect = new RenderRectangle();
            _itemsControl = tabControl;
            _tabsUsed = Size.Empty;
            _indicatord = Size.Empty;

            _indicator = new HorizontalRule(visualBootStrapper);
            _lastStyleContext = visualBootStrapper.StyleContext;

            _separator = new HorizontalRule(visualBootStrapper);
            _lastStyleContext.RegisterStyleSetter(_indicator,
                StyleSetter.Transition, new[]
                {
                    new Transition(StyleSetter.Margin, TimeSpan.FromSeconds(0.3),
                        TimeSpan.Zero, TransitionTiming.Ease),
                    new Transition(StyleSetter.Width, TimeSpan.FromSeconds(0.3),
                        TimeSpan.Zero, TransitionTiming.Ease)
                });
            _lastStyleContext.RegisterStyleSetter(_indicator,
                StyleSetter.HorizontalAlignment, HorizontalAlignments.Left);

            _lastStyleContext.RegisterStyleSetter(_indicator,
                StyleSetter.Background, visualBootStrapper.StyleContext.GetCurrentAccentColor().ToBrush());
            _lastStyleContext.RegisterStyleSetter(_separator,
                StyleSetter.Background, SolidColorBrush.LightGray);

            Background = _lastStyleContext.ColorPalette.Background.ToBrush();

            var stackPanel = new StackPanel<Object>(visualBootStrapper)
            {
                Orientation = Orientations.Horizontal
            };
            stackPanel.AddBinding(new SourceBinding(tabControl, nameof(tabControl.TabItems),
                stackPanel, nameof(StackPanel<Object>.Children)));

            _scrollPanel = new ScrollPanel<Object>(visualBootStrapper)
            {
                Content = stackPanel,
                ScrollMode = ScrollMode.Horizontal,
                IsScrollWithMouseDrag = true
            };

            _scrollPanel.VerticalAlignment = VerticalAlignments.Top;

            //_scrollPanel.PropertyChanged += OnScrollPropertyChanged;

            AddChildren(_scrollPanel, _indicator, _separator);

            tabControl.PropertyChanged += OnTabPropertyChanged;
        }

        private void OnTabPropertyChanged(Object sender,
                                          PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(TabControl.SelectedTab) when _lastStyleContext is { } styleContext:
                    if (_itemsControl.SelectedTab is { } valid) MoveIndicatorRect();
                    break;
            }
        }

        public override ValueSize Measure(IRenderSize availableSpace,
                                          IMeasureContext measureContext)
        {
            _lastStyleContext = measureContext;

            if (!(_itemsControl is { } valid) ||
                !(valid.ItemsSource is { }))
                return base.Measure(availableSpace, measureContext);

            var tabsAvailable = new ValueRenderSize(availableSpace.Width - tabsLeft, availableSpace.Height,
                availableSpace.Offset);
            _tabsUsed = measureContext.MeasureElement(_scrollPanel, tabsAvailable);

            //_tabsUsed = _tabPageRenderer.Measure(valid, _itemsControl.TabItems,
            //    Orientations.Horizontal, availableSpace, measureContext);

            //_tabWidthDeficit = _tabsUsed.Width - availableSpace.Width;

            measureContext.MeasureElement(_separator,
                new ValueRenderSize(_indicatorRect.Width, SEPARATOR_LINE_HEIGHT,
                    availableSpace.Offset));

            var indicatorRect = new ValueRenderRectangle(0, 
                availableSpace.Height - INDICATOR_LINE_HEIGHT,
                _indicatorRect.Width, INDICATOR_LINE_HEIGHT, 
                availableSpace.Offset);

            _indicatord = measureContext.MeasureElement(_indicator, indicatorRect);


            return new ValueSize(_tabsUsed.Width, _tabsUsed.Height +
                                                  INDICATOR_LINE_HEIGHT +
                                                  //SEPARATOR_GAP_TOP + 
                                                  SEPARATOR_GAP_BOTTOM);
        }

        private const Int32 tabsLeft = 0;

        public override void Arrange(IRenderSize availableSpace,
                                     IRenderContext renderContext)
        {
            _lastStyleContext = renderContext;
            _lastElementLocator = renderContext;

            // SCROLL PANEL
            var tabPageRect = new ValueRenderRectangle(
                tabsLeft, 0,
                _tabsUsed.Width,
                _tabsUsed.Height,
                new ValuePoint2D(0, 0));
            renderContext.DrawElement(_scrollPanel, tabPageRect);

            // BOTTOM SEPARATOR
            var separatorRect = new ValueRenderRectangle(
                /* X */ tabsLeft,
                /* Y */ _tabsUsed.Height + SEPARATOR_GAP_TOP,  //availableSpace.Height - (SEPARATOR_GAP_BOTTOM + SEPARATOR_LINE_HEIGHT),
                /* W */ _tabsUsed.Width,
                /* H */ SEPARATOR_LINE_HEIGHT,
                availableSpace.Offset);

            renderContext.DrawElement(_separator,separatorRect);


            // INDICATOR
            if (_indicatorRect.IsEmpty)
            {
                if (_itemsControl.SelectedTab != null)
                {
                    MoveIndicatorRect();
                    InvalidateMeasure();
                }
            }
            else
            {
                var indicatorOffset = new ValuePoint2D(availableSpace.Offset.X +
                                                       _scrollPanel.HorizontalOffset,
                    availableSpace.Offset.Y);

                var indicatorRect = new ValueRenderRectangle(0 - _scrollPanel.HorizontalOffset, 
                    availableSpace.Height - (SEPARATOR_GAP_BOTTOM + INDICATOR_LINE_HEIGHT),
                    _indicatord, indicatorOffset);

                renderContext.DrawElement(_indicator, indicatorRect);
            }
        }

        private void MoveIndicatorRect()
        {
            //Debug.WriteLine("#Moving indicator rect");

            if (!(_itemsControl.SelectedTab is { } valid) ||
                //!_tabPageRenderer.TryGetRenderedElement(valid, out var pos))
                !(_lastElementLocator?.TryGetLastRenderBounds(valid) is {} pos))
                //Debug.WriteLine("#FAIL");
                return;

            _indicatorRect = new Rectangle(pos.Left + _scrollPanel.HorizontalOffset, 0, pos.Size);

            _lastStyleContext.RegisterStyleSetter(_indicator,
                StyleSetter.Margin, new Thickness(pos.Left + _scrollPanel.HorizontalOffset,
                    0, 0, 0));
            _lastStyleContext.RegisterStyleSetter(_indicator,
                StyleSetter.Width, pos.Width);

            //Debug.WriteLine("#Moved to " + _indicatorRect);
        }

        private const Int32 SEPARATOR_LINE_HEIGHT = 1;
        private const Int32 INDICATOR_LINE_HEIGHT = 3;
        private const Int32 SEPARATOR_GAP_TOP = 10;
        private const Int32 SEPARATOR_GAP_BOTTOM = 5;


        private Size _tabsUsed;

        private TabControl _itemsControl;
        private Size _indicatord;
        private IElementLocator? _lastElementLocator;
        private Rectangle _indicatorRect;
        private readonly ScrollPanel<Object> _scrollPanel;
        private readonly HorizontalRule _indicator;
        private IStyleProvider _lastStyleContext;
        private readonly HorizontalRule _separator;
    }
}
