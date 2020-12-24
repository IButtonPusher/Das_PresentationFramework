using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Geometry
{
    public interface IRectangle : IShape2d,
                                  IPointContainer,
                                  IEquatable<IRectangle>
    {
        Point2D BottomLeft { get; }

        Point2D BottomRight { get; }

        Point2D TopLeft { get; }

        Point2D TopRight { get; }

        Double X { get; }

        Double Y { get; }

        void Union(IRectangle rect);
    }
}