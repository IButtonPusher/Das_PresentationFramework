using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Core.Geometry;
using Das.Views.Input;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;
using Das.Views.Styles;

namespace Das.Views.Panels
{
    // ReSharper disable once UnusedType.Global
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class ScrollPanel : ContentPanel,
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


        Boolean IFlingHost.CanFlingVertical => IsScrollsVertical;

        Boolean IFlingHost.CanFlingHorizontal => IsScrollsHorizontal;

        Double IFlingHost.CurrentX => HorizontalOffset;

        Double IFlingHost.CurrentY => VerticalOffset;

        public ValueMinMax GetVerticalMinMaxFling()
        {
            return IsScrollsVertical
                ? new ValueMinMax(0 - VerticalOffset, _maximumYScroll - VerticalOffset)
                : ValueMinMax.Empty;
        }

        public ValueMinMax GetHorizontalMinMaxFling()
        {
            return IsScrollsHorizontal
                ? new ValueMinMax(0 - HorizontalOffset, _maximumXScroll - HorizontalOffset)
                : ValueMinMax.Empty;
        }


        void IFlingHost.OnFlingStarting(Double totalHorizontalChange,
                                        Double totalVerticalChange)
        {
            if (totalVerticalChange.IsNotZero())
                Debug.WriteLine("**Starting fling when y scroll: " + VerticalOffset +
                                " delta: " + totalVerticalChange);

            if (totalHorizontalChange.IsNotZero())
                Debug.WriteLine("**Starting fling when x scroll: " + HorizontalOffset +
                                " delta: " + totalHorizontalChange);

            OnScrollTransitionStarting?.Invoke(totalHorizontalChange, totalVerticalChange);
        }

        void IFlingHost.OnFlingStep(Double deltaHorizontal,
                                    Double deltaVertical)
        {
            //Debug.WriteLine("fling step x: " + deltaHorizontal + " y: " + deltaVertical);
            
            OnScroll(deltaHorizontal, deltaVertical);
            
            //Debug.WriteLine("after step x: " + HorizontalOffset + " y: " + VerticalOffset);
        }

        public void OnFlingEnded(Boolean wasCancelled)
        {
            Debug.WriteLine("***end of fling v-offset: " + VerticalOffset + 
                            " h: " + HorizontalOffset + "***");

            if (_inputContext is { } inputContext)
                inputContext.TryReleaseMouseCapture(this);

            OnScrollTransitionEnded?.Invoke();
        }

        public virtual Boolean OnInput(DragEventArgs args)
        {
            //Debug.WriteLine("scroll Handle drag: " + args.LastChange.Height);

            if (args.InputContext.IsMousePresent && !IsScrollWithMouseDrag)
                //todo: only use drag in a touch-only scenario?
                return false;

            var isCapture = (IsScrollsHorizontal && Math.Abs(args.TotalDragged.X) > MOUSE_CAPTURE_THRESHOLD) ||
                            (IsScrollsVertical && Math.Abs(args.TotalDragged.Y) > MOUSE_CAPTURE_THRESHOLD);

            if (isCapture)
            {
                _inputContext = args.InputContext;
                _inputContext.TryCaptureMouseInput(this);
            }

            // use the opposites here because if we are dragging, for example,
            // to the right, that's a +X but we want the scroll to decrease
            var res = OnScroll(0 - args.LastChange.Width,
                0 - args.LastChange.Height);

            return res;
        }

        public Boolean OnInput(FlingEventArgs args)
        {
            if (!IsScrollWithMouseDrag)
                return false;

            var working = _flingHandler.OnInput(args);

            if (working)
                args.InputContext.TryCaptureMouseInput(this);

            return working;
        }


        public Boolean OnInput(MouseDownEventArgs args)
        {
            return IsScrollWithMouseDrag && _flingHandler.OnInput(args);
        }

