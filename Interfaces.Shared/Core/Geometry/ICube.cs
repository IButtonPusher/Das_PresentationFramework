using System;

namespace Das.Views.Core.Geometry
{
    public interface ICube : IRectangle
    {
        Double Depth { get; }
    }
}