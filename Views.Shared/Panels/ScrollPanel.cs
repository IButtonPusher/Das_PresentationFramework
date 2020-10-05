using System;
using Das.Views.Core.Geometry;
using Das.Views.Input;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    // ReSharper disable once UnusedType.Global
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class ScrollPanel<T> : ContentPanel<T>,
                                  IHandleInput<MouseWheelEventArgs>,
                                  IHandleInput<DragEventArgs>
    {
        public ScrollPanel()
        {
            _lastAvailable = Size.Empty;
            _lastNeeded = Size.Empty;
        }

        public virtual Boolean OnInput(DragEventArgs args)
        {
            if (args.InputContext.IsMousePresent && !IsScrollWithMouseDrag)
            {
                //todo: only use drag in a touch-only scenario?
                return false;
            }

            OnScroll(0 - args.LastChange.Width, 0 - args.LastChange.Height);

            return true;
        }

        protected virtual void OnScroll(Double x, Double y)
        {
            //System.Diagnostics.Debug.WriteLine("Scrolling: " + amount);

            if (x != 0 && IsScrollsHorizontal)
            {
                var proposed = Math.Max(HorizontalOffset + x, 0);
                var maximum = Math.Max(_lastNeeded.Width - _lastAvailable.Width, 0);
                if (proposed <= maximum)
                    HorizontalOffset = Convert.ToInt32(proposed);
            }

            if (y != 0 && IsScrollsVertical)
            {
                var proposed = Math.Max(VerticalOffset + y, 0);
                var maximum = Math.Max(_lastNeeded.Height - _lastAvailable.Height, 0);
                if (proposed <= maximum)
                    VerticalOffset = Convert.ToInt32(proposed);
            }
        }

        public InputAction HandlesActions => InputAction.MouseDrag | 
                                             InputAction.MouseWheel;


        public override void Dispose()
        {
            
        }

        public override ISize Measure(ISize availableSpace, 
                                      IMeasureContext measureContext)
        {
            _lastAvailable = availableSpace;

            var h = IsScrollsVertical ? Double.PositiveInfinity : availableSpace.Height; 
            var w = IsScrollsHorizontal ? Double.PositiveInfinity : availableSpace.Width;

            _lastNeeded = base.Measure(new ValueSize(w, h), measureContext);

            var res = new ValueSize(Math.Min(_lastNeeded.Width, availableSpace.Width),
                Math.Min(_lastNeeded.Height, availableSpace.Height));

            return res;
        }

        public override void Arrange(ISize availableSpace, 
                                     IRenderContext renderContext)
        {
            if (!(Content is {} content))
                return;

            if (VerticalOffset == 0 && HorizontalOffset == 0)
            {
                base.Arrange(availableSpace, renderContext);
                return;
            }

            var dest = new ValueRectangle(HorizontalOffset, 0 - VerticalOffset, 
                availableSpace.Width, availableSpace.Height + VerticalOffset);

            renderContext.DrawElement(content, dest);
        }

        public Boolean OnInput(MouseWheelEventArgs args)
        {
            OnScroll(0, args.Delta * _wheelCoefficient);
            return true;
        }


        private Int32 _horizontalOffset;

        public Int32 HorizontalOffset
        {
            get => _horizontalOffset;
            set => SetValue(ref _horizontalOffset, value);
        }

        private const Int32 _wheelCoefficient = 5;
        private Int32 _verticalOffset;
        private ISize _lastAvailable;
        private ISize _lastNeeded;

        public Int32 VerticalOffset
        {
            get => _verticalOffset;
            set => SetValue(ref _verticalOffset, value, OnOffsetChanged);
        }

        private void OnOffsetChanged(Int32 _)
        {
            IsChanged = true;
        }


        private ScrollMode _scrollMode;

        public ScrollMode ScrollMode
        {
            get => _scrollMode;
            // ReSharper disable once UnusedMember.Global
            set => SetValue(ref _scrollMode, value);
        }

        public Boolean IsScrollWithMouseDrag { get; set; }

        public Boolean IsScrollsVertical => (ScrollMode & ScrollMode.Vertical) == ScrollMode.Vertical;

        public Boolean IsScrollsHorizontal => (ScrollMode & ScrollMode.Horizontal) == ScrollMode.Horizontal;
    }
}
