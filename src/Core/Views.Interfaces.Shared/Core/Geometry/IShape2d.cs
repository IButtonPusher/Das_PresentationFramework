using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Geometry
{
    public interface IShape2d : ISize
    {
        Double Bottom { get; }

        Double Left { get; }

        Double Right { get; }

        Double Top { get; }
    }
}