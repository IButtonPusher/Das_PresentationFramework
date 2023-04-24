using System;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Core.Geometry;
using Das.Views.Input;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;

namespace Das.Views.Panels;

// ReSharper disable once UnusedType.Global
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public partial class ScrollPanel : ContentPanel,
                                   IHandleInput<MouseWheelEventArgs>,
                                   IHandleInput<DragEventArgs>,
                                   IHandleInput<FlingEventArgs>,
                                   IHandleInput<MouseDownEventArgs>,
                                   IHandleInput<MouseUpEventArgs>,
                                   IFlingHost
{
   public ScrollPanel(IVisualBootstrapper visualBootstrapper)
      : base(visualBootstrapper)
   {
      _lastAvailable = ValueSize.Empty;
      _lastNeeded = ValueSize.Empty;

      _flingHandler = new FlingHandler(this);

      _isClipsContent = true;
   }


   public override void Arrange<TRenderSize>(TRenderSize availableSpace,
                                             IRenderContext renderContext)
   {
      if (!(Content is { } content))
         return;

      if (VerticalOffset == 0 && HorizontalOffset == 0)
      {
         base.Arrange(availableSpace, renderContext);
         return;
      }

      _maximumXScroll = IsScrollsHorizontal
         ? Convert.ToInt32(Math.Max(_lastNeeded.Width - availableSpace.Width, 0))
         : 0;

      if (VerticalOffset > _maximumYScroll)
         VerticalOffset = Convert.ToInt32(_maximumYScroll);

      if (HorizontalOffset > _maximumXScroll)
         HorizontalOffset = Convert.ToInt32(_maximumXScroll);

      var dest = new ValueRenderRectangle(
         0,
         0,
         availableSpace.Width + HorizontalOffset,
         availableSpace.Height + VerticalOffset,
         new ValuePoint2D(HorizontalOffset, VerticalOffset));

      renderContext.DrawElement(content, dest);
   }

   public override void Dispose()
   {
      base.Dispose();

      OnScrollTransitionStarting = null;
      OnScrollTransitionEnded = null;
   }


   public override ValueSize Measure<TRenderSize>(TRenderSize availableSpace,
                                                  IMeasureContext measureContext)
   {
      _lastAvailable = availableSpace;

      var h = IsScrollsVertical ? Double.PositiveInfinity : availableSpace.Height;
      var w = IsScrollsHorizontal ? Double.PositiveInfinity : availableSpace.Width;

      _lastNeeded = base.Measure(
         new ValueRenderSize(w, h, new ValuePoint2D(HorizontalOffset, VerticalOffset)),
         measureContext);


      _maximumYScroll = IsScrollsVertical
         ? Convert.ToInt32(Math.Max(_lastNeeded.Height - _lastAvailable.Height, 0))
         : 0;

      _maximumXScroll = IsScrollsHorizontal
         ? Convert.ToInt32(Math.Max(_lastNeeded.Width - _lastAvailable.Width, 0))
         : 0;


      var letsUseW = Math.Min(_lastNeeded.Width, availableSpace.Width);
      var letsUseH = Math.Min(_lastNeeded.Height, availableSpace.Height);
      return new ValueSize(letsUseW, letsUseH);
   }

   protected virtual Boolean OnScroll(Double deltaX,
                                      Double deltaY)
   {
      //Debug.WriteLine(Environment.TickCount + " scrolling Y " + deltaY + 
      //                " WAS: " + VerticalOffset + 
      //                " MAX: " + _maximumYScroll);

      var didScroll = false;
      Double nextScroll;

      if (deltaY.IsNotZero() && IsScrollsVertical)
      {
         nextScroll = GetValueBetween(VerticalOffset + deltaY, 0, _maximumYScroll);

         //Debug.WriteLine("next y: " + nextYScroll);

         //var nextYScroll = Math.Max(0, VerticalOffset + deltaY);

         if (nextScroll.AreDifferent(VerticalOffset))
         {
            if (nextScroll < 0)
               VerticalOffset = 0;
            else if (nextScroll >= _maximumYScroll)
            {
               VerticalOffset = _maximumYScroll;
               InvalidateMeasure();
            }
            else
               VerticalOffset = Convert.ToInt32(nextScroll);

            didScroll = true;
         }
         else
         {
            InvalidateMeasure();
         }
      }

      if (!deltaX.IsNotZero() || !IsScrollsHorizontal)
         return didScroll;

      nextScroll = GetValueBetween(HorizontalOffset + deltaX, 0, _maximumXScroll);
      //Math.Min(HorizontalOffset - deltaX, _maximumXScroll);

      //Debug.WriteLine($"hscroll {deltaX} offset was {HorizontalOffset} next {nextScroll}");

      if (nextScroll.AreDifferent(HorizontalOffset))
      {
         //var nextScroll = Math.Min(HorizontalOffset - deltaX, _maximumXScroll);
         if (nextScroll < 0)
            HorizontalOffset = 0;
         else if (nextScroll > _maximumXScroll)
            HorizontalOffset = _maximumXScroll;
         else
            HorizontalOffset = Convert.ToInt32(nextScroll);

         didScroll = true;
      }

      return didScroll;
   }

   private static Double GetValueBetween(Double value,
                                         Double min,
                                         Double max)
   {
      if (value <= min)
         return min;

      if (value >= max)
         return max;

      return value;
   }


   private void OnOffsetChanged(Int32 val)
   {
      InvalidateArrange();
   }

   public override Boolean IsClipsContent
   {
      get => _isClipsContent;
      set => _isClipsContent = value;
   }

   public override ValueRenderRectangle ArrangedBounds { get; set; }

   public Int32 HorizontalOffset
   {
      get => _horizontalOffset;
      set => SetValue(ref _horizontalOffset, value, OnOffsetChanged);
   }

   public Boolean IsScrollsHorizontal => (ScrollMode & ScrollMode.Horizontal) == ScrollMode.Horizontal;

   public Boolean IsScrollsVertical => (ScrollMode & ScrollMode.Vertical) == ScrollMode.Vertical;

   public Boolean IsScrollWithMouseDrag { get; set; }

   public ScrollMode ScrollMode
   {
      get => _scrollMode;
      // ReSharper disable once UnusedMember.Global
      set => SetValue(ref _scrollMode, value);
   }

   public Int32 VerticalOffset
   {
      get => _verticalOffset;
      set => SetValue(ref _verticalOffset, value, OnOffsetChanged);
   }

   /// <summary>
   ///    Allows for virtualizing panels to better plan ahead
   /// </summary>
   public event Action<Double, Double>? OnScrollTransitionStarting;

   public event Action? OnScrollTransitionEnded;

   private const Int32 _scrollCoefficient = 5;

   private const Double MOUSE_UP_MOVE_THRESHOLD = 10.0;

   private const Double MOUSE_CAPTURE_THRESHOLD = 50.0;
   private readonly FlingHandler _flingHandler;
   private Int32 _horizontalOffset;

   private IInputContext? _inputContext;

   private Boolean _isClipsContent;
   private ISize _lastAvailable;
   private ValueSize _lastNeeded;
   private Int32 _maximumXScroll;
   private Int32 _maximumYScroll;
   private ScrollMode _scrollMode;
   private Int32 _verticalOffset;
}