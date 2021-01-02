using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.BoxModel;
using Das.Views.Controls;
using Das.Views.Core;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Input;
using Das.Views.Layout;
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
                                    IStyleContext styleContext,
                                    IVisualLineage visualLineage,
                                    Dictionary<IVisualElement, ValueCube> renderPositions,
                                    ILayoutQueue layoutQueue)
            : this(perspective, surrogateProvider,
                renderPositions,
                new Dictionary<IVisualElement, ValueSize>(),
                styleContext, visualLineage, layoutQueue)
        {
        }

        protected BaseRenderContext(IViewPerspective perspective,
                                    IVisualSurrogateProvider surrogateProvider,
                                    Dictionary<IVisualElement, ValueCube> renderPositions,
                                    Dictionary<IVisualElement, ValueSize> lastMeasurements,
                                    IStyleContext styleContext,
                                    IVisualLineage visualLineage,
                                    ILayoutQueue layoutQueue)
            : base(lastMeasurements, styleContext, surrogateProvider, 
                visualLineage, layoutQueue)
        {
            RenderPositions = renderPositions;
            LastRenderPositions = new Dictionary<IVisualElement, ValueCube>();

            _lastMeasurements = lastMeasurements;
            
            _renderLock = new Object();
            Perspective = perspective;
            CurrentElementRect = ValueRenderRectangle.Empty;
            _locations = new Stack<ValueRenderRectangle>();
            _locations.Push(CurrentElementRect);

            _boxModel = new BoxModelLayoutTree();
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
                if (!(visual.Element is IHandleInput interactive) ||
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
                if (visual.Element is IHandleInput interactive &&
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
                    if (!(kvp.Key is IHandleInput interactive) ||
                        (interactive.HandlesActions & inputAction) != inputAction ||
                        !(interactive is IHandleInput<T> inputHandler))
                        continue;

                    yield return new RenderedVisual<IHandleInput<T>>(inputHandler, kvp.Value);
                }
            }
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

        public abstract void FillRoundedRectangle<TRectangle, TBrush, TThickness>(TRectangle rect,
            TBrush brush,
            TThickness cornerRadii)
            where TRectangle : IRectangle
            where TBrush : IBrush
            where TThickness : IThickness;

        public abstract void DrawRect<TRectangle, TPen>(TRectangle rect,
                                                        TPen pen)
            where TRectangle : IRectangle
            where TPen : IPen;

        public abstract void DrawRoundedRect<TRectangle, TPen, TThickness>(TRectangle rect,
                                                                           TPen pen,
                                                                           TThickness cornerRadii)
            where TRectangle : IRectangle
            where TPen : IPen
            where TThickness : IThickness;

        public abstract void FillPie<TPoint, TBrush>(TPoint center,
                                                     Double radius,
                                                     Double startAngle,
                                                     Double endAngle,
                                                     TBrush brush)
            where TPoint : IPoint2D
            where TBrush : IBrush;


        //public void DrawElementAt<TPosition>(IVisualElement element,
        //                                     TPosition location)
        //    where TPosition : IPoint2D
        //{
        //    if (!_lastMeasurements.TryGetValue(element, out var size))
        //        return;

        //    _fairyRect ??= new RenderRectangle(0, 0, 0, 0, Point2D.Empty);

        //    _fairyRect.X = location.X;
        //    _fairyRect.Y = location.Y;
        //    _fairyRect.Width = size.Width;
        //    _fairyRect.Height = size.Height;

        //    DrawElement(element, _fairyRect);
        //}

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
                ViewState = viewState;

                FillRectangle(rect, viewState.StyleContext.ColorPalette.Background);

                LastRenderPositions.Clear();
                foreach (var kvp in RenderPositions) LastRenderPositions[kvp.Key] = kvp.Value;
                RenderPositions.Clear();


                DrawElement(element, new ValueRenderRectangle(rect));
            }
        }


        //public void DrawContentElement<TSize>(IVisualElement element,
        //                                      TSize size)
        //    where TSize : ISize
        //{
        //    _fairyRect ??= new RenderRectangle(0, 0, 0, 0, Point2D.Empty);

        //    _fairyRect.X = 0;
        //    _fairyRect.Y = 0;
        //    _fairyRect.Width = size.Width;
        //    _fairyRect.Height = size.Height;

        //    DrawElement(element, _fairyRect);
        //}

        public void DrawElement<TRenderRectangle>(IVisualElement visual,
                                                  TRenderRectangle rect)
            where TRenderRectangle : IRenderRectangle
        {
            if (!CanDrawVisual(ref visual))
                return;

            DrawPseudoElements(visual, ref rect, out var rightLabelRect);

            VisualLineage.PushVisual(visual);

            var layoutVisual = GetElementForLayout(visual);

            //var styles = ViewState.StyleContext;

            //var selector = layoutVisual is IHandleInput interactive
            //    ? interactive.CurrentVisualStateType
            //    : VisualStateType.None;

            var border = layoutVisual.Border;
            var borderThickness = border.GetThickness(rect);

            //var border = styles.GetStyleSetter<IThickness>(StyleSetterType.BorderThickness,
            //    selector, layoutVisual, VisualLineage);

            IThickness margin = layoutVisual.Margin.GetValue(rect);
            //if (margin.IsEmpty)
            //    margin = styles.GetStyleSetter<IThickness>(StyleSetterType.Margin,
            //        selector, layoutVisual, VisualLineage);
            

            //_fairyRect ??= new RenderRectangle(0, 0, 0, 0, Point2D.Empty);

            //_fairyRect.Update(rect,
            //    CurrentElementRect.Offset, margin, border);

            //var useRect = _fairyRect;

            //useRect.Update(rect, CurrentElementRect.Offset, margin, border);

            /////////
            //var useRect2 = _boxModel.PushVisualBox(rect, visual.Transform, margin, border,
            //    GetCurrentClip());
            var useRect2 = _boxModel.ComputeContentBounds(rect, margin, borderThickness);

            _boxModel.PushTransform(visual.Transform);

            /////////
            
            //if (!useRect.Equals(useRect2))
            //{}

            var radius = layoutVisual.BorderRadius.GetValue(rect);

            if (!visual.BoxShadow.IsEmpty)
            {
                var thickness = borderThickness;//!border.IsEmpty ? border : new ValueThickness(1);
                foreach (var layer in visual.BoxShadow)
                {
                    var h = useRect2.Height + layer.SpreadRadius.GetQuantity(rect.Height);

                    var shadowRect = new ValueRectangle(useRect2.X + layer.OffsetX.GetQuantity(rect.Width),
                        useRect2.Y + layer.OffsetY.GetQuantity(rect.Height), useRect2.Width, h);
                    var colorVals = new BoxValues<IBrush>(layer.Color);
                    OnDrawBorder(shadowRect, colorVals, thickness, radius);
                   // OnDrawBorder(shadowRect, thickness, layer.Color, radius);

                    //var h = useRect.Height + layer.SpreadRadius.GetQuantity(rect.Height);

                    //var shadowRect = new ValueRectangle(useRect.X + layer.OffsetX.GetQuantity(rect.Width),
                    //    useRect.Y + layer.OffsetY.GetQuantity(rect.Height), useRect.Width, h);
                    //OnDrawBorder(shadowRect, thickness, layer.Color, radius);
                }
            }

            //if (!border.IsEmpty && !visual.BoxShadow.IsEmpty)
            //{
               

            //}

            var background = layoutVisual.Background;
                             //??
                             //styles.GetStyleSetter<SolidColorBrush>(StyleSetterType.Background,
                             //    selector, layoutVisual, VisualLineage);
            if (background?.IsInvisible == false)
            {
                if (radius.IsEmpty)
                    FillRectangle(useRect2, background);
                else
                    FillRoundedRectangle(useRect2, background, radius);
            }

            if (!border.IsEmpty)
            {
                

                //var brush = border.
                    //styles.GetStyleSetter<IBrush>(StyleSetterType.BorderBrush, selector,
                    //layoutVisual, VisualLineage);

                //if (!brush.IsInvisible)
                    OnDrawBorder(useRect2, border, borderThickness, radius);
            }

            

            useRect2 = _boxModel.PushContentBounds(useRect2);

            //useRect += CurrentLocation;

            //if (!useRect.Equals(useRect2))
            //{}

            //PushRect(useRect);
            PushRect(useRect2);

            SetElementRenderPosition(useRect2, visual);

            //System.Diagnostics.Debug.WriteLine(_tabs + visual.GetType().Name + "\t\ttarget rect: (" + rect.X + "," +
            //                                   rect.Y +
            //                                   ") [" + rect.Width.ToString("0.0") + "," + rect.Height.ToString("0.0") + "] - \t\t" +
            //                                   " effective: " + useRect + " inherited: " + CurrentLocation +
            //                                   " recorded: " + RenderPositions[visual]);

            //_tabs += "\t";

            if (layoutVisual.IsClipsContent && !useRect2.IsEmpty)
            {
                if (ZoomLevel.AreDifferent(1.0))
                    PushClip(useRect2 * ZoomLevel);
                else
                    PushClip(useRect2);
            }

            /////////////////////////////////
            /////////////////////////////////
            if (useRect2.Bottom > 0 || useRect2.Right > 0)
            {
                // don't draw it if it's completely off screen
                layoutVisual.Arrange(CurrentElementRect, this);
                visual.ArrangedBounds = useRect2;
            }
            /////////////////////////////////
            /////////////////////////////////

            PopRect();
            if (layoutVisual.IsClipsContent && !useRect2.IsEmpty)
                PopClip(useRect2);

            VisualLineage.PopVisual();

            /////////
            _boxModel.PopTransform();
            _boxModel.PopCurrentBox();
            /////////


            if (!rightLabelRect.IsEmpty && visual.AfterLabel is { } lbl)
            {
                DrawElement(lbl, rightLabelRect);
            }

            visual.AcceptChanges(ChangeType.Arrange);
            LayoutQueue.RemoveVisualFromArrangeQueue(visual);

            //_tabs = _tabs.Substring(0, _tabs.Length - 1);
        }

        /// <summary>
        /// Draws the "before" pseudo element and/or computes the "after"
        /// but doesn't draw it
        /// </summary>
        private void DrawPseudoElements<TRenderRectangle>(IVisualElement visual,
                                                          ref TRenderRectangle rect,
                                                          out ValueRenderRectangle rightLabelRect)
            where TRenderRectangle : IRenderRectangle
        {
            var beforeLabelRect = ValueRenderRectangle.Empty;
            rightLabelRect = ValueRenderRectangle.Empty;

            var leftMargin = 0.0;

            if (visual.BeforeLabel == null && visual.AfterLabel == null)
                return;

            if (visual.BeforeLabel is { } beforeLeft &&
                GetLastMeasure(beforeLeft) is { } leftWants &&
                !leftWants.IsEmpty)
            {
                beforeLabelRect = new ValueRenderRectangle(rect.Left, rect.Top,
                    leftWants.Width, leftWants.Height, rect.Offset);
                DrawElement(beforeLeft, beforeLabelRect);
                

                var beforeMargin = beforeLeft.Margin.Left;

                if (beforeMargin.IsNotZero() && beforeMargin.Units == LengthUnits.Px)
                    leftMargin = beforeMargin.GetQuantity(0);
            }

            if (visual.AfterLabel is { } afterLabel &&
                GetLastMeasure(afterLabel) is { } rightWants &&
                !rightWants.IsEmpty)
            {
                var rx = (rect.Width - (rightWants.Width + beforeLabelRect.Width)) + leftMargin;
                var ry = rect.Top + afterLabel.Top?.GetQuantity(0) ?? 0;

                rightLabelRect = new ValueRenderRectangle(rx, ry,
                    rightWants.Width, rightWants.Height, rect.Offset);
            }

            rect = rect.Reduce<TRenderRectangle>(beforeLabelRect.Width, 0, rightLabelRect.Width, 0);
        }

        private static Boolean CanDrawVisual(ref IVisualElement visual)
        {
            if (visual.Visibility != Visibility.Visible)
            {
                visual.AcceptChanges(ChangeType.Arrange);
                return false;
            }

            return true;
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


        public IViewPerspective Perspective { get; }

        protected ValuePoint2D CurrentLocation => CurrentElementRect.Location;

        protected Dictionary<IVisualElement, ValueCube> LastRenderPositions { get; }

        protected Dictionary<IVisualElement, ValueCube> RenderPositions { get; }

        protected abstract ValueRectangle GetCurrentClip();



        protected void OnDrawBorder<TRectangle,TThickness>(TRectangle rect,
                                                           IBoxValue<IBrush> color,
                                                           //IBoxValue<>
                                                                            //VisualBorder border,
                                                                            ValueThickness thickness,
                                                                            TThickness cornerRadius)
            where TRectangle : IRectangle
            where TThickness : IThickness
        {
            if (!cornerRadius.IsEmpty)
            {
                if (!(color.Bottom is SolidColorBrush scb))
                    throw new NotImplementedException();

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
            FillRectangle(leftRect, color.Left);

            var topRect = new ValueRectangle(topLeftOutside, sumWidth, thickness.Top);
            FillRectangle(topRect, color.Top);

            var rightRect = new ValueRectangle(rect.Right, topLeftOutside.Y, thickness.Right, sumHeight);
            FillRectangle(rightRect, color.Right);

            var bottomRect = new ValueRectangle(topLeftOutside.X, bottomRightInside.Y,
                sumWidth, thickness.Bottom);
            FillRectangle(bottomRect, color.Bottom);
        }


        //protected void OnDrawBorder<TRectangle, TShape, TBrush, TThickness>(TRectangle rect,
        //                                                                    TShape thickness,
        //                                                                    TBrush brush,
        //                                                                    TThickness cornerRadius)
        //    where TRectangle : IRectangle
        //    where TShape : IThickness
        //    where TBrush : IBrush
        //    where TThickness : IThickness
        //{
        //    if (!cornerRadius.IsEmpty)
        //    {
        //        if (!(brush is SolidColorBrush scb))
        //            throw new NotImplementedException();

        //        var p = new Pen(scb.Color, Convert.ToInt32(thickness.Left));

        //        DrawRoundedRect(rect, p, cornerRadius);
        //        return;
        //    }

        //    var sumHeight = rect.Height + thickness.Top + thickness.Bottom;
        //    var sumWidth = rect.Width + thickness.Left + thickness.Right;

        //    var topLeftOutside = new ValuePoint2D(rect.Left - thickness.Left,
        //        rect.Top - thickness.Top);

        //    var bottomRightInside = rect.BottomRight;

        //    var leftRect = new ValueRectangle(topLeftOutside, thickness.Left, sumHeight);
        //    FillRectangle(leftRect, brush);

        //    var topRect = new ValueRectangle(topLeftOutside, sumWidth, thickness.Top);
        //    FillRectangle(topRect, brush);

        //    var rightRect = new ValueRectangle(rect.Right, topLeftOutside.Y, thickness.Right, sumHeight);
        //    FillRectangle(rightRect, brush);

        //    var bottomRect = new ValueRectangle(topLeftOutside.X, bottomRightInside.Y,
        //        sumWidth, thickness.Bottom);
        //    FillRectangle(bottomRect, brush);
        //}

        protected abstract void PopClip<TRectangle>(TRectangle rect)
            where TRectangle : IRectangle;

        protected abstract void PushClip<TRectangle>(TRectangle rect)
            where TRectangle : IRectangle;

        private void PopRect()
        {
            _currentZ--;
            _locations.Pop();
            CurrentElementRect = _locations.Peek();
        }

        private void PushRect(ValueRenderRectangle rect)
        {
            _currentZ++;
            _locations.Push(rect);
            CurrentElementRect = rect;
        }

        //private void PushRect(RenderRectangle rect)
        //{
        //    _currentZ++;
        //    _locations.Push(rect);
        //    CurrentElementRect = rect;
        //}

        private void SetElementRenderPosition(//RenderRectangle useRect,
            ValueRenderRectangle useRect,
                                              IVisualElement element)
        {
            var currentClip = GetCurrentClip();

            if (currentClip.IsEmpty)
                RenderPositions[element] = new ValueCube(useRect, _currentZ);
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
        //private String _tabs = String.Empty;

        //[ThreadStatic]
        //private static RenderRectangle? _fairyRect;

        private readonly Dictionary<IVisualElement, ValueSize> _lastMeasurements;

        private readonly Stack<ValueRenderRectangle> _locations;
        protected readonly IBoxModel _boxModel;
        private readonly Object _renderLock;

        private Int32 _currentZ;
        protected ValueRenderRectangle CurrentElementRect;
    }
}
