using System;

namespace Das.Views.Core.Geometry
{
    public interface IThickness : ISize
    {
        Double Bottom { get; }

        Double Left { get; }

        Double Right { get; }

        Double Top { get; }

        //Double Width { get; }

        //Double Height { get; }

        //Boolean IsEmpty { get; }
    }
}
