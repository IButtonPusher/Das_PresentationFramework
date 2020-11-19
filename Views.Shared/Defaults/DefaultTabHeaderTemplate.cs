using System;
using System.ComponentModel;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.ItemsControls;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;
using Das.Views.Styles;

namespace Das.Views.Defaults
{
    public class DefaultTabHeaderTemplate : DefaultContentTemplate
    {
        public DefaultTabHeaderTemplate(IVisualBootStrapper templateResolver,
                                        TabControl tabControl) 
            : base(templateResolver, tabControl)
        {
            _itemsControl = tabControl;
            tabControl.PropertyChanged += OnTabPropertyChanged;
            _indicator = new HorizontalRule(templateResolver);
            _indicatorRect = new RenderRectangle();
            _tabsUsed =  Size.Empty;
            _indicatord = Size.Empty;

            _lastStyleContext = templateResolver.StyleContext;
            _tabPageRenderer = new SequentialRenderer();
            _separator = new HorizontalRule(templateResolver);
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
                StyleSetter.Background, templateResolver.StyleContext.GetCurrentAccentColor().ToBrush());
            _lastStyleContext.RegisterStyleSetter(_separator,
                StyleSetter.Background, SolidColorBrush.LightGray);
        }

        private void OnTabPropertyChanged(Object sender, 
                                          PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(TabControl.SelectedTab) when _lastStyleContext is {} styleContext:
                    if (_itemsControl.SelectedTab is { } valid)
                    {
                        MoveIndicatorRect();
                        //if (_tabPageRenderer.ElementsRendered.TryGetValue(valid, out var pos))
                        //{
                        //    _indicatorRect = new Rectangle(pos.Left, 0, pos.Size);

                        //    styleContext.RegisterStyleSetter(_indicator,
                        //        StyleSetter.Margin, new Thickness(pos.Left, 0, 0, 0));
                        //    styleContext.RegisterStyleSetter(_indicator,
                        //        StyleSetter.Width, pos.Width);
                        //}
                    }
                    break;
            }
        }

        private void MoveIndicatorRect()
        {
            if (!(_itemsControl.SelectedTab is { } valid) ||
                !_tabPageRenderer.ElementsRendered.TryGetValue(valid, out var pos))
                return;

            _indicatorRect = new Rectangle(pos.Left, 0, pos.Size);

            _lastStyleContext.RegisterStyleSetter(_indicator,
                StyleSetter.Margin, new Thickness(pos.Left, 0, 0, 0));
            _lastStyleContext.RegisterStyleSetter(_indicator,
                StyleSetter.Width, pos.Width);
        }

        public override ValueSize Measure(IRenderSize availableSpace, 
                                          IMeasureContext measureContext)
        {
            _lastStyleContext = measureContext;

            if (!(_itemsControl is { } valid) ||
                !(valid.ItemsSource is {}))
                return base.Measure(availableSpace, measureContext);

            _tabsUsed = _tabPageRenderer.Measure(valid, _itemsControl.TabItems,
                Orientations.Horizontal, availableSpace, measureContext);

            measureContext.MeasureElement(_separator,
                new ValueRenderSize(_indicatorRect.Width, SEPARATOR_LINE_HEIGHT,
                    availableSpace.Offset));

            var indicatorRect = new ValueRenderRectangle(0, availableSpace.Height - INDICATOR_LINE_HEIGHT,
                _indicatorRect.Width, INDICATOR_LINE_HEIGHT, availableSpace.Offset);

            _indicatord = measureContext.MeasureElement(_indicator, indicatorRect);
               

            return new ValueSize(_tabsUsed.Width, _tabsUsed.Height +
                                                  INDICATOR_LINE_HEIGHT +
                                                 //SEPARATOR_GAP_TOP + 
                                                 SEPARATOR_GAP_BOTTOM);
        }

        public override void Arrange(IRenderSize availableSpace, 
                                     IRenderContext renderContext)
        {
            _lastStyleContext = renderContext;

            var tabPageRect = new ValueRenderRectangle(0, 0, availableSpace.Width,
                availableSpace.Height - (
                    //SEPARATOR_GAP_TOP +
                    INDICATOR_LINE_HEIGHT +
                    SEPARATOR_GAP_BOTTOM), availableSpace.Offset);

            _tabPageRenderer.Arrange(Orientations.Horizontal, tabPageRect, renderContext);

            renderContext.DrawElement(_separator, 
                new ValueRenderRectangle(0, 
                    availableSpace.Height - (SEPARATOR_GAP_BOTTOM + SEPARATOR_LINE_HEIGHT),
                    availableSpace.Width, SEPARATOR_LINE_HEIGHT, availableSpace.Offset));

            var useRect = new ValueRenderRectangle(0, 
                availableSpace.Height - (SEPARATOR_GAP_BOTTOM + INDICATOR_LINE_HEIGHT),
                _indicatord, availableSpace.Offset);


            if (_indicatorRect.IsEmpty)
            {
                if (_itemsControl.SelectedTab != null)
                {
                    MoveIndicatorRect();
                    InvalidateMeasure();
                }
            }
            else
                renderContext.DrawElement(_indicator, useRect);

            
        }

        public override void OnParentChanging(IContainerVisual? newParent)
        {
            base.OnParentChanging(newParent);
            _itemsControl = newParent as TabControl ?? throw new InvalidOperationException();
        }

        private IStyleProvider _lastStyleContext;
        private TabControl _itemsControl;
        private readonly SequentialRenderer _tabPageRenderer;
        private readonly HorizontalRule _separator;
        private readonly HorizontalRule _indicator;
        private Rectangle _indicatorRect;
        private Size _tabsUsed;
        private Size _indicatord;

        private const Int32 SEPARATOR_LINE_HEIGHT = 1;
        private const Int32 INDICATOR_LINE_HEIGHT = 3;
        private const Int32 SEPARATOR_GAP_TOP = 10;
        private const Int32 SEPARATOR_GAP_BOTTOM = 5;
    }
}
