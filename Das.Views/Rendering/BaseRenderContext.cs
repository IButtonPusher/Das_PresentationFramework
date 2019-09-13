using System;
using System.Collections.Generic;
using System.Linq;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Styles;

namespace Das.Views.Rendering
{
    public abstract class BaseRenderContext : IRenderContext
    {
        protected BaseRenderContext(IMeasureContext measureContext, IViewPerspective perspective)
        {
            RenderPositions = new Dictionary<IVisualElement, ICube>();

            _measureContext = measureContext;
            Perspective = perspective;
            CurrentElementRect = new Rectangle();
            _locations = new Stack<Rectangle>();
            _locations.Push(CurrentElementRect);
        }

        private readonly IMeasureContext _measureContext;
        protected Point CurrentLocation => CurrentElementRect.Location;
        protected Rectangle CurrentElementRect;
        private readonly Stack<Rectangle> _locations;
        private Int32 _currentZ;

        protected Dictionary<IVisualElement, ICube> RenderPositions { get; }

        IDictionary<IVisualElement, ICube> IRenderContext.RenderPositions
            => new Dictionary<IVisualElement, ICube>(RenderPositions);

        public IEnumerable<IRenderedVisual> GetElementsAt(IPoint point)
        {
            var positions = RenderPositions;
            var elements = positions.Where(p => p.Value.Contains(point))
                .OrderByDescending(p => p.Value.Depth);

            foreach (var kvp in elements)
                yield return new RenderedVisual(kvp.Key, kvp.Value);
        }


        public abstract void DrawLine(IPen pen, IPoint pt1, IPoint pt2);
        public abstract void DrawLines(IPen pen, IPoint[] points);

        public abstract void FillRect(IRectangle rect, IBrush brush);
//        {
//            rect = GetAbsoluteRect(rect);
//            OnFillRect(rect, brush);
//        }

        public abstract void DrawRect(IRectangle rect, IPen pen);

        public abstract void FillPie(IPoint center, double radius, double startAngle, 
            double endAngle, IBrush brush);

        public abstract void DrawEllipse(IPoint center, double radius, IPen pen);

        public abstract void DrawFrame(IFrame frame);

        //protected abstract void OnFillRect(IRectangle rect, IBrush brush);

        protected void OnDrawBorder(IRectangle rect, IShape2d thickness, IBrush brush)
        {
            var sumHeight = rect.Height + thickness.Top + thickness.Bottom;
            var sumWidth = rect.Width + thickness.Left + thickness.Right;

            var topLeftOutside = new Point(rect.Left - thickness.Left,
                rect.Top - thickness.Top);

            var bottomRightInside = rect.BottomRight;

            var leftRect = new Rectangle(topLeftOutside, thickness.Left, sumHeight);
            FillRect(leftRect, brush);

            var topRect = new Rectangle(topLeftOutside, sumWidth, thickness.Top);
            FillRect(topRect, brush);

            var rightRect = new Rectangle(rect.Right, topLeftOutside.Y, thickness.Right, sumHeight);
            FillRect(rightRect, brush);

            var bottomRect = new Rectangle(topLeftOutside.X, bottomRightInside.Y,
                sumWidth, thickness.Bottom);
            FillRect(bottomRect, brush);
        }

        /// <summary>
        /// Margins + space added due to alignment
        /// </summary>
        /// <returns></returns>
        private Rectangle GetOffset(IRectangle rect, IVisualElement element,
            Thickness border)
        {
            var margin = GetStyleSetter<Thickness>(StyleSetters.Margin, element)
                         * Perspective.ZoomLevel;

            margin += border;

            var totalSize = _measureContext.GetLastMeasure(element);
            if (Size.Empty.Equals(totalSize))
                return new Rectangle(rect, margin);

            var desiredSize = Size.Subtract(totalSize, margin);


            var valign = GetStyleSetter<VerticalAlignments>(
                StyleSetters.VerticalAlignment, element);
            var halign = GetStyleSetter<HorizontalAlignments>(
                StyleSetters.HorizontalAlignment, element);

            if (valign == VerticalAlignments.Top && halign == HorizontalAlignments.Left)
                return new Rectangle(rect, margin);

            var width = desiredSize.Width;
            var height = desiredSize.Height;


            Double yGap;
            var xGap = rect.Width + margin.Left - (desiredSize.Width + margin.Right);


            switch (valign)
            {
                case VerticalAlignments.Top:
                    yGap = margin.Top;
                    break;

                case VerticalAlignments.Bottom:
                    yGap = rect.Height - (height + margin.Bottom);
                    break;

                case VerticalAlignments.Center:
                    yGap = (rect.Height - height) / 2 + margin.Top - margin.Bottom;
                    break;

                case VerticalAlignments.Stretch:
                    yGap = margin.Top;
                    height = rect.Height - margin.Height;
                    break;
                default:
                    throw new NotImplementedException();
            }

            switch (halign)
            {
                case HorizontalAlignments.Left:
                    if (xGap > 0)
                        xGap = 0;
                    break;
                case HorizontalAlignments.Right:
                    xGap = rect.Width - (width + margin.Left);
                    break;
                case HorizontalAlignments.Center:
                    xGap = (rect.Width - width) / 2 + margin.Left - margin.Right;
                    break;
                case HorizontalAlignments.Stretch:
                    width = rect.Width - margin.Width;
                    xGap = margin.Left;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            return new Rectangle(xGap + rect.X, yGap + rect.Y, width, height);
        }

        public Rectangle DrawElement(IVisualElement element, IRectangle rect)
        {
            var border = GetStyleSetter<Thickness>(StyleSetters.BorderThickness, element);

            //zb rect = 0,0,1024,768, CurrentLocation = 0,0
            //suppose element measured at 200x200 and is center,center aligned
            var useRect = GetOffset(rect, element, border);
            //=> useRect = {x: 412, y: 284, w: 200, h:200}

            //however, if the margin is 10,,, 
            //=> useRect = { x: 422, y: 294, w: 200, h: 200}

            //useRect += CurrentLocation;

            if (!border.IsEmpty)
            {
                var brush = GetStyleSetter<IBrush>(StyleSetters.BorderBrush, element);
                OnDrawBorder(useRect, border, brush);
            }

            var background = GetStyleSetter<Brush>(StyleSetters.Background, element);
            if (!background.IsInvisible)
                FillRect(useRect, background);
            useRect += CurrentLocation;


            PushRect(useRect);
            RenderPositions[element] = new Cube(useRect, _currentZ);

            var drawn = OnDrawElement(element, useRect);

            PopRect();

            return drawn;
        }

        private void PushRect(Rectangle rect)
        {
            _currentZ++;
            _locations.Push(rect);
            CurrentElementRect = rect;
        }

        private void PopRect()
        {
            _currentZ--;
            _locations.Pop();
            CurrentElementRect = _locations.Peek();
        }

        protected virtual Rectangle OnDrawElement(IVisualElement element,
            // ReSharper disable once UnusedParameter.Global
            IRectangle rect)
        {
            element.Arrange(CurrentElementRect.Size, this);
            return CurrentElementRect;
        }

        public abstract void DrawString(string s, IFont font, IBrush brush, IRectangle location);

        public abstract void DrawImage(IImage img, IRectangle rect);

        public IViewState ViewState { get; set; }
        public abstract void DrawString(string s, IFont font, IBrush brush, IPoint point);

        protected virtual IPoint GetAbsolutePoint(IPoint relativePoint)
            => CurrentLocation + relativePoint;

        protected virtual Rectangle GetAbsoluteRect(IRectangle relativeRect)
            => new Rectangle(relativeRect.TopLeft + CurrentLocation, relativeRect.Size);

        public T GetStyleSetter<T>(StyleSetters setter, IVisualElement element)
            => ViewState.GetStyleSetter<T>(setter, element);

        public IViewPerspective Perspective { get; }
    }
}