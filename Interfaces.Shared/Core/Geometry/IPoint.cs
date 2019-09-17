using System;

namespace Das.Views.Core.Geometry
{
    public interface IPoint : IDeepCopyable<IPoint>
    {
        Double X { get; }
        Double Y { get; }
    }
}