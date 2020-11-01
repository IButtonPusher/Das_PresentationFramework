using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Input;
using Das.Views.Panels;
using Das.Views.Styles;

namespace Das.Views.Rendering
{
    public abstract class BaseRenderContext : ContextBase, 
                                              IRenderContext
    {
        protected BaseRenderContext(IMeasureContext measureContext,
                                    IViewPerspective perspective,
                                    IVisualSurrogateProvider surrogateProvider)
        {
            RenderPositions = new Dictionary<IVisualElement, ICube>();

            _measureContext = measureContext;
            _surrogateProvider = surrogateProvider;
            _renderLock = new Object();
            Perspective = perspective;
            CurrentElementRect = new Rectangle();
            _locations = new Stack<Rectangle>();
            _locations.Push(CurrentElementRect);
        }

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
                if (visual.Element is IInteractiveView interactive &&
                    (interactive.HandlesActions & inputAction) == inputAction &&
                    interactive is IHandleInput<T> handler)
                    yield return handler;
        }

        public IEnumerable<IRenderedVisual<IHandleInput<T>>> GetRenderedVisualsForMouseInput<T>(
            IPoint2D point2D, InputAction inputAction)
            where T : IInputEventArgs
        {
            foreach (var visual in GetElementsAt<IHandleInput<T>>(point2D))
                if (visual.Element is IInteractiveView interactive &&
                    (interactive.HandlesActions & inputAction) == inputAction)
                    yield return visual;
        }

        public ICube? TryGetElementBounds(IVisualElement element)
        {
            if (RenderPositions.TryGetValue(element, out var c))
                return c;

            return default;
        }


        public abstract IImage? GetImage(Stream stream);

        public abstract void DrawImage(IImage img,
                                       IRectangle sourceRest,
                                       IRectangle destination);

        public abstract void DrawLine(IPen pen,
                                      IPoint2D pt1,
                                      IPoint2D pt2);

        public abstract void DrawLines(IPen pen,
                                       IPoint2D[] points);

        public abstract void FillRectangle(IRectangle rect,
                                      IBrush brush);

        public abstract void FillRoundedRectangle(IRectangle rect, 
                                             IBrush brush, 
                                             Double cornerRadius);

        public abstract void DrawRect(IRectangle rect,
                                      IPen pen);

        public abstract void DrawRoundedRect(IRectangle rect, 
                                             IPen pen, 
                                             Double cornerRadius);

        public abstract void FillPie(IPoint2D center,
                                     Double radius,
                                     Double startAngle,
                                     Double endAngle,
                                     IBrush brush);

        public abstract void DrawEllipse(IPoint2D center, Double radius, IPen pen);

        public abstract void DrawFrame(IFrame frame);

        public Rectangle DrawMainElement(IVisualElement element, 
                                         IRectangle rect, 
                                         IViewState viewState)
        {
            lock (_renderLock)
                RenderPositions.Clear();
            ViewState = viewState;
            return DrawElement(element, rect);
        }

        public Rectangle DrawElement(IVisualElement element,
                                     IRectangle rect)
        {
            _surrogateProvider.EnsureSurrogate(ref element);

            var selector = element is IInteractiveView interactive
                ? interactive.CurrentStyleSelector
                : StyleSelector.None;

            var border = GetStyleSetter<Thickness>(StyleSetter.BorderThickness, 
                selector, element);

            //zb rect = 0,0,1024,768, CurrentLocation = 0,0
            //suppose element measured at 200x200 and is center,center aligned
            var useRect = GetOffset(rect, element, border);
            //=> useRect = {x: 412, y: 284, w: 200, h:200}

            //however, if the margin is 10,,, 
            //=> useRect = { x: 422, y: 294, w: 200, h: 200}

            var radius = GetStyleSetter<Double>(StyleSetter.BorderRadius, selector, element);

            var background = GetStyleSetter<SolidColorBrush>(StyleSetter.Background, 
                selector, element);
            if (!background.IsInvisible)
            {
                if (radius.IsZero())
                    FillRectangle(useRect, background);
                else
                    FillRoundedRectangle(useRect, background, radius);
            }

            if (!border.IsEmpty)
            {
                var brush = GetStyleSetter<IBrush>(StyleSetter.BorderBrush, selector, element);
                OnDrawBorder(useRect, border, brush, radius);
            }

            useRect += CurrentLocation;

            PushRect(useRect);
            lock (_renderLock)
            {
                RenderPositions[element] = new Cube(useRect, _currentZ);
            }

            var drawn = OnDrawElement(element, useRect);

            PopRect();

            return drawn;
        }

