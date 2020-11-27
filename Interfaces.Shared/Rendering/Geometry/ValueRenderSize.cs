using System;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;

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

        public ValueRenderSize(ISize size,
                               IPoint2D offset,
                               Thickness? padDown)
        {
            if (padDown == null || padDown.IsEmpty)
            {
                Height = size.Height;
                Width = size.Width;
            }
            else
            {
                Height = size.Height - padDown.Height;
                Width = size.Width - padDown.Width;
            }

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

        IRenderSize IRenderSize.PlusVertical(ISize adding)
        {
            return GeometryHelper.PlusRenderVertical(this, adding);
        }

        public IRenderSize Reduce(Thickness padding)
        {
            return GeometryHelper.Reduce(this, padding);
        }

        public ValueRenderRectangle ToFullRectangle()
        {
            return new ValueRenderRectangle(Point2D.Empty, this, Offset);
        }

        public ValueSize ToValueSize()
        {
            return GeometryHelper.ToValueSize(this);
        }

        ISize IDeepCopyable<ISize>.DeepCopy()
        {
            return DeepCopy();
        }

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

        public IRenderSize MinusVertical(ISize subtract)
        {
            return GeometryHelper.MinusVertical(this, subtract);
        }

        public ISize PlusVertical(ISize adding)
        {
            return GeometryHelper.PlusVertical(this, adding);
        }

        public IRenderSize Minus(ISize subtract)
        {
            return GeometryHelper.Minus(this, subtract);
        }

        ISize ISize.Minus(ISize subtract)
        {
            return Minus(subtract);
        }

        public Double CenterY(ISize item)
        {
            return GeometryHelper.CenterY(this, item);
        }

        public Double CenterX(ISize item)
        {
            return GeometryHelper.CenterX(this, item);
        }


        public IPoint2D Offset { get; }

        public Boolean Equals(IRenderSize other)
        {
            return false;
        }

        public override String ToString()
        {
            return "Width: " + Width + " Height: " + Height;
        }

        public static readonly ValueRenderSize Empty = new ValueRenderSize(0, 0);
    }
}