using System;
using System.Collections.Generic;
using Das.Views.Core.Geometry;

namespace Das.Views.Geometry
{
    public class VisualPercentThickness : IThickness
    {
        private readonly IVisualElement _visual;
        private readonly Double _leftPercent;
        private readonly Double _topPercent;
        private readonly Double _rightPercent;
        private readonly Double _bottomPercent;
        private readonly Dictionary<IVisualElement, ValueCube> _renderPositions;

        public VisualPercentThickness(IVisualElement visual,
                                      Double leftPercent,
                                      Double topPercent,
                                      Double rightPercent,
                                      Double bottomPercent,
                                      Dictionary<IVisualElement, ValueCube> renderPositions)
        {
            _visual = visual;
            _leftPercent = leftPercent;
            _topPercent = topPercent;
            _rightPercent = rightPercent;
            _bottomPercent = bottomPercent;
            
            _renderPositions = renderPositions;
        }

        public Double Bottom => _renderPositions.TryGetValue(_visual, out var rect) 
            ? rect.Bottom  * _bottomPercent
            : 0;

        public Double Left => _renderPositions.TryGetValue(_visual, out var rect) 
            ? rect.Left  * _leftPercent
            : 0;

        public Double Right => _renderPositions.TryGetValue(_visual, out var rect) 
            ? rect.Right  * _rightPercent
            : 0;

        public Double Top => _renderPositions.TryGetValue(_visual, out var rect) 
            ? rect.Top  * _topPercent
            : 0;

        public override String ToString()
        {
            return $"{Left} {Top} {Right} {Bottom} % ";
        }
    }
}
