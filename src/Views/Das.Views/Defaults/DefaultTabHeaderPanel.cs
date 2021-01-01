﻿using System;
using System.ComponentModel;
using System.Threading.Tasks;
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
        public DefaultTabHeaderPanel(ITabControl tabControl,
                                     IVisualBootstrapper visualBootstrapper) :
            base(visualBootstrapper)
        {
            _indicatorRect = new RenderRectangle();
            _itemsControl = tabControl;
            _tabsUsed = Size.Empty;
            _indicatord = Size.Empty;

            _indicator = new HorizontalRule(visualBootstrapper);

            _styleRegistry = visualBootstrapper.StyleContext;

            var accent = _styleRegistry.ColorPalette.Accent;

            _separator = new HorizontalRule(visualBootstrapper);
            _styleRegistry.RegisterStyleSetter(_indicator,
                StyleSetterType.Transition, new[]
                {
                    new Transition(StyleSetterType.Margin, TimeSpan.FromSeconds(0.3),
                        TimeSpan.Zero, TransitionTiming.Ease),
                    new Transition(StyleSetterType.Width, TimeSpan.FromSeconds(0.3),
                        TimeSpan.Zero, TransitionTiming.Ease)
                });
            _styleRegistry.RegisterStyleSetter(_indicator,
                StyleSetterType.HorizontalAlignment, HorizontalAlignments.Left);

            _styleRegistry.RegisterStyleSetter(_indicator,
                StyleSetterType.Background, accent);
            _styleRegistry.RegisterStyleSetter(_separator,
                StyleSetterType.Background, SolidColorBrush.LightGray);

            Background = _styleRegistry.ColorPalette.Background;

            var stackPanel = new UniformStackPanel(visualBootstrapper)
            {
                Orientation = Orientations.Horizontal
            };

            var sourcePropertyAccessor = visualBootstrapper.GetPropertyAccessor(
                tabControl.GetType(), nameof(tabControl.TabItems));

            var spBinding = new OneWayCollectionBinding(tabControl, nameof(tabControl.TabItems),
                stackPanel, nameof(Children), null, sourcePropertyAccessor);

            stackPanel.AddBinding(spBinding);

            _scrollPanel = new ScrollPanel(visualBootstrapper)
            {
                Content = stackPanel,
                ScrollMode = ScrollMode.Horizontal,
                IsScrollWithMouseDrag = true
            };

            _scrollPanel.VerticalAlignment = VerticalAlignments.Top;
            VerticalAlignment = VerticalAlignments.Top;


            AddChildren(_scrollPanel, _indicator, _separator);

            tabControl.PropertyChanged += OnTabPropertyChanged;
        }

        public override void Arrange(IRenderSize availableSpace,
                                     IRenderContext renderContext)
        {
            _lastStyleContext = renderContext;
            _lastElementLocator = renderContext;

            // SCROLL PANEL
            var tabPageRect = new ValueRenderRectangle(
                tabsLeft, 0,
                availableSpace.Width,
                availableSpace.Height,
                new ValuePoint2D(0, 0));
            renderContext.DrawElement(_scrollPanel, tabPageRect);

            // BOTTOM SEPARATOR
            var separatorRect = new ValueRenderRectangle(
                /* X */ tabsLeft,
                /* Y */ _tabsUsed.Height + SEPARATOR_GAP_TOP,
                /* W */ _tabsUsed.Width,
                /* H */ SEPARATOR_LINE_HEIGHT,
                availableSpace.Offset);

            renderContext.DrawElement(_separator, separatorRect);


            // INDICATOR
            if (_indicatorRect.IsEmpty)
            {
                if (_itemsControl.SelectedTab != null)
                    if (MoveIndicatorRect())
                        InvalidateMeasure();
            }
            else
            {
                var indicatorOffset = new ValuePoint2D(availableSpace.Offset.X +
                                                       _scrollPanel.HorizontalOffset,
                    availableSpace.Offset.Y);

                var indicatorRect = new ValueRenderRectangle(
                    0 - _scrollPanel.HorizontalOffset,
                    availableSpace.Height - (SEPARATOR_GAP_BOTTOM + INDICATOR_LINE_HEIGHT),
                    _indicatord,
                    indicatorOffset);

                renderContext.DrawElement(_indicator, indicatorRect);
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
                                                  SEPARATOR_GAP_TOP +
                                                  SEPARATOR_GAP_BOTTOM);
        }

        private Boolean MoveIndicatorRect()
        {
            if (!(_itemsControl.SelectedTab is { } valid) ||
                !(_lastElementLocator?.TryGetLastRenderBounds(valid) is { } pos))
                return false;

            _indicatorRect = new Rectangle(pos.Left + _scrollPanel.HorizontalOffset, 0, pos.Size);

            if (!(_lastStyleContext is { })) 
                return false;
            
            _styleRegistry.RegisterStyleSetter(_indicator,
                StyleSetterType.Margin, new Thickness(pos.Left + _scrollPanel.HorizontalOffset,
                    0, 0, 0));
            _styleRegistry.RegisterStyleSetter(_indicator,
                StyleSetterType.Width, pos.Width);

            return true;
        }

        private void OnTabPropertyChanged(Object sender,
                                          PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ITabControl.SelectedTab) when _lastStyleContext is { }:
                    if (_itemsControl.SelectedTab is { })
                        MoveIndicatorRect();
                    break;
            }
        }

        private const Int32 tabsLeft = 0;

        private const Int32 SEPARATOR_LINE_HEIGHT = 1;
        private const Int32 INDICATOR_LINE_HEIGHT = 3;
        private const Int32 SEPARATOR_GAP_TOP = 10;
        private const Int32 SEPARATOR_GAP_BOTTOM = 5;
        private readonly HorizontalRule _indicator;

        private readonly ITabControl _itemsControl;
        private readonly ScrollPanel _scrollPanel;
        private readonly HorizontalRule _separator;
        private Size _indicatord;
        private Rectangle _indicatorRect;
        private IElementLocator? _lastElementLocator;
        private IStyleProvider? _lastStyleContext;


        private Size _tabsUsed;
        private readonly IStyleContext _styleRegistry;
    }
}