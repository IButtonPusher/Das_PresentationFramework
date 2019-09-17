using System;

namespace Das.Views.Core.Geometry
{
    public interface IRoundedSize : IEquatable<IRoundedSize>
    {
        Int32 Width { get; }
        Int32 Height { get; }
    }
}
