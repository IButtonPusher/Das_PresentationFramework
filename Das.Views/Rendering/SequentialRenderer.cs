﻿using System;
using System.Collections.Generic;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Styles;

namespace Das.Views.Rendering
{
    /// <summary>
    /// Renders a collection of elements vertically or horizontally
    /// </summary>
    public class SequentialRenderer : ISequentialRenderer
    {
        private readonly bool _isWrapContent;
        protected readonly Dictionary<IVisualElement, Rectangle> ElementsRendered;

        public SequentialRenderer(Boolean isWrapContent = false)
        {
            _isWrapContent = isWrapContent;
            ElementsRendered = new Dictionary<IVisualElement, Rectangle>();
        }

        public Size Measure(IVisualElement container, IEnumerable<IVisualElement> elements,
            Orientations orientation, ISize availableSpace, IMeasureContext measureContext)
        {
            var remainingSize = new Size(availableSpace.Width, availableSpace.Height);

            var current = new Rectangle();
            var totalHeight = 0.0;
            var totalWidth = 0.0;

            var maxWidth = 0.0;
            var maxHeight = 0.0;

            ElementsRendered.Clear();

            foreach (var child in elements)
            {
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

            var margin = measureContext.GetStyleSetter<Thickness>(StyleSetters.Margin, container);

            totalWidth = Math.Max(totalWidth, maxWidth);
            totalHeight = Math.Max(totalHeight, maxHeight);

            return new Size(totalWidth + margin.Width, totalHeight + margin.Height);
        }

        public void Arrange(Orientations orientation,
            ISize availableSpace, IRenderContext renderContext)
        {
            foreach (var kvp in ElementsRendered)
            {
                var child = kvp.Key;
                var current = ElementsRendered[child];
                switch (orientation)
                {
                    case Orientations.Vertical:
                        current = new Rectangle(current.X, current.Y, availableSpace.Width,
                            current.Height);
                        break;
                    case Orientations.Horizontal:
                        current = new Rectangle(current.X, current.Y, current.Width,
                            availableSpace.Height);
                        break;
                }

                var _ = renderContext.DrawElement(child, current);
            }
        }
    }
}