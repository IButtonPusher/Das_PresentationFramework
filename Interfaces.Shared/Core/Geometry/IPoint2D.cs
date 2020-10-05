using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Geometry
{
    public interface IPoint2D : IDeepCopyable<IPoint2D>
    {
        Double X { get; }

        Double Y { get; }
    }
}