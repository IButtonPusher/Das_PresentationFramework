using System.Collections.Generic;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;

namespace Das.Views.Extended.Runtime
{
    public class LineFrame : IFrame
    {
        public LineFrame(IList<IMultiLine> triangles, ISize size)
        {
            Triangles = triangles;
            Size = size;
        }

        public ISize Size { get; }
        public IList<IMultiLine> Triangles { get; }
    }
}
