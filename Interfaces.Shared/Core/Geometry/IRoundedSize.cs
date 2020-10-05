using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Geometry
{
    public interface IRoundedSize : IEquatable<IRoundedSize>
    {
        Int32 Height { get; }

        Int32 Width { get; }
    }
}