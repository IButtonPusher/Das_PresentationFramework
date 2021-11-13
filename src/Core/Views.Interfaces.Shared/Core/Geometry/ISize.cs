using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Geometry
{
    public interface ISize : IEquatable<ISize>
    {
        Double Height { get; }

        Boolean IsEmpty { get; }

        Double Width { get; }
    }
}