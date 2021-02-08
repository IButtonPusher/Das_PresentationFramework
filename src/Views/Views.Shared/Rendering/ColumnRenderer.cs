using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Panels;
using Das.Views.Rendering.Geometry;

namespace Das.Views.Rendering
{
    public class ColumnRenderer : SequentialRenderer
    {
        public ColumnRenderer(IVisualCollection visuals,
                              Dictionary<Int32, Double> rowHeights)
            : base(visuals)
        {
            _rowHeights = rowHeights;
            RowHeights = new Dictionary<Int32, Double>();
        }

        public Dictionary<Int32, Double> RowHeights { get; }


        public override void Arrange<TRenderRect>(Orientations orientation,
                                                  TRenderRect bounds,
                                                 IRenderContext renderContext)
        {
            _currentRow = 0;
            _currentY = 0;
            base.Arrange(orientation, bounds, renderContext);
        }

        public override ValueSize Measure<TRenderSize>(IVisualElement container,
                                                      Orientations orientation,
                                                      TRenderSize availableSpace,
                                                      IMeasureContext measureContext)
        {
            _currentRow = 0;
            _currentY = 0;
            RowHeights.Clear();
            return base.Measure(container, orientation, availableSpace, measureContext);
        }

        protected override ValueRenderRectangle GetElementBounds(IVisualElement child,
                                                                 ValueRenderRectangle precedingVisualBounds)
        {
            var useY = _currentY;
            var topGap = 0.0;

            var consider = base.GetElementBounds(child, precedingVisualBounds);
            var currentRowHeight = _rowHeights[_currentRow];

            switch (child.VerticalAlignment)
            {
                case VerticalAlignments.Top:
                case VerticalAlignments.Stretch:
                    break;

                case VerticalAlignments.Bottom:
                    topGap = currentRowHeight - consider.Height;
                    break;

                case VerticalAlignments.Center:
                case VerticalAlignments.Default:
                    topGap = (currentRowHeight - consider.Height) / 2;

                    break;


                default:
                    throw new ArgumentOutOfRangeException();
            }

            var useHeight = currentRowHeight - topGap;
            useY += topGap;


            _currentRow++;
            _currentY += currentRowHeight;

            return new ValueRenderRectangle(consider.X, useY, 
                new ValueSize(consider.Width, useHeight), consider.Offset);
        }

        protected override ValueSize SetChildSize(IVisualElement child,
                                                  RenderRectangle current)
        {
            RowHeights[_currentRow] = current.Height;
            _currentRow++;
            return base.SetChildSize(child, current);
        }

        private readonly Dictionary<Int32, Double> _rowHeights;
        private Int32 _currentRow;
        private Double _currentY;
    }
}