        public Boolean OnInput(MouseUpEventArgs args)
        {
            if (args.PositionWentDown == null ||
                args.InputContext.GetVisualWithMouseCapture() != this)
                return false;


            args.InputContext.TryReleaseMouseCapture(this);
            
            if (IsScrollsHorizontal)
            {
                var diffX = args.PositionWentDown.X - args.Position.X;
                if (Math.Abs(diffX) >= MOUSE_UP_MOVE_THRESHOLD)
                    return true;
            }

            if (IsScrollsVertical)
            {
                var diffY = args.PositionWentDown.Y - args.Position.Y;
                if (Math.Abs(diffY) >= MOUSE_UP_MOVE_THRESHOLD)
                    return true;
            }

            
            return false;
        }

        /// <summary>
        ///     todo: possibly...
        /// </summary>
        public VisualStateType CurrentVisualStateType => VisualStateType.None;

        public InputAction HandlesActions => InputAction.MouseDrag |
                                             InputAction.MouseWheel |
                                             InputAction.Fling;

        public Boolean OnInput(MouseWheelEventArgs args)
        {
            OnScroll(0, args.Delta * _scrollCoefficient);
            return true;
        }

        public Int32 HorizontalOffset
        {
            get => _horizontalOffset;
            set => SetValue(ref _horizontalOffset, value, OnHorzOffsetChanged);
        }

        public override Boolean IsClipsContent
        {
            get => _isClipsContent;
            set => _isClipsContent = value;
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

        public override void Arrange(IRenderSize availableSpace,
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
                0, //HorizontalOffset, 
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


        public override ValueSize Measure(IRenderSize availableSpace,
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


            //return _lastNeeded;
        }

        protected virtual Boolean OnScroll(Double deltaX,
                                           Double deltaY)
        {
            //Debug.WriteLine(Environment.TickCount + " scrolling Y " + deltaY + " WAS: " + VerticalOffset);

            var didScroll = false;

            if (deltaY.IsNotZero() && IsScrollsVertical)
            {
                var nextYScroll = GetValueBetween(VerticalOffset + deltaY, 0, _maximumYScroll);

                //var nextYScroll = Math.Max(0, VerticalOffset + deltaY);

                if (nextYScroll.AreDifferent(VerticalOffset))
                {
                    if (nextYScroll < 0)
                        VerticalOffset = 0;
                    else if (nextYScroll > _maximumYScroll)
                        VerticalOffset = _maximumYScroll;
                    else
                        VerticalOffset = Convert.ToInt32(nextYScroll);

                    didScroll = true;
                }
            }

            if (!deltaX.IsNotZero() || !IsScrollsHorizontal)
                return didScroll;

            var nextScroll = GetValueBetween(HorizontalOffset + deltaX, 0, _maximumXScroll);
            //Math.Min(HorizontalOffset - deltaX, _maximumXScroll);

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

        private void OnHorzOffsetChanged(Int32 obj)
        {
            //Debug.WriteLine("horz offset is now " + obj);

            InvalidateMeasure();
        }


        private void OnOffsetChanged(Int32 val)
        {
            //Debug.WriteLine("vert offset is now " + val);

            InvalidateArrange();
            //InvalidateMeasure();
        }

        /// <summary>
        ///     Allows for virtualizing panels to better plan ahead
        /// </summary>
        public event Action<Double, Double>? OnScrollTransitionStarting;

        public event Action? OnScrollTransitionEnded;

        private const Int32 _scrollCoefficient = 5;
        private readonly FlingHandler _flingHandler;
        private Int32 _horizontalOffset;

        private Boolean _isClipsContent;
        private ISize _lastAvailable;
        private ValueSize _lastNeeded;
        private Int32 _maximumXScroll;
        private Int32 _maximumYScroll;
        private ScrollMode _scrollMode;
        private Int32 _verticalOffset;

        private IInputContext? _inputContext;

        private const Double MOUSE_UP_MOVE_THRESHOLD = 10.0;

        private const Double MOUSE_CAPTURE_THRESHOLD = 50.0;
    }
}