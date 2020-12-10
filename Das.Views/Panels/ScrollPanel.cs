﻿using System;
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
    public class ScrollPanel<T> : ContentPanel<T>,
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
            OnScroll(deltaHorizontal, deltaVertical);
        }

        public void OnFlingEnded(Boolean wasCancelled)
        {
            Debug.WriteLine("***end of fling v-offset: " + VerticalOffset + "***");

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

            OnScroll(args.LastChange.Width, 0 - args.LastChange.Height);

            return true;
        }

        public Boolean OnInput(FlingEventArgs args)
        {
            if (!IsScrollWithMouseDrag)
                return false;

            var working = _flingHandler.OnInput(args);

            //if (working)
            //{
            //    _inputContext = args.InputContext;
            //    _inputContext.TryCaptureMouseInput(this);
            //}

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

            //Debug.WriteLine("mouse up: " + args.Position);
            return false;
        }

        /// <summary>
        ///     todo: possibly...
        /// </summary>
        public StyleSelector CurrentStyleSelector => StyleSelector.None;

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

            //GetMeasureSpace(measureContext, availableSpace,
            //    out _,
            //    out var mySize);

            return _lastNeeded;

            //return _lastNeeded;
        }

        protected virtual void OnScroll(Double deltaX,
                                        Double deltaY)
        {
            if (deltaY.IsNotZero() && IsScrollsVertical)
            {
                var nextYScroll = VerticalOffset + deltaY;
                if (VerticalOffset < 0)
                    VerticalOffset = 0;
                else if (nextYScroll > _maximumYScroll)
                    VerticalOffset = _maximumYScroll;
                else
                    VerticalOffset = Convert.ToInt32(nextYScroll);
            }

            if (!deltaX.IsNotZero() || !IsScrollsHorizontal)
                return;

            var nextScroll = HorizontalOffset - deltaX;
            if (nextScroll < 0)
                HorizontalOffset = 0;
            else if (nextScroll > _maximumXScroll)
                HorizontalOffset = _maximumXScroll;
            else
                HorizontalOffset = Convert.ToInt32(nextScroll);

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