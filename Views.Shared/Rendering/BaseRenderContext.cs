using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Input;
using Das.Views.Panels;
using Das.Views.Styles;

namespace Das.Views.Rendering
{
    public abstract class BaseRenderContext : IRenderContext
    {
        protected BaseRenderContext(IMeasureContext measureContext, 
                                    IViewPerspective perspective)
        {
            RenderPositions = new Dictionary<IVisualElement, ICube>();

            _measureContext = measureContext;
            _renderLock = new Object();
            Perspective = perspective;
            CurrentElementRect = new Rectangle();
            _locations = new Stack<Rectangle>();
            _locations.Push(CurrentElementRect);
        }

        //IDictionary<IVisualElement, ICube> IRenderContext.RenderPositions
        //    => new Dictionary<IVisualElement, ICube>(RenderPositions);

        public IEnumerable<IRenderedVisual> GetElementsAt(IPoint2D point2D)
        {
            lock (_renderLock)
            {
                foreach (var kvp in RenderPositions.Where(p => p.Value.Contains(point2D))
                                                   .OrderByDescending(p => p.Value.Depth))
                {
                    if (kvp.Key is IContentContainer container && container.Content != null)
                        yield return new RenderedVisual(container.Content, kvp.Value);
                    
                    yield return new RenderedVisual(kvp.Key, kvp.Value);
                }
                    
            }
        }

        public IEnumerable<T> GetVisualsForInput<T>(IPoint2D point2D,
                                                  InputAction inputAction)
        where T : class
        {
            foreach (var visual in GetElementsAt(point2D))
            {
                if (!(visual.Element is IInteractiveView interactive) || 
                    !(visual.Element is T ofType))
                    continue;

                if (interactive.HandlesActions.HasFlag(inputAction))
                    yield return ofType;
            }
        }

        public IEnumerable<IHandleInput<T>> GetVisualsForMouseInput<T>(IPoint2D point2D, 
                                                                       InputAction inputAction) 
            where T : IInputEventArgs
        {
            foreach (var visual in GetElementsAt(point2D))
            {
                if ((visual.Element is IInteractiveView interactive) &&
                    (interactive.HandlesActions & inputAction) == inputAction &&
                    interactive is IHandleInput<T> handler)
                {
                    yield return handler;
                }
            }
        }


        public abstract IImage? GetImage(Stream stream);

        public abstract void DrawLine(IPen pen, IPoint2D pt1, IPoint2D pt2);

        public abstract void DrawLines(IPen pen, IPoint2D[] points);

        public abstract void FillRect(IRectangle rect, IBrush brush);

        public abstract void DrawRect(IRectangle rect, IPen pen);

        public abstract void FillPie(IPoint2D center, Double radius, Double startAngle,
                                     Double endAngle, IBrush brush);

        public abstract void DrawEllipse(IPoint2D center, Double radius, IPen pen);

        public abstract void DrawFrame(IFrame frame);

        public Rectangle DrawElement(IVisualElement element, IRectangle rect)
        {
            var border = GetStyleSetter<Thickness>(StyleSetters.BorderThickness, element);

            //zb rect = 0,0,1024,768, CurrentLocation = 0,0
            //suppose element measured at 200x200 and is center,center aligned
            var useRect = GetOffset(rect, element, border);
            //=> useRect = {x: 412, y: 284, w: 200, h:200}

            //however, if the margin is 10,,, 
            //=> useRect = { x: 422, y: 294, w: 200, h: 200}

            if (!border.IsEmpty)
            {
                var brush = GetStyleSetter<IBrush>(StyleSetters.BorderBrush, element);
                OnDrawBorder(useRect, border, brush);
            }

            var background = GetStyleSetter<SolidColorBrush>(StyleSetters.Background, element);
            if (!background.IsInvisible)
                FillRect(useRect, background);
            useRect += CurrentLocation;


            PushRect(useRect);
            lock (_renderLock)
                RenderPositions[element] = new Cube(useRect, _currentZ);

            var drawn = OnDrawElement(element, useRect);

            PopRect();

            return drawn;
        }

        public abstract void DrawString(String s, IFont font, IBrush brush, IRectangle location);

        public abstract void DrawImage(IImage img, IRectangle rect);

        public IViewState? ViewState { get; set; }

        public abstract void DrawString(String s, IFont font, IBrush brush, IPoint2D point2D);

        public T GetStyleSetter<T>(StyleSetters setter, IVisualElement element)
        {
            if (ViewState is {} vs)
                return vs.GetStyleSetter<T>(setter, element);
            else throw new NullReferenceException();
        }

        public IViewPerspective Perspective { get; }

        protected Point2D CurrentLocation => CurrentElementRect.Location;

        protected Dictionary<IVisualElement, ICube> RenderPositions { get; }

        protected virtual IPoint2D GetAbsolutePoint(IPoint2D relativePoint2D)
        {
            return CurrentLocation + relativePoint2D;
        }

        protected virtual Rectangle GetAbsoluteRect(IRectangle relativeRect)
        {
            return new Rectangle(relativeRect.TopLeft + CurrentLocation, relativeRect.Size);
        }

        /// <summary>
        ///     Margins + space added due to alignment
        /// </summary>
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

        protected void OnDrawBorder(IRectangle rect, IShape2d thickness, IBrush brush)
        {
            var sumHeight = rect.Height + thickness.Top + thickness.Bottom;
            var sumWidth = rect.Width + thickness.Left + thickness.Right;

            var topLeftOutside = new Point2D(rect.Left - thickness.Left,
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

        protected virtual Rectangle OnDrawElement(IVisualElement element,
                                                  // ReSharper disable once UnusedParameter.Global
                                                  IRectangle rect)
        {
            element.Arrange(CurrentElementRect.Size, this);
            return CurrentElementRect;
        }

        private void PopRect()
        {
            _currentZ--;
            _locations.Pop();
            CurrentElementRect = _locations.Peek();
        }

        private void PushRect(Rectangle rect)
        {
            _currentZ++;
            _locations.Push(rect);
            CurrentElementRect = rect;
        }

        private readonly Stack<Rectangle> _locations;
        private readonly Object _renderLock;

        private readonly IMeasureContext _measureContext;
        private Int32 _currentZ;
        protected Rectangle CurrentElementRect;
    }
}