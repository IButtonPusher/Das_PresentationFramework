using System;
using System.Collections.Generic;

namespace Das.Views.Core.Geometry
{
    public interface IMultiLine : IEnumerable<IPoint>
    {
        IPoint[] PointArray { get; }
    }
}