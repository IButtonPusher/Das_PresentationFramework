using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Geometry
{
    public readonly struct ValueRectangle : IRectangle
    {
        public ValueRectangle(Double x,
                              Double y,
                              ISize size)
            : this(x, y, size.Width, size.Height)
        {
        }

        public ValueRectangle(IPoint2D position,
                              ISize size)
            : this(position.X, position.Y, size.Width, size.Height)
        {
        }

        public ValueRectangle(IPoint2D position,
                              Double width,
                              Double height)
            : this(position.X, position.Y, width, height)
        {
        }

        ISize ISize.Divide(Double pct)
        {
            return GeometryHelper.Divide(this, pct);
        }

        public ValueRectangle(Double x,
                              Double y,
                              Double width,
                              Double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
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

        public ISize Reduce(Thickness padding)
        {
            return GeometryHelper.Reduce(this, padding);
        }

        ISize ISize.Minus(ISize subtract)
        {
            return GeometryHelper.Minus(this, subtract);
        }

        public Point2D Location => TopLeft;

        public ISize Size => new ValueSize(Width, Height);

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

        public ISize PlusVertical(ISize adding)
        {
            return GeometryHelper.PlusVertical(this, adding);
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

        public override String ToString()
        {
            return $"x: {Left:0.0}, y: {Top:0.0} w: {Width:0.0} h: {Height:0.0}";
        }

        public Double CenterY(ISize item)
        {
            return GeometryHelper.CenterY(this, item);
        }

        public Double CenterX(ISize item)
        {
            return GeometryHelper.CenterX(this, item);
        }

        public static ValueRectangle Empty = new ValueRectangle(0, 0, 0, 0);
    }
}