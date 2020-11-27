using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Rendering.Geometry;
using Das.Views.Styles;

namespace Das.Views.Rendering
{
    /// <summary>
    ///     Renders a collection of elements vertically or horizontally
    /// </summary>
    public class SequentialRenderer : ISequentialRenderer
    {
        public SequentialRenderer(Boolean isWrapContent = false)
        {
            _measureLock = new Object();
            _isWrapContent = isWrapContent;
            ElementsRendered = new Dictionary<IVisualElement, RenderRectangle>();
        }

        public ValueSize Measure(IVisualElement container, 
                                 IList<IVisualElement> elements,
                                 Orientations orientation, 
                                 IRenderSize availableSpace, 
                                 IMeasureContext measureContext)
        {
            lock (_measureLock)
            {
                var remainingSize = new RenderSize(availableSpace.Width,
                    availableSpace.Height, availableSpace.Offset);

                var current = new RenderRectangle();
                var totalHeight = 0.0;
                var totalWidth = 0.0;

                var maxWidth = 0.0;
                var maxHeight = 0.0;

                ElementsRendered.Clear();

                //foreach (var child in elements)
                for (var c = 0; c < elements.Count; c++)
                {
                    var child = elements[c];
                    current.Size = measureContext.MeasureElement(child, remainingSize);
                    ElementsRendered[child] = current.DeepCopy();

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

                var margin = measureContext.GetStyleSetter<Thickness>(StyleSetter.Margin, container);

                totalWidth = Math.Max(totalWidth, maxWidth);
                totalHeight = Math.Max(totalHeight, maxHeight);

                return new ValueSize(totalWidth + margin.Width, totalHeight + margin.Height);
            }
        }

        public void Arrange(Orientations orientation,
                            IRenderRectangle bounds, 
                            IRenderContext renderContext)
        {

            foreach (var kvp in ElementsRendered)
            {
                var child = kvp.Key;
                var current = ElementsRendered[child];
                switch (orientation)
                {
                    case Orientations.Vertical:
                        current = new RenderRectangle(current.X, current.Y, bounds.Width,
                            current.Height, current.Offset);
                        break;
                    case Orientations.Horizontal:
                        current = new RenderRectangle(current.X + bounds.X, current.Y, current.Width,
                            bounds.Height, current.Offset);
                        break;
                }

                renderContext.DrawElement(child, current);
            }
        }

        public Boolean TryGetRenderedElement(IVisualElement element,
                                             out RenderRectangle pos)
        {
            lock (_measureLock)
            {
                return ElementsRendered.TryGetValue(element, out pos);
            }
        }

        private readonly Boolean _isWrapContent;
        private readonly Object _measureLock;
        public Dictionary<IVisualElement, RenderRectangle> ElementsRendered { get; }
    }
}