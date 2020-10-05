using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Das.Views.Core.Geometry
{
    public interface IRoundedRectangle : IPointContainer
    {
        Int32 Height { get; }

        Int32 Width { get; }

        Int32 X { get; }

        Int32 Y { get; }

        IRoundedRectangle GetUnion(IRoundedRectangle other);

        IRoundedRectangle GetUnion(IEnumerable<IRoundedRectangle> others);
    }
}