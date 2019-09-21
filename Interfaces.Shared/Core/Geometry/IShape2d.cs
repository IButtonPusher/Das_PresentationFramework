using System;

namespace Das.Views.Core.Geometry
{
    public interface IShape2d : ISize
    {
        Double Left { get; }

        Double Top { get; }

        Double Right { get; }

        Double Bottom { get; }
    }
}