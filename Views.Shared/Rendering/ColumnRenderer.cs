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
        //private readonly Func<Int32, Double> _getRowHeight;
        private readonly Dictionary<Int32, Double> _rowHeights;
        private Int32 _currentRow;
        private Double _currentY;

        public ColumnRenderer(IVisualCollection visuals,
                              //Func<Int32, Double> getRowHeight,
                              Dictionary<Int32, Double> rowHeights) 
            : base(visuals)
        {
            //_getRowHeight = getRowHeight;
            _rowHeights = rowHeights;
            RowHeights = new Dictionary<Int32, Double>();
        }

        public override ValueSize Measure(IVisualElement container, 
                                          Orientations orientation, 
                                          IRenderSize availableSpace,
                                          IMeasureContext measureContext)
        {
            _currentRow = 0;
            _currentY = 0;
            return base.Measure(container, orientation, availableSpace, measureContext);
        }

       
        public override void Arrange(Orientations orientation, 
                                     IRenderRectangle bounds, 
                                     IRenderContext renderContext)
        {
            _currentRow = 0;
            _currentY = 0;
            base.Arrange(orientation, bounds, renderContext);
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
                    //useY += (currentRowHeight - consider.Height);
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
            
            return new ValueRenderRectangle(consider.X, useY, // consider.Y,
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
