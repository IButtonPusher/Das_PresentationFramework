using System;

namespace Das.Views.Core.Geometry
{
    public readonly struct ValueRectangle : IRectangle
    {
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

        public Boolean Equals(ISize other)
        {
            return GeometryHelper.AreSizesEqual(this, other);
        }

        void IRectangle.Union(IRectangle rect)
        {
            throw new NotSupportedException();
        }

        public override String ToString()
        {
            return $"x: {Left:0.0}, y: {Top:0.0} w: {Width:0.0} h: {Height:0.0}";
        }
    }
}
