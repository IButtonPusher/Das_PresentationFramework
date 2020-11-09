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
using Das.Views.Rendering.Geometry;
using Das.Views.Styles;

namespace Das.Views.Rendering
{
    public abstract class BaseRenderContext : ContextBase, 
                                              IRenderContext
    {
       
        protected BaseRenderContext(IViewPerspective perspective,
                                    IVisualSurrogateProvider surrogateProvider)
        : this(perspective, surrogateProvider, new Dictionary<IVisualElement, ICube>())
        {
           
        }

        protected BaseRenderContext(IViewPerspective perspective,
                                    IVisualSurrogateProvider surrogateProvider,
                                    Dictionary<IVisualElement, ICube> renderPositions)
        {
            RenderPositions = renderPositions;
            LastRenderPositions = new Dictionary<IVisualElement, ICube>();

            _surrogateProvider = surrogateProvider;
            _renderLock = new Object();
            Perspective = perspective;
            CurrentElementRect = new RenderRectangle();
            _locations = new Stack<RenderRectangle>();
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
            lock (_renderLock)
            {
                if (RenderPositions.TryGetValue(element, out var c))
                    return c;

                return default;
            }
        }

        public ICube? TryGetLastRenderBounds(IVisualElement element)
        {
            lock (_renderLock)
            {
                if (LastRenderPositions.TryGetValue(element, out var c))
                    return c;

                return default;
            }
        }


        public abstract IImage? GetImage(Stream stream);

        public virtual IImage? GetImage(Byte[] bytes)
        {
            var ms = new MemoryStream(bytes);
            return GetImage(ms);
        }

        public abstract IImage GetNullImage();

       

        public abstract void DrawLine<TPen, TPoint1, TPoint2>(TPen pen,
                                                              TPoint1 pt1,
                                                              TPoint2 pt2)
            where TPen : IPen
            where TPoint1 : IPoint2D
            where TPoint2 : IPoint2D;

        public abstract void DrawLines(IPen pen,
                                       IPoint2D[] points);

        public abstract void FillRectangle<TRectangle, TBrush>(TRectangle rect,
                                                               TBrush brush)
            where TRectangle : IRectangle
            where TBrush : IBrush;

        public abstract void FillRoundedRectangle<TRectangle, TBrush>(TRectangle rect,
                                                                      TBrush brush,
                                                                      Double cornerRadius)
            where TRectangle : IRectangle
            where TBrush : IBrush;

        public abstract void DrawRect<TRectangle, TPen>(TRectangle rect,
                                                        TPen pen)
            where TRectangle : IRectangle
            where TPen : IPen;

        public abstract void DrawRoundedRect<TRectangle, TPen>(TRectangle rect,
                                                               TPen pen,
                                                               Double cornerRadius)
            where TRectangle : IRectangle
            where TPen : IPen;

        public abstract void FillPie<TPoint, TBrush>(TPoint center,
                                                      Double radius,
                                                      Double startAngle,
                                                      Double endAngle,
                                                      TBrush brush)
            where TPoint : IPoint2D
            where TBrush : IBrush;

        public abstract void DrawEllipse<TPoint, TPen>(TPoint center,
                                                       Double radius,
                                                       TPen pen)
            where TPoint : IPoint2D
            where TPen : IPen;

        public abstract void DrawFrame(IFrame frame);

        public abstract void DrawImage<TRectangle>(IImage img, 
                                                   TRectangle destination) 
            where TRectangle : IRectangle;

        public virtual Rectangle DrawMainElement<TRectangle>(IVisualElement element,
                                                             TRectangle rect,
                                                             IViewState viewState)
            where TRectangle : IRectangle
        {
            lock (_renderLock)
            {
                LastRenderPositions.Clear();
                foreach (var kvp in RenderPositions)
                {
                    LastRenderPositions[kvp.Key] = kvp.Value;
                }
                RenderPositions.Clear();
            }

            ViewState = viewState;
            return DrawElement(element, new ValueRenderRectangle(rect));
        }

        public  Rectangle DrawElement<TRenderRectangle>(IVisualElement element, 
                                                        TRenderRectangle rect)
            where TRenderRectangle : IRenderRectangle
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

        public abstract void DrawString<TFont, TBrush, TRectangle>(String s,
                                                                   TFont font,
                                                                   TRectangle rect,
                                                                   TBrush brush)
            where TFont : IFont
            where TBrush : IBrush
            where TRectangle : IRectangle;

        public abstract void DrawImage<TRectangle1, TRectangle2>(IImage img,
                                                                 TRectangle1 sourceRect,
                                                                 TRectangle2 destination)
            where TRectangle1 : IRectangle
            where TRectangle2 : IRectangle;



        public abstract void DrawString<TFont, TBrush, TPoint>(String s,
                                                               TFont font,
                                                               TBrush brush,
                                                               TPoint location)
            where TFont : IFont
            where TBrush : IBrush
            where TPoint : IPoint2D;


        public Double GetZoomLevel()
        {
            return ViewState?.ZoomLevel ?? 1;
        }

        public IViewPerspective Perspective { get; }

        protected Point2D CurrentLocation => CurrentElementRect.Location;

        protected Dictionary<IVisualElement, ICube> RenderPositions { get; }

        protected Dictionary<IVisualElement, ICube> LastRenderPositions { get; }

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
        private RenderRectangle GetOffset(IRenderRectangle rect, 
                                    IVisualElement element,
                                    Thickness border)
        {
            var margin = GetStyleSetter<Thickness>(StyleSetter.Margin, element)
                         * Perspective.ZoomLevel;

            margin += border;

            // this is wrong - breaks the whole paradigm of the parent deciding child size
            //var totalSize = _measureContext.GetLastMeasure(element);
            var totalSize = rect.Size;
            if (Size.Empty.Equals(totalSize))
                return new RenderRectangle(rect, margin, rect.Offset);

            var desiredSize = Size.Subtract(totalSize, margin);


            var valign = GetStyleSetter<VerticalAlignments>(
                StyleSetter.VerticalAlignment, element);
            var halign = GetStyleSetter<HorizontalAlignments>(
                StyleSetter.HorizontalAlignment, element);

            if (valign == VerticalAlignments.Top && halign == HorizontalAlignments.Left)
                return new RenderRectangle(rect, margin, rect.Offset);

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


            return new RenderRectangle(xGap + rect.X, yGap + rect.Y, 
                width, height, rect.Offset);
        }

        protected void OnDrawBorder<TRectangle, TShape, TBrush>(TRectangle rect,
                                                                TShape thickness,
                                                                TBrush brush,
                                                                Double cornerRadius)
            where TRectangle : IRectangle
            where TShape : IShape2d
            where TBrush : IBrush
        {
            if (cornerRadius != 0)
            {
                if (!(brush is SolidColorBrush scb))
                    throw new NotImplementedException();

                //var scb = (SolidColorBrush) brush;
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
                element.Arrange(CurrentElementRect, this);
            }

            return CurrentElementRect;
        }

        private void PopRect()
        {
            _currentZ--;
            _locations.Pop();
            CurrentElementRect = _locations.Peek();
        }

        private void PushRect(RenderRectangle rect)
        {
            _currentZ++;
            _locations.Push(rect);
            CurrentElementRect = rect;
        }

        private readonly Stack<RenderRectangle> _locations;

        private readonly IVisualSurrogateProvider _surrogateProvider;
        private readonly Object _renderLock;
        private Int32 _currentZ;
        protected RenderRectangle CurrentElementRect;
    }
}