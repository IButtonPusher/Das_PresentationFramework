using System.Collections.Generic;
using Das.Views.Core.Geometry;

namespace Das.Views.Core.Drawing
{
    /// <summary>
    /// Two dimensional data from a three dimensional scene as observed by a camera.
    /// </summary>
    public interface IFrame
    {
        ISize Size { get; }

        IList<IMultiLine> Triangles { get; }
    }
}
