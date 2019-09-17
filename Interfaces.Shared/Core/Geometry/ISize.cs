using System;

namespace Das.Views.Core.Geometry
{
    public interface ISize : IEquatable<ISize>
    {
        Double Width { get; }
        Double Height { get; }

        Boolean IsEmpty { get; }
    }
}