        public abstract void DrawString(String s,
                                        IFont font,
                                        IBrush brush,
                                        IRectangle location);

        public abstract void DrawImage(IImage img,
                                       IRectangle rect);

        public abstract void DrawString(String s,
                                        IFont font,
                                        IBrush brush,
                                        IPoint2D point2D);


        public Double GetZoomLevel()
        {
            return ViewState?.ZoomLevel ?? 1;
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

        public IEnumerable<IRenderedVisual<TElement>> GetElementsAt<TElement>(IPoint2D point2D)
        {
            lock (_renderLock)
            {
                foreach (var kvp in RenderPositions.Where(p => p.Value.Contains(point2D))
                                                   .OrderByDescending(p => p.Value.Depth))
                {
                    var current = kvp.Key;
                    while (current is IContentContainer container)
                    {
                        if (container.Content is TElement valid)
                            yield return new RenderedVisual<TElement>(valid, kvp.Value);

                        current = container.Content;
                    }

                    if (kvp.Key is TElement good)
                        yield return new RenderedVisual<TElement>(good, kvp.Value);
                }
            }
        }

        /// <summary>
        ///     Margins + space added due to alignment
        /// </summary>
        private Rectangle GetOffset(IRectangle rect, 
                                    IVisualElement element,
                                    Thickness border)
        {
            var margin = GetStyleSetter<Thickness>(StyleSetter.Margin, element)
                         * Perspective.ZoomLevel;

            margin += border;

            //var totalSize = _measureContext.GetLastMeasure(element);
            var totalSize = rect.Size;
            if (Size.Empty.Equals(totalSize))
                return new Rectangle(rect, margin);

            var desiredSize = Size.Subtract(totalSize, margin);


            var valign = GetStyleSetter<VerticalAlignments>(
                StyleSetter.VerticalAlignment, element);
            var halign = GetStyleSetter<HorizontalAlignments>(
                StyleSetter.HorizontalAlignment, element);

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

        protected void OnDrawBorder(IRectangle rect,
                                    IShape2d thickness,
                                    IBrush brush,
                                    Double cornerRadius)
        {
            if (cornerRadius != 0)
            {
                var scb = (SolidColorBrush) brush;
                var p = new Pen(scb.Color, Convert.ToInt32(thickness.Left));

                DrawRoundedRect(rect, p, cornerRadius);
                return;
            }

            var sumHeight = rect.Height + thickness.Top + thickness.Bottom;
            var sumWidth = rect.Width + thickness.Left + thickness.Right;

            var topLeftOutside = new ValuePoint2D(rect.Left - thickness.Left,
                rect.Top - thickness.Top);

            var bottomRightInside = rect.BottomRight;

            var leftRect = new ValueRectangle(topLeftOutside, thickness.Left, sumHeight);
            FillRectangle(leftRect, brush);

            var topRect = new ValueRectangle(topLeftOutside, sumWidth, thickness.Top);
            FillRectangle(topRect, brush);

            var rightRect = new ValueRectangle(rect.Right, topLeftOutside.Y, thickness.Right, sumHeight);
            FillRectangle(rightRect, brush);

            var bottomRect = new ValueRectangle(topLeftOutside.X, bottomRightInside.Y,
                sumWidth, thickness.Bottom);
            FillRectangle(bottomRect, brush);
        }

        protected virtual Rectangle OnDrawElement(IVisualElement element,
                                                  // ReSharper disable once UnusedParameter.Global
                                                  IRectangle rect)
        {
            if (rect.Bottom > 0 || rect.Right > 0)
            {
                // don't draw it if it's completely off screen
                element.Arrange(CurrentElementRect.Size, this);
            }

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

        private readonly IMeasureContext _measureContext;
        private readonly IVisualSurrogateProvider _surrogateProvider;
        private readonly Object _renderLock;
        private Int32 _currentZ;
        protected Rectangle CurrentElementRect;
    }
}