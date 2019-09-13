using System;

namespace Das.Views.Core.Geometry
{
    public interface IPointContainer
    {
        ISize Size { get; }

        Point Location { get; }

        Boolean Contains(IPoint point);

        Boolean Contains(Int32 x, Int32 y);

        Boolean Contains(Double x, Double y);
    }
}