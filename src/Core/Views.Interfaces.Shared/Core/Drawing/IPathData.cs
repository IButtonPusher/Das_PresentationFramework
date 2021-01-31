using System;
using Das.Views.Core.Geometry;

namespace Das.Views.Core.Drawing
{
    public interface IPathData
    {
        IPoint2F[] Points { get;  }

        Byte[] Types { get;  }
    }
}
