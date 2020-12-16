using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Das.Views.Core.Geometry
{
    public interface IMultiLine : IEnumerable<IPoint2D>
    {
        IPoint2D[] PointArray { get; }
    }
}