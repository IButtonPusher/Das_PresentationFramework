using System;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering
{
    public interface IRenderRectangle : IRectangle,
                                        IRenderSize
    {
        new IRenderSize Size { get; }
    }
}
