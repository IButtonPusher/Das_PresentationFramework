using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Geometry
{
    public interface IRectangle : IShape2d,
                                  IPointContainer,
                                  IEquatable<IRectangle>
    {
        IPoint2D BottomLeft { get; }

        IPoint2D BottomRight { get; }

        IPoint2D TopLeft { get; }

        IPoint2D TopRight { get; }

        Double X { get; }

        Double Y { get; }

        void Union(IRectangle rect);
    }
}