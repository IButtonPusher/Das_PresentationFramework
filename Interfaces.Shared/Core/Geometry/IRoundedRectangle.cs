using System;

namespace Das.Views.Core.Geometry
{
    public interface IRoundedRectangle : IPointContainer
    {
        Int32 X { get; }

        Int32 Y { get; }

        Int32 Width { get; }

        Int32 Height { get; }
    }
}