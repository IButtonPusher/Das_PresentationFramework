using System;
using System.Collections.Generic;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Panels;
using Das.Views.Rendering.Geometry;

namespace Das.Views.Rendering
{
    public class ColumnRenderer : SequentialRenderer
    {
        private readonly Func<Int32, Double> _getRowHeight;
        private Int32 _currentRow;

        public ColumnRenderer(IVisualCollection visuals,
                              Func<Int32, Double> getRowHeight) 
            : base(visuals)
        {
            _getRowHeight = getRowHeight;
            RowHeights = new Dictionary<Int32, Double>();
        }

        public override ValueSize Measure(IVisualElement container, 
                                          Orientations orientation, 
                                          IRenderSize availableSpace,
                                          IMeasureContext measureContext)
        {
            _currentRow = 0;
            return base.Measure(container, orientation, availableSpace, measureContext);
        }

        public override void Arrange(Orientations orientation, 
                                     IRenderRectangle bounds, 
                                     IRenderContext renderContext)
        {
            _currentRow = 0;
            base.Arrange(orientation, bounds, renderContext);
        }

        protected override ValueRenderRectangle GetElementBounds(IVisualElement child, 
                                                                 ValueRenderRectangle precedingVisualBounds)
        {
            var consider = base.GetElementBounds(child, precedingVisualBounds);
            var useHeight = _getRowHeight(_currentRow);
            _currentRow++;
            return new ValueRenderRectangle(consider.X, consider.Y,
                new ValueSize(consider.Width, useHeight), consider.Offset);
        }

        protected override ValueSize SetChildSize(IVisualElement child, 
                                                  RenderRectangle current)
        {
            RowHeights[_currentRow] = current.Height;
            _currentRow++;
            return base.SetChildSize(child, current);
        }

        public Dictionary<Int32, Double> RowHeights {get;}
    }
}
