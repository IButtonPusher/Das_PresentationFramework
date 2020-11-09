using System;
using System.Threading.Tasks;
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
                                  IHandleInput<MouseDownEventArgs>
    {
        public ScrollPanel()
        {
            //_flingLock = new Object();
            _lastAvailable = Size.Empty;
            _lastNeeded = Size.Empty;

            _flingHandler = new FlingHandler(() => IsScrollsHorizontal,
                () => IsScrollsVertical, OnScroll);
        }

        public virtual Boolean OnInput(DragEventArgs args)
        {
            if (args.InputContext.IsMousePresent && !IsScrollWithMouseDrag)
                //todo: only use drag in a touch-only scenario?
                return false;

            OnScroll(0 - args.LastChange.Width, 0 - args.LastChange.Height);

            return true;
        }

        /// <summary>
        /// todo: possibly...
        /// </summary>
        public StyleSelector CurrentStyleSelector => StyleSelector.None;

        public Boolean OnInput(FlingEventArgs args)
        {
            return _flingHandler.OnInput(args);
        }


        public Boolean OnInput(MouseDownEventArgs args)
        {
            return _flingHandler.OnInput(args);
        }

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
            set => SetValue(ref _horizontalOffset, value);
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
            //Debug.WriteLine("Arranging scroll panel");

            if (!(Content is {} content))
                return;

            if (VerticalOffset == 0 && HorizontalOffset == 0)
            {
                base.Arrange(availableSpace, renderContext);
                return;
            }

            if (VerticalOffset > _maximumYScroll)
                VerticalOffset = Convert.ToInt32(_maximumYScroll);

            if (HorizontalOffset > _maximumXScroll)
                HorizontalOffset = Convert.ToInt32(_maximumXScroll);

            var dest = new ValueRenderRectangle(HorizontalOffset, 0 - VerticalOffset,
                availableSpace.Width, availableSpace.Height + VerticalOffset,
                new ValuePoint2D(HorizontalOffset, VerticalOffset));

            renderContext.DrawElement(content, dest);

            //Debug.WriteLine("Arranged scroll panel");
        }


        public override ISize Measure(IRenderSize availableSpace,
                                      IMeasureContext measureContext)
        {
            _lastAvailable = availableSpace;

            var h = IsScrollsVertical ? Double.PositiveInfinity : availableSpace.Height;
            var w = IsScrollsHorizontal ? Double.PositiveInfinity : availableSpace.Width;

            _lastNeeded = base.Measure(
                new ValueRenderSize(w, h,new ValuePoint2D(HorizontalOffset, VerticalOffset)), 
                measureContext);

            var res = new ValueSize(Math.Min(_lastNeeded.Width, availableSpace.Width),
                Math.Min(_lastNeeded.Height, availableSpace.Height));

            _maximumYScroll = IsScrollsVertical
                ? Math.Max(_lastNeeded.Height - _lastAvailable.Height, 0)
                : 0;

            _maximumXScroll = IsScrollsHorizontal
                ? Math.Max(_lastNeeded.Width - _lastAvailable.Width, 0)
                : 0;

            return res;
        }


        private void OnOffsetChanged(Int32 _)
        {
            //Debug.WriteLine("Scroll offset changed");

            IsChanged = true;
        }

        protected virtual void OnScroll(Double x,
                                        Double y)
        {
            if (x != 0 && IsScrollsHorizontal)

                HorizontalOffset = Convert.ToInt32(
                    Math.Min(
                        Math.Max(HorizontalOffset + x, 0),
                        _maximumXScroll));

            if (y != 0 && IsScrollsVertical)
                VerticalOffset = Convert.ToInt32(
                    Math.Min(
                        Math.Max(VerticalOffset + y, 0),
                        _maximumYScroll));
        }

        private const Int32 _scrollCoefficient = 5;
        private readonly FlingHandler _flingHandler;


        private Int32 _horizontalOffset;
        private ISize _lastAvailable;
        private ISize _lastNeeded;

        private Double _maximumXScroll;
        private Double _maximumYScroll;


        private ScrollMode _scrollMode;
        private Int32 _verticalOffset;
    }
}