using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering.Geometry
{
    public readonly struct ValueRenderRectangle : IRenderRectangle
    {
        public ValueRenderRectangle(IRectangle r)
            : this(r.X, r.Y, r.Size, Point2D.Empty)
        {
        }

        public ValueRenderRectangle(Double x,
                                    Double y,
                                    ISize size,
                                    IPoint2D offset)
            : this(x, y, size.Width, size.Height, offset)
        {
        }

        public ValueRenderRectangle(IPoint2D position,
                                    ISize size,
                                    IPoint2D offset)
            : this(position.X, position.Y, size.Width, size.Height, offset)
        {
        }

        public ValueRenderRectangle(IPoint2D position,
                                    Double width,
                                    Double height,
                                    IPoint2D offset)
            : this(position.X, position.Y, width, height, offset)
        {
        }

        public ValueRenderRectangle(Double x,
                                    Double y,
                                    Double width,
                                    Double height,
                                    IPoint2D offset)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Offset = offset;
        }

        public Point2D BottomLeft => new Point2D(Left, Top + Height);

        public Point2D BottomRight => new Point2D(Left + Width, Top + Height);

        public Point2D TopLeft => new Point2D(Left, Top);

        public Point2D TopRight => new Point2D(Left + Width, Top);

        public Double X { get; }

        public Double Y { get; }

        public Double Bottom => Y + Height;

        public Double Left => X;

        public Double Right => X + Width;

        public Double Top => Y;

        public Double Height { get; }

        public Boolean IsEmpty => Width > 0 || Height > 0;

        public Double Width { get; }

        public IPoint2D Offset { get; }

        public IRenderSize MinusVertical(ISize subtract)
        {
            return GeometryHelper.MinusVertical(this, subtract);
        }

        public ValueSize ToValueSize()
        {
            return GeometryHelper.ToValueSize(this);
        }

        ISize ISize.PlusVertical(ISize adding)
        {
            return PlusVertical(adding);
        }

        ISize ISize.Reduce(Thickness padding)
        {
            return GeometryHelper.Reduce(this, padding);
        }

        public ValueRenderRectangle ToFullRectangle()
        {
            return this;
        }

        public IRenderSize PlusVertical(ISize adding)
        {
            return GeometryHelper.PlusRenderVertical(this, adding);
        }

        IRenderSize IRenderSize.DeepCopy()
        {
            return new RenderSize(Width, Height, Offset);
        }

        public IRenderSize Reduce(Thickness padding)
        {
            return GeometryHelper.Reduce(this, padding);
        }

        ISize ISize.Minus(ISize subtract)
        {
            return GeometryHelper.Minus(this, subtract);
        }

        public IRenderSize Minus(ISize subtract)
        {
            return GeometryHelper.Minus(this, subtract);
        }

        public Point2D Location => TopLeft;

        public IRenderSize Size => new ValueRenderSize(Width, Height, Offset);

        ISize IPointContainer.Size => Size;

        public Boolean Contains(IPoint2D point2D)
        {
            return GeometryHelper.IsRectangleContains(this, point2D);
        }

        public Boolean Contains(Int32 x, Int32 y)
        {
            return GeometryHelper.IsRectangleContains(this, x, y);
        }

        public Boolean Contains(Double x, Double y)
        {
            return GeometryHelper.IsRectangleContains(this, x, y);
        }

        public Boolean Equals(ISize other)
        {
            return GeometryHelper.AreSizesEqual(this, other);
        }

        void IRectangle.Union(IRectangle rect)
        {
            throw new NotSupportedException();
        }

        ISize IDeepCopyable<ISize>.DeepCopy()
        {
            return new ValueSize(Width, Height);
        }

        Boolean IEquatable<IRenderSize>.Equals(IRenderSize other)
        {
            return false;
        }

        public override String ToString()
        {
            return $"x: {Left:0.0}, y: {Top:0.0} w: {Width:0.0} h: {Height:0.0}";
        }
    }
}