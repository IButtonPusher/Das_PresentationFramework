using System;

namespace Das.Views.Core.Geometry
{
    public interface IRectangle : IShape2d, IPoint, IPointContainer
    {
        Point BottomLeft { get; }

        Point BottomRight { get; }

        Point TopLeft { get; }

        Point TopRight { get; }
    }
}