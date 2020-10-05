using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Geometry
{
    public interface IPointContainer
    {
        Point2D Location { get; }

        ISize Size { get; }

        Boolean Contains(IPoint2D point2D);

        Boolean Contains(Int32 x, Int32 y);

        Boolean Contains(Double x, Double y);
    }
}