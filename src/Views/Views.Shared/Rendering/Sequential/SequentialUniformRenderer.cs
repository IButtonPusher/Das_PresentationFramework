using System;
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

                //_currentlyRendering.Clear();

                var margin = MeasureImpl(container, measureContext, availableSpace,
                    orientation, //_currentlyRendering,
                    out var maxWidth, out var maxHeight,
                    out var totalWidth, out var totalHeight,
                    out var renderingVisualsCount);

                totalWidth = Math.Max(totalWidth, maxWidth);
                totalHeight = Math.Max(totalHeight, maxHeight);

                if (orientation == Orientations.Horizontal)
                   totalWidth = _maxWidth * renderingVisualsCount;//_currentlyRendering.Count;

                if (orientation == Orientations.Vertical)
                    totalHeight = _maxHeight * renderingVisualsCount;//_currentlyRendering.Count;

                return new ValueSize(totalWidth + margin.Width, totalHeight + margin.Height);
            }
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

        protected override Boolean TryGetElementBounds(IVisualElement child,
                                                                 ValueRenderRectangle precedingVisualBounds,
                                                                 out ValueRenderRectangle bounds)
        {
           bounds = new(_lastOrientation == Orientations.Vertical  // X
                 ? precedingVisualBounds.Left
                 : precedingVisualBounds.Right,
                
              _lastOrientation == Orientations.Vertical // Y
                 ? precedingVisualBounds.Bottom
                 : precedingVisualBounds.Top,

              _maxWidth, 
              _maxHeight, 
              ValuePoint2D.Empty);

           return true;
        }

        //protected override ValueRenderRectangle GetElementBounds(IVisualElement child,
        //                                                         ValueRenderRectangle precedingVisualBounds)
        //{
        //    return new(_lastOrientation == Orientations.Vertical  // X
        //            ? precedingVisualBounds.Left
        //        : precedingVisualBounds.Right,
                
        //        _lastOrientation == Orientations.Vertical // Y
        //            ? precedingVisualBounds.Bottom
        //            : precedingVisualBounds.Top,

        //        _maxWidth, 
        //        _maxHeight, 
        //        ValuePoint2D.Empty);
        //}


        private Double _maxWidth;
        private Double _maxHeight;
        private Orientations _lastOrientation;

    }
}
