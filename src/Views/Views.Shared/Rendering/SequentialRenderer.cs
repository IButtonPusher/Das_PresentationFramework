using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Panels;
using Das.Views.Rendering.Geometry;

namespace Das.Views.Rendering
{
    /// <summary>
    ///     Renders a collection of elements vertically or horizontally
    /// </summary>
    public class SequentialRenderer : ISequentialRenderer,
                                      IDisposable
    {
        public SequentialRenderer(IVisualCollection visuals,
            Boolean isWrapContent = false)
        {
            _measureLock = new Object();
            _currentlyRendering = new List<IVisualElement>();
            _visuals = visuals;
            _isWrapContent = isWrapContent;
            ElementsRendered = new Dictionary<IVisualElement, ValueRenderRectangle>();
        }

        protected ValueThickness MeasureImpl(IVisualElement container,
                                        IMeasureContext measureContext,
                                        IRenderSize availableSpace,
                                        Orientations orientation,
                                        List<IVisualElement> currentlyRendering,
                                        out Double maxWidth,
                                        out Double maxHeight,
                                        out Double totalWidth,
                                        out Double totalHeight)
        {
            var remainingSize = new RenderSize(availableSpace.Width,
                availableSpace.Height, availableSpace.Offset);
            var current = new RenderRectangle();

            totalHeight = 0.0;
            totalWidth = 0.0;

            maxWidth = 0.0;
            maxHeight = 0.0;

            foreach (var child in _visuals.GetAllChildren())
            {
                currentlyRendering.Add(child);


                current.Size = measureContext.MeasureElement(child, remainingSize);
                var offset = SetChildSize(child, current);
                if (!offset.IsEmpty)
                {
                    current.Width += offset.Width;
                    current.Height += offset.Height;
                }

                switch (orientation)
                {
                    case Orientations.Horizontal:
                        if (current.Height > maxHeight)
                            maxHeight = current.Height;

                        if (_isWrapContent && current.Width + totalWidth > availableSpace.Width
                                           && totalHeight + maxHeight < availableSpace.Height)
                        {
                            maxWidth = Math.Max(maxWidth, totalWidth);
                            totalHeight += maxHeight;

                            current.X = 0;
                            current.Y += maxHeight;
                            maxHeight = totalWidth = 0;
                        }

                        current.X += current.Width;
                        totalWidth += current.Width;
                        remainingSize.Width -= current.Width;
                        break;
                    case Orientations.Vertical:
                        if (current.Width > totalWidth)
                            totalWidth = current.Width;

                        if (_isWrapContent && current.Height + totalHeight > availableSpace.Height
                                           && totalWidth + maxWidth < availableSpace.Width)
                        {
                            maxHeight = Math.Max(maxHeight, totalHeight);
                            totalWidth += maxWidth;

                            current.Y = 0;
                            current.X += maxHeight;
                            maxWidth = totalHeight = 0;
                        }

                        current.Y += current.Height;
                        totalHeight += current.Height;
                        remainingSize.Height -= current.Height;
                        break;
                }
            }

            var margin = container.Margin.GetValue(availableSpace);
            return margin;
        }

        public virtual ValueSize Measure(IVisualElement container,
                                         Orientations orientation,
                                         IRenderSize availableSpace,
                                         IMeasureContext measureContext)
        {
            lock (_measureLock)
            {
                _currentlyRendering.Clear();

                var margin = MeasureImpl(container, measureContext, availableSpace,
                    orientation, _currentlyRendering,
                    out var maxWidth, out var maxHeight,
                    out var totalWidth, out var totalHeight);

                //var remainingSize = new RenderSize(availableSpace.Width,
                //    availableSpace.Height, availableSpace.Offset);

                //var current = new RenderRectangle();
                //var totalHeight = 0.0;
                //var totalWidth = 0.0;

                //var maxWidth = 0.0;
                //var maxHeight = 0.0;

                //ElementsRendered.Clear();

                //foreach (var child in _visuals.GetAllChildren())
                //{
                //    _currentlyRendering.Add(child);

                //    current.Size = measureContext.MeasureElement(child, remainingSize);
                //    var offset = SetChildSize(child, current);
                //    if (!offset.IsEmpty)
                //    {
                //        current.Width += offset.Width;
                //        current.Height += offset.Height;
                //    }

                //    switch (orientation)
                //    {
                //        case Orientations.Horizontal:
                //            if (current.Height > maxHeight)
                //                maxHeight = current.Height;

                //            if (_isWrapContent && current.Width + totalWidth > availableSpace.Width
                //                               && totalHeight + maxHeight < availableSpace.Height)
                //            {
                //                maxWidth = Math.Max(maxWidth, totalWidth);
                //                totalHeight += maxHeight;

                //                current.X = 0;
                //                current.Y += maxHeight;
                //                maxHeight = totalWidth = 0;
                //            }

                //            current.X += current.Width;
                //            totalWidth += current.Width;
                //            remainingSize.Width -= current.Width;
                //            break;
                //        case Orientations.Vertical:
                //            if (current.Width > totalWidth)
                //                totalWidth = current.Width;

                //            if (_isWrapContent && current.Height + totalHeight > availableSpace.Height
                //                               && totalWidth + maxWidth < availableSpace.Width)
                //            {
                //                maxHeight = Math.Max(maxHeight, totalHeight);
                //                totalWidth += maxWidth;

                //                current.Y = 0;
                //                current.X += maxHeight;
                //                maxWidth = totalHeight = 0;
                //            }

                //            current.Y += current.Height;
                //            totalHeight += current.Height;
                //            remainingSize.Height -= current.Height;
                //            break;
                //    }
                //}

                //var margin = container.Margin.GetValue(availableSpace);

                return new ValueSize(Math.Max(totalWidth, maxWidth) + margin.Width,
                    Math.Max(totalHeight, maxHeight) + margin.Height);
            }
        }

        public virtual void Arrange(Orientations orientation,
                                    IRenderRectangle bounds, 
                                    IRenderContext renderContext)
        {
            var offset = bounds.Location;


            foreach (var kvp in GetRenderables(orientation, bounds))
            {
                if (offset.IsOrigin)
                    renderContext.DrawElement(kvp.Key, kvp.Value);
                else
                    renderContext.DrawElement(kvp.Key, kvp.Value.Move(offset));
            }
        }

        protected virtual ValueSize SetChildSize(IVisualElement child,
                                            RenderRectangle current)
        {
            ElementsRendered[child] = new ValueRenderRectangle(current);

            return ValueSize.Empty;
        }

        protected virtual ValueRenderRectangle GetElementBounds(IVisualElement child,
                                                                ValueRenderRectangle precedingVisualBounds)
        {
            return ElementsRendered[child];
        }

        protected virtual IEnumerable<KeyValuePair<IVisualElement, ValueRenderRectangle>> GetRenderables(
            Orientations orientation,
            IRenderRectangle bounds )
        {
            lock (_measureLock)
            {

                var current = ValueRenderRectangle.Empty;

                foreach (var child in _currentlyRendering)
                {
                    current = GetElementBounds(child, current);

                    current = GetElementRenderBounds(child, current, orientation,
                        bounds);

                    yield return new KeyValuePair<IVisualElement, ValueRenderRectangle>(child, current);
                }
            }
        }

        private static ValueRenderRectangle GetElementRenderBounds(IVisualElement child,
                                                                   ValueRenderRectangle current,
                                                                   Orientations orientation,
                                                                   IRenderRectangle bounds)
        {
            var useX = current.X;
            var useY = current.Y;

            switch (orientation)
            {
                case Orientations.Vertical:
                    // may need to adjust the X based on alignment

                    var useHorzAlign = child.HorizontalAlignment;

                    switch (useHorzAlign)
                    {
                        case HorizontalAlignments.Right:
                            useX += bounds.Width - current.Width;
                            break;

                        case HorizontalAlignments.Center:
                            useX += (bounds.Width - current.Width) / 2;
                            break;
                        
                        case HorizontalAlignments.Left:
                        case HorizontalAlignments.Default:
                        case HorizontalAlignments.Stretch:
                            
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;

                case Orientations.Horizontal:

                    var useVertAlign = child.VerticalAlignment;

                    switch (useVertAlign)
                    {
                        case VerticalAlignments.Bottom:
                            useY += bounds.Height - current.Height;
                            break;

                        case VerticalAlignments.Center:
                            useY += (bounds.Height - current.Height) / 2;
                            break;
                        
                        case VerticalAlignments.Top:
                        case VerticalAlignments.Default:
                        case VerticalAlignments.Stretch:
                            
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
            }

            current = new ValueRenderRectangle(useX, useY, 
                //bounds.Width, //this draws things (picture frame) too wide
                current.Width,
                current.Height, current.Offset);

            return current;
        }

        protected readonly IVisualCollection _visuals;
        protected readonly Boolean _isWrapContent;
        protected readonly List<IVisualElement> _currentlyRendering;
        protected readonly Object _measureLock;
        private Dictionary<IVisualElement, ValueRenderRectangle> ElementsRendered { get; }

        public virtual void Dispose()
        {
            _currentlyRendering.Clear();
            ElementsRendered.Clear();
        }
    }
}