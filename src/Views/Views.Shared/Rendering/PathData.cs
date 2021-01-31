using System;
using System.Collections.Generic;
using System.Linq;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering
{
    public class PathData : IPathData
    {
        public PathData(IEnumerable<IPoint2F> points,
                        IEnumerable<Byte> types)
        {
            Points = points.ToArray();
            Types = types.ToArray();
        }

        public IPoint2F[] Points { get; }

        public Byte[] Types { get;  }
    }
}
