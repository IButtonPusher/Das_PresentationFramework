﻿using System;
using Das.Extensions;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Panels;
using Das.Views.Rendering.Geometry;

namespace Das.Views.Rendering
{
    /// <summary>
    /// Renders a collection of visuals vertically or horizontally.
    /// Uses the largest measured size for every visual
    /// </summary>
    public class SequentialUniformRenderer : SequentialRenderer
    {
        public SequentialUniformRenderer(IVisualCollection visuals,
                                         Boolean isWrapContent = false)
            : base(visuals, isWrapContent)
        {
            _maxHeight = _maxWidth = 0;
            _lastOrientation = Orientations.Vertical;
        }

        public override ValueSize Measure<TRenderSize>(IVisualElement container,
                                                      Orientations orientation, 
                                                      TRenderSize availableSpace,
                                                      IMeasureContext measureContext)
        {
            lock (_measureLock)
            {
                _maxWidth = _maxHeight = 0;
                _lastOrientation = orientation;

                _currentlyRendering.Clear();

                //var remainingSize = new RenderSize(availableSpace.Width,
                //    availableSpace.Height, availableSpace.Offset);
                //var current = new RenderRectangle();
                
                //var totalHeight = 0.0;
                //var totalWidth = 0.0;

                //var maxWidth = 0.0;
                //var maxHeight = 0.0;

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

                var margin = MeasureImpl(container, measureContext, availableSpace,
                    orientation, _currentlyRendering,
                    out var maxWidth, out var maxHeight,
                    out var totalWidth, out var totalHeight);

                totalWidth = Math.Max(totalWidth, maxWidth);
                totalHeight = Math.Max(totalHeight, maxHeight);

                if (orientation == Orientations.Horizontal)
                    totalWidth = _maxWidth * _currentlyRendering.Count;

                if (orientation == Orientations.Vertical)
                    totalHeight = _maxHeight * _currentlyRendering.Count;

                return new ValueSize(totalWidth + margin.Width, totalHeight + margin.Height);


            }

            //return base.Measure(container, elements, orientation, availableSpace, measureContext);
        }

        protected override ValueSize SetChildSize(IVisualElement child, 
                                             RenderRectangle current)
        {
            var diffX = current.Width - _maxWidth;
            var diffY = current.Height - _maxHeight;

            if (diffX > 0)
            {
                _maxWidth = current.Width;
                diffX = 0;
            }

            if (diffY > 0)
            {
                _maxHeight = current.Height;
                diffY = 0;
            }

            if (diffX.IsZero() && diffY.IsZero())
                return ValueSize.Empty;

            return new ValueSize(0 - diffX, 0 - diffY);

        }

        protected override ValueRenderRectangle GetElementBounds(IVisualElement child,
                                                                 ValueRenderRectangle precedingVisualBounds)
        {
            return new(_lastOrientation == Orientations.Vertical  // X
                    ? precedingVisualBounds.Left
                : precedingVisualBounds.Right,
                
                _lastOrientation == Orientations.Vertical // Y
                    ? precedingVisualBounds.Bottom
                    : precedingVisualBounds.Top,

                _maxWidth, 
                _maxHeight, 
                ValuePoint2D.Empty);
        }


        private Double _maxWidth;
        private Double _maxHeight;
        private Orientations _lastOrientation;

    }
}
