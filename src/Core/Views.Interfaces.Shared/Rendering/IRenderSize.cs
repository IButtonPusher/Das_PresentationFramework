using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Rendering.Geometry;

namespace Das.Views.Rendering
{
    public interface IRenderSize : ISize,
                                   IEquatable<IRenderSize>

    {
        IPoint2D Offset { get; }

        //new IRenderSize DeepCopy();

        //new IRenderSize Minus(ISize subtract);

        //IRenderSize MinusVertical(ISize subtract);

        //new IRenderSize PlusVertical(ISize adding);

        //new IRenderSize Reduce(Thickness padding);

        ValueRenderRectangle ToFullRectangle();

        ValueSize ToValueSize();
    }
}