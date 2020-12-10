using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Panels;
using Das.Views.Rendering.Geometry;
using Das.Views.Styles;

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

        public virtual ValueSize Measure(IVisualElement container,
                                         Orientations orientation, 
                                 IRenderSize availableSpace, 
                                 IMeasureContext measureContext)
        {
            lock (_measureLock)
            {
                _currentlyRendering.Clear();

                var remainingSize = new RenderSize(availableSpace.Width,
                    availableSpace.Height, availableSpace.Offset);

                var current = new RenderRectangle();
                var totalHeight = 0.0;
                var totalWidth = 0.0;

                var maxWidth = 0.0;
                var maxHeight = 0.0;

                ElementsRendered.Clear();
                
                //for (var c = 0; c < elements.Count; c++)
                _visuals.RunOnEachChild(child =>
                {
                    //  var child = elements[c];
                    _currentlyRendering.Add(child);

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
                });

                var margin = measureContext.GetStyleSetter<Thickness>(StyleSetter.Margin, container);

                totalWidth = Math.Max(totalWidth, maxWidth);
                totalHeight = Math.Max(totalHeight, maxHeight);

                return new ValueSize(totalWidth + margin.Width, totalHeight + margin.Height);
            }
        }

        public virtual void Arrange(Orientations orientation,
                            IRenderRectangle bounds, 
                            IRenderContext renderContext)
        {
            var offset = bounds.Location;
            if (!offset.IsOrigin)
            {}

            //foreach (var kvp in GetRenderables(orientation, bounds, renderContext))
            //{
            //    renderContext.DrawElement(kvp.Key, kvp.Value);
            //}



            foreach (var kvp in GetRenderables(orientation, bounds, renderContext))
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
            IRenderRectangle bounds,
            IVisualContext context)
        {
            lock (_measureLock)
            {

                var current = ValueRenderRectangle.Empty;

                foreach (var child in _currentlyRendering)
                {
                    current = GetElementBounds(child, current);

                    current = GetElementRenderBounds(child, current, orientation,
                        bounds, context);

                    yield return new KeyValuePair<IVisualElement, ValueRenderRectangle>(child, current);
                }
            }
        }

        private static ValueRenderRectangle GetElementRenderBounds(IVisualElement child,
                                                                   ValueRenderRectangle current,
                                                                   Orientations orientation,
                                                                   IRenderRectangle bounds,
                                                                   IVisualContext context)
        {
            var useX = current.X;
            var useY = current.Y;

            switch (orientation)
            {
                case Orientations.Vertical:
                    // may need to adjust the X based on alignment

                    var useHorzAlign = child.HorizontalAlignment != HorizontalAlignments.Default
                        ? child.HorizontalAlignment
                        : context.GetStyleSetter<HorizontalAlignments>(StyleSetter.HorizontalAlignment, child);

                    

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

                    //useY = 0;
                    
                    var useVertAlign = child.VerticalAlignment != VerticalAlignments.Default
                        ? child.VerticalAlignment
                        : context.GetStyleSetter<VerticalAlignments>(StyleSetter.VerticalAlignment, child);

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

        //protected virtual ValueRenderRectangle GetCoreTargetRect(IVisualElement visual)
        //{
        //    return ElementsRendered[visual];
        //}

        //public Boolean TryGetRenderedElement(IVisualElement element,
        //                                     out ValueRenderRectangle pos)
        //{
        //    lock (_measureLock)
        //    {
        //        return ElementsRendered.TryGetValue(element, out pos);
        //    }
        //}

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