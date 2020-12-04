using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
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
                                    IVisualSurrogateProvider surrogateProvider,
                                    IStyleContext styleContext)
        : this(perspective, surrogateProvider, 
            new Dictionary<IVisualElement, ValueCube>(),
            new Dictionary<IVisualElement, ValueSize>(),
            styleContext)
        {
           
        }

        protected BaseRenderContext(IViewPerspective perspective,
                                    IVisualSurrogateProvider surrogateProvider,
                                    Dictionary<IVisualElement, ValueCube> renderPositions,
                                    Dictionary<IVisualElement, ValueSize> lastMeasurements,
                                    IStyleContext styleContext)
        : base(lastMeasurements, styleContext)
        {
            

            RenderPositions = renderPositions;
            LastRenderPositions = new Dictionary<IVisualElement, ValueCube>();

            _surrogateProvider = surrogateProvider;
            _lastMeasurements = lastMeasurements;
            _styleContext = styleContext;
            _renderLock = new Object();
            Perspective = perspective;
            CurrentElementRect = new RenderRectangle();
            _locations = new Stack<RenderRectangle>();
            _locations.Push(CurrentElementRect);
        }


        public IEnumerable<IRenderedVisual> GetElementsAt<TPoint>(TPoint point2D)
            where TPoint : IPoint2D
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

        public IEnumerable<TVisual> GetVisualsForInput<TVisual, TPoint>(TPoint point2D,
                                           InputAction inputAction)
            where TVisual : class
            where TPoint : IPoint2D
        {
            foreach (var visual in GetElementsAt(point2D))
            {
                if (!(visual.Element is IInteractiveView interactive) ||
                    !(visual.Element is TVisual ofType))
                    continue;

                if (interactive.HandlesActions.HasFlag(inputAction))
                    yield return ofType;
            }
        }

        public IEnumerable<IHandleInput<T>> GetVisualsForMouseInput<T, TPoint>(TPoint point2D,
                                                                       InputAction inputAction)
            where T : IInputEventArgs
            where TPoint : IPoint2D
        {
            foreach (var visual in GetElementsAt(point2D))
                if (visual.Element is IInteractiveView interactive &&
                    (interactive.HandlesActions & inputAction) == inputAction &&
                    interactive is IHandleInput<T> handler)
                    yield return handler;
        }

        public IEnumerable<IRenderedVisual<IHandleInput<T>>> GetRenderedVisualsForMouseInput<T, TPoint>(
            TPoint point2D,
            InputAction inputAction)
            where T : IInputEventArgs
            where TPoint : IPoint2D
        {
            lock (_renderLock)
            {
                foreach (var kvp in RenderPositions.Where(p => p.Value.Contains(point2D))
                                                   .OrderByDescending(p => p.Value.Depth))
                {
                    if (!(kvp.Key is IInteractiveView interactive) || 
                        (interactive.HandlesActions & inputAction) != inputAction || 
                        !(interactive is IHandleInput<T> inputHandler))
                        continue;

                    //if (kvp.Key is IContentContainer container && container.Content != null)
                    //    yield return new RenderedVisual(container.Content, kvp.Value);

                    yield return new RenderedVisual<IHandleInput<T>>(inputHandler, kvp.Value);
                }
            }

            //foreach (var visual in GetElementsAt<IHandleInput<T>>(point2D))
            //    if (visual.Element is IInteractiveView interactive &&
            //        (interactive.HandlesActions & inputAction) == inputAction)
            //        yield return visual;
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
                if (RenderPositions.TryGetValue(element, out var c) || 
                    LastRenderPositions.TryGetValue(element, out c))
                    return c;

                return default;
            }
        }


        //public abstract IImage? GetImage(Stream stream);

        //public abstract IImage? GetImage(Stream stream, Double maximumWidthPct);

        //public virtual IImage? GetImage(Byte[] bytes)
        //{
        //    var ms = new MemoryStream(bytes);
        //    return GetImage(ms);
        //}

        //public abstract IImage GetNullImage();

       

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


        public void DrawElementAt<TPosition>(IVisualElement element, 
                                                  TPosition location) 
            where TPosition : IPoint2D
        {
            if (!_lastMeasurements.TryGetValue(element, out var size))
                return;

            _fairyRect ??= new RenderRectangle(0, 0, 0, 0, Point2D.Empty);

            _fairyRect.X = location.X;
            _fairyRect.Y = location.Y;
            _fairyRect.Width = size.Width;
            _fairyRect.Height = size.Height;

            DrawElement(element, _fairyRect);
        }

        public abstract void DrawEllipse<TPoint, TPen>(TPoint center,
                                                       Double radius,
                                                       TPen pen)
            where TPoint : IPoint2D
            where TPen : IPen;

        public abstract void DrawFrame(IFrame frame);

        public abstract void DrawImage<TRectangle>(IImage img, 
                                                   TRectangle destination) 
            where TRectangle : IRectangle;

        public virtual void DrawMainElement<TRectangle>(IVisualElement element,
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

                ViewState = viewState;
                DrawElement(element, new ValueRenderRectangle(rect));
            }

            //ViewState = viewState;
            //DrawElement(element, new ValueRenderRectangle(rect));
        }


        public void DrawContentElement<TSize>(IVisualElement element, 
                                                 TSize size) 
            where TSize : ISize
        {
            _fairyRect ??= new RenderRectangle(0, 0, 0, 0, Point2D.Empty);

            _fairyRect.X = 0;
            _fairyRect.Y = 0;
            _fairyRect.Width = size.Width;
            _fairyRect.Height = size.Height;

            DrawElement(element, _fairyRect);

            //return DrawElementImpl(element, size, 0, 0, Point2D.Empty);
        }

        public void DrawElement<TRenderRectangle>(IVisualElement element, 
                                                        TRenderRectangle rect)
            where TRenderRectangle : IRenderRectangle
        {
            _styleContext.PushVisual(element);

            _surrogateProvider.EnsureSurrogate(ref element);

            var selector = element is IInteractiveView interactive
                ? interactive.CurrentStyleSelector
                : StyleSelector.None;


            var border = GetStyleSetter<Thickness>(StyleSetter.BorderThickness, 
                selector, element);

            var margin = GetStyleSetter<Thickness>(StyleSetter.Margin, element);

            _fairyRect ??= new RenderRectangle(0, 0, 0, 0, Point2D.Empty);

            _fairyRect.Update(rect,
                CurrentElementRect.Offset, margin, border);

            var useRect = _fairyRect;

            useRect.Update(rect, CurrentElementRect.Offset, margin, border);

            var radius = GetStyleSetter<Double>(StyleSetter.BorderRadius, selector, element);

            var background = element.Background ??
                             GetStyleSetter<SolidColorBrush>(StyleSetter.Background, 
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

            SetElementRenderPosition(useRect, element);

           // lock (_renderLock)
            {
                //var currentClip = GetCurrentClip();
                
                //if (currentClip.IsEmpty)
                //{
                //    RenderPositions[element] = new ValueCube(useRect, _currentZ);
                //}
                //else
                //{
                //    var leftOverlap = useRect.Left < currentClip.Left
                //        ? useRect.Left - currentClip.Left
                //        : 0;
                //    var topOverlap = useRect.Top < currentClip.Top
                //        ? useRect.Top - currentClip.Top
                //        : 0;
                //    var rightOverlap = useRect.Right > currentClip.Right
                //        ? useRect.Right - currentClip.Right
                //        : 0;
                //    var bottomOverlap = useRect.Bottom > currentClip.Bottom
                //        ? useRect.Bottom - currentClip.Bottom
                //        : 0;

                //    var left = useRect.Left + leftOverlap;
                //    var top = useRect.Top + topOverlap;
                //    var width = useRect.Width - (leftOverlap + rightOverlap);
                //    var height = useRect.Height - (topOverlap + bottomOverlap);

                //    RenderPositions[element] = new ValueCube(left, top, width, height,
                //        _currentZ);
                //}
            }

            //System.Diagnostics.Debug.WriteLine(_tabs + element.GetType().Name + "\t\ttarget rect: (" + rect.X + "," + 
            //                                   rect.Y +
            //                                   ") [" + rect.Width.ToString("0.0") + "," + rect.Height.ToString("0.0") + "] - \t\t" +
            //                                   " effective: " + useRect + " inherited: " + CurrentLocation + 
            //                                   " recorded: " + RenderPositions[element]);

            //_tabs += "\t";

            if (element.IsClipsContent && !useRect.IsEmpty)
            {
                if (ZoomLevel.AreDifferent(1.0))
                {
                    PushClip(useRect * ZoomLevel);
                }
                else
                    PushClip(useRect);
            }

            /////////////////////////////////
            /////////////////////////////////
            if (useRect.Bottom > 0 || useRect.Right > 0)
            {
                // don't draw it if it's completely off screen
                element.Arrange(CurrentElementRect, this);
            }
            /////////////////////////////////
            /////////////////////////////////

            PopRect();
            if (element.IsClipsContent && !useRect.IsEmpty)
                PopClip(useRect);

            _styleContext.PopVisual();

            element.AcceptChanges(ChangeType.Arrange);

            //_tabs = _tabs.Substring(0, _tabs.Length - 1);
        }

        private void SetElementRenderPosition(RenderRectangle useRect,
                                              IVisualElement element)
        {
            var currentClip = GetCurrentClip();
                
            if (currentClip.IsEmpty)
            {
                RenderPositions[element] = new ValueCube(useRect, _currentZ);
            }
            else
            {
                var leftOverlap = useRect.Left < currentClip.Left
                    ? useRect.Left - currentClip.Left
                    : 0;
                var topOverlap = useRect.Top < currentClip.Top
                    ? useRect.Top - currentClip.Top
                    : 0;
                var rightOverlap = useRect.Right > currentClip.Right
                    ? useRect.Right - currentClip.Right
                    : 0;
                var bottomOverlap = useRect.Bottom > currentClip.Bottom
                    ? useRect.Bottom - currentClip.Bottom
                    : 0;

                var left = useRect.Left + leftOverlap;
                var top = useRect.Top + topOverlap;
                var width = useRect.Width - (leftOverlap + rightOverlap);
                var height = useRect.Height - (topOverlap + bottomOverlap);

                RenderPositions[element] = new ValueCube(left, top, width, height,
                    _currentZ);
            }
        }

        protected abstract void PushClip<TRectangle>(TRectangle rect)
            where TRectangle : IRectangle;

        protected abstract void PopClip<TRectangle>(TRectangle rect)
            where TRectangle : IRectangle;

        protected abstract ValueRectangle GetCurrentClip();

        //private void OnElementDisposed(IVisualElement element)
        //{
        //    lock (_renderLock)
        //        RenderPositions.Remove(element);
        //}

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


        public IViewPerspective Perspective { get; }

        protected Point2D CurrentLocation => CurrentElementRect.Location;

        protected Dictionary<IVisualElement, ValueCube> RenderPositions { get; }

        protected Dictionary<IVisualElement, ValueCube> LastRenderPositions { get; }

        protected virtual IPoint2D GetAbsolutePoint(IPoint2D relativePoint2D)
        {
            if (ZoomLevel.AreDifferent(1.0))
                return new ValuePoint2D(
                    (CurrentLocation.X + relativePoint2D.X) * ZoomLevel,
                    (CurrentLocation.Y + relativePoint2D.Y) * ZoomLevel);

            return CurrentLocation + relativePoint2D;
        }

        protected virtual IPoint2D GetAbsolutePointNoZoom(IPoint2D relativePoint2D)
        {
            if (ZoomLevel.AreDifferent(1.0))
                return new ValuePoint2D(
                    (CurrentLocation.X + relativePoint2D.X),
                    (CurrentLocation.Y + relativePoint2D.Y));

            return CurrentLocation + relativePoint2D;
        }

        protected virtual ValueRectangle GetAbsoluteRect<TRectangle>(TRectangle relativeRect)
            where TRectangle : IRectangle
        {
            if (ZoomLevel.AreDifferent(1.0))
                return new ValueRectangle(
                    (relativeRect.X + CurrentLocation.X) * ZoomLevel,
                    (relativeRect.Y + CurrentLocation.Y) * ZoomLevel,
                    relativeRect.Width * ZoomLevel,
                    relativeRect.Height * ZoomLevel);

                return new ValueRectangle(relativeRect.TopLeft + CurrentLocation, 
                    relativeRect.Size);
        }

        protected virtual ValueIntRectangle GetAbsoluteIntRect<TRectangle>(TRectangle relativeRect)
            where TRectangle : IRectangle
        {
            return new ValueIntRectangle(relativeRect.TopLeft + CurrentLocation, relativeRect.Size);
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
            if (CurrentElementRect.X < 0)
            {}
        }

        private readonly Stack<RenderRectangle> _locations;

        private readonly IVisualSurrogateProvider _surrogateProvider;
        private readonly Dictionary<IVisualElement, ValueSize> _lastMeasurements;
        private readonly IStyleContext _styleContext;
        private readonly Object _renderLock;
        private Int32 _currentZ;
        protected RenderRectangle CurrentElementRect;
        //private String _tabs = String.Empty;

        [ThreadStatic]
        private static RenderRectangle? _fairyRect;
    }
}