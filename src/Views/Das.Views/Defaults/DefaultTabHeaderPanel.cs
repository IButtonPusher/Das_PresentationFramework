using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.Views.Colors;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.ItemsControls;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;
using Das.Views.Styles;
using Das.Views.Styles.Transitions;
using Das.Views.Transitions;

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

         _indicator = new HorizontalRule(visualBootstrapper)
         {
            HorizontalAlignment = HorizontalAlignments.Left,
            Background = visualBootstrapper.ColorPalette.Secondary
         };
         MarginProperty.AddTransition(_indicator, new ThicknessTransition(_indicator,
            MarginProperty, TimeSpan.FromSeconds(0.3), TimeSpan.Zero, TransitionFunctionType.Ease));
         WidthProperty.AddTransition(_indicator, new QuantifiedDoubleTransition(_indicator,
            WidthProperty, TimeSpan.FromSeconds(0.3), TimeSpan.Zero, TransitionFunctionType.Ease));

         _separator = new HorizontalRule(visualBootstrapper)
         {
            Background = visualBootstrapper.ColorPalette.GetAlpha(ColorType.Background, .2)
         };

         Background = visualBootstrapper.ColorPalette.Background;

         var stackPanel = new UniformStackPanel(visualBootstrapper)
         {
            Orientation = Orientations.Horizontal
         };

         var sourcePropertyAccessor = visualBootstrapper.Properties.GetPropertyAccessor(
            tabControl.GetType(), nameof(tabControl.TabItems));

         var spBinding = new OneWayCollectionBinding(tabControl, nameof(tabControl.TabItems),
            stackPanel, nameof(Children), null, sourcePropertyAccessor);

         stackPanel.AddBinding(spBinding);

         _scrollPanel = new ScrollPanel(visualBootstrapper)
         {
            Content = stackPanel,
            ScrollMode = ScrollMode.Horizontal,
            IsScrollWithMouseDrag = true,
            VerticalAlignment = VerticalAlignments.Top
         };

         VerticalAlignment = VerticalAlignments.Top;

         AddChildren(_scrollPanel, _indicator, _separator);

         tabControl.PropertyChanged += OnTabPropertyChanged;
      }

      public override void Dispose()
      {
         _itemsControl.PropertyChanged -= OnTabPropertyChanged;
         base.Dispose();
      }

      public override void Arrange<TRenderSize>(TRenderSize availableSpace,
                                                IRenderContext renderContext)
      {
         // SCROLL PANEL
         var tabPageRect = new ValueRenderRectangle(
            tabsLeft, 0,
            availableSpace.Width,
            availableSpace.Height,
            availableSpace.Offset);

         // BOTTOM SEPARATOR
         var separatorRect = new ValueRenderRectangle(
            /* X */ tabsLeft,
            /* Y */ _tabsUsed.Height, //+ SEPARATOR_GAP_TOP,
            /* W */ _tabsUsed.Width,
            /* H */ SEPARATOR_LINE_HEIGHT,
            availableSpace.Offset);


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
               availableSpace.Height - INDICATOR_LINE_HEIGHT,
               _indicatord,
               indicatorOffset);

            renderContext.DrawElement(_indicator, indicatorRect);
         }

         renderContext.DrawElement(_separator, separatorRect);
         renderContext.DrawElement(_scrollPanel, tabPageRect);
      }

      public override ValueSize Measure<TRenderSize>(TRenderSize availableSpace,
                                                     IMeasureContext measureContext)
      {
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

         _indicatord = measureContext.MeasureElement(_indicator, indicatorRect.Size);


         return new ValueSize(_tabsUsed.Width, _tabsUsed.Height +
                                               INDICATOR_LINE_HEIGHT /*+
                                                  SEPARATOR_GAP_TOP +
                                                  SEPARATOR_GAP_BOTTOM*/);
      }

      private Boolean MoveIndicatorRect()
      {
         if (!(_itemsControl.SelectedTab is { } valid))
            return false;

         var pos = valid.ArrangedBounds;

         var x = pos.Left + _scrollPanel.HorizontalOffset - _scrollPanel.ArrangedBounds.Left;

         _indicatorRect = new Rectangle(x,
            0, pos.Size);

         _indicator.SuspendLayout();
         _indicator.Margin = new QuantifiedThickness(x, 0, 0, 0);
         _indicator.ResumeLayout();
         
         _indicator.Width = pos.Width;


         return true;
      }

      private void OnTabPropertyChanged(Object sender,
                                        PropertyChangedEventArgs e)
      {
         switch (e.PropertyName)
         {
            case nameof(ITabControl.SelectedTab): //when _lastStyleContext is { }:
               if (_itemsControl.SelectedTab is { })
                  MoveIndicatorRect();
               break;
         }
      }

      private const Int32 tabsLeft = 0;

      private const Int32 SEPARATOR_LINE_HEIGHT = 1;

      private const Int32 INDICATOR_LINE_HEIGHT = 3;

      private readonly HorizontalRule _indicator;

      private readonly ITabControl _itemsControl;
      private readonly ScrollPanel _scrollPanel;
      private readonly HorizontalRule _separator;
      private Size _indicatord;

      private Rectangle _indicatorRect;


      private Size _tabsUsed;
   }
}
