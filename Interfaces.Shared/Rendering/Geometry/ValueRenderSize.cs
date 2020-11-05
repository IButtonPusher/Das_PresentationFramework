using System;
using Das.Extensions;
using Das.Views.Rendering;

namespace Das.Views.Core.Geometry
{
    public readonly struct ValueRenderSize : IRenderSize
    {
        public ValueRenderSize(Double width, 
                               Double height,
                               IPoint2D offset)
        {
            Height = height;
            Width = width;
            Offset = offset;
        }

        public ValueRenderSize(Double width, 
                               Double height)
        : this(width, height, Point2D.Empty)
        {
        }

        public ValueRenderSize(ISize size) 
            : this(size.Width, size.Height, Point2D.Empty)
        {
            
        }

        public Boolean Equals(ISize other)
        {
            return GeometryHelper.AreSizesEqual(this, other);
        }

        public IRenderSize Reduce(Thickness padding)
        {
            return GeometryHelper.Reduce(this, padding);
        }

        

        ISize IDeepCopyable<ISize>.DeepCopy() => DeepCopy();

        public IRenderSize DeepCopy()
        {
            return new ValueRenderSize(Width, Height, Offset);
        }

        public Double Height { get; }

        public Boolean IsEmpty => Width.IsZero() || Height.IsZero();

        public Double Width { get; }

        ISize ISize.Reduce(Thickness padding)
        {
            return Reduce(padding);
        }

        public IRenderSize Minus(ISize subtract)
        {
            return GeometryHelper.Minus(this, subtract);
        }

        ISize ISize.Minus(ISize subtract) => Minus(subtract);
        

        public IPoint2D Offset { get; }

        public Boolean Equals(IRenderSize other)
        {
            return false;
        }
    }
}
