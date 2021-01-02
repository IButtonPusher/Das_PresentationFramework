using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering
{
    public interface IRenderRectangle : IRectangle,
                                       IRenderSize,
                                        IEquatable<IRenderRectangle>
    {
        new IRenderSize Size { get; }

        TRectangle Reduce<TRectangle>(Double left,
                                      Double top,
                                      Double right,
                                      Double bottom)
            where TRectangle : IRenderRectangle;
    }
}