using System;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering
{
    public interface IRenderSize : ISize,
                                   IEquatable<IRenderSize>
                                   
    {
        IPoint2D Offset { get; } 

        new IRenderSize Reduce(Thickness padding);

        new IRenderSize DeepCopy();
        
        new IRenderSize Minus(ISize subtract);
    }
}
