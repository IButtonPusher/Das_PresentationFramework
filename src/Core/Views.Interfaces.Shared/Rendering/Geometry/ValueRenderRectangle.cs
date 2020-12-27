using System;
using System.Threading.Tasks;
using Das.Extensions;
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

        public ValueRenderRectangle(IRenderSize size,
                                    Thickness? padding)
            : this(Point2D.Empty, size, size.Offset, padding)
        {
        }

        public ValueRenderRectangle(IRenderRectangle rect,
                                    Thickness? padding)
        : this(rect.Location, rect.Size, rect.Offset, padding)
        {
        }

        public ValueRenderRectangle(IPoint2D position,
                                    IRenderSize renderSize,
                                    Thickness? padding)
        : this(position, renderSize, renderSize.Offset, padding)
        {
           
        }

        public ValueRenderRectangle(ISize size,
                                    IPoint2D offset,
                                    Thickness? padding)
            : this(Point2D.Empty, size, offset, padding)
        {
           
        }


        public ValueRenderRectangle(IPoint2D position,
                                    ISize renderSize,
                                    IPoint2D offset,
                                    Thickness? padding)
        {
            if (padding?.IsEmpty == false)
            {
                X = position.X + padding.Left;
                Y = position.Y + padding.Top;
                Width = renderSize.Width - padding.Width;
                Height = renderSize.Height - padding.Height;
            }
            else
            {
                X = position.X;
                Y = position.Y;
                Width = renderSize.Width;
                Height = renderSize.Height;
            }

            Offset = offset;

            _hash = 0;
            _hash = GeometryHelper.BuildRectHash(this);
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

            _hash = 0;
            _hash = GeometryHelper.BuildRectHash(this);
        }

        public static readonly ValueRenderRectangle Empty = new ValueRenderRectangle(0, 0, 0, 0,
            ValuePoint2D.Empty);

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

        public Boolean IsEmpty => Width.IsZero() && Height.IsZero();

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

        ISize ISize.Divide(Double pct)
        {
            return new ValueSize(Width * pct, Height * pct);
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

        public ValueRenderRectangle Move(IPoint2D point)
        {
            return new ValueRenderRectangle(X + point.X, Y + point.Y, Size, Offset);
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

        public Double CenterY(ISize item)
        {
            return GeometryHelper.CenterY(this, item);
        }

        public Double CenterX(ISize item)
        {
            return GeometryHelper.CenterX(this, item);
        }

        public override String ToString()
        {
            return $"x: {Left:0.0}, y: {Top:0.0} w: {Width:0.0} h: {Height:0.0}";
        }

        public Boolean Equals(IRectangle other)
        {
            return GeometryHelper.AreRectsEqual(this, other);
        }

        public override Boolean Equals(Object obj)
        {
            return obj is IRectangle r && GeometryHelper.AreRectsEqual(this, r);
        }

        public override Int32 GetHashCode()
        {
            return _hash;
        }

        TRectangle IRenderRectangle.Reduce<TRectangle>(Double left, 
                                                       Double top, 
                                                       Double right, 
                                                       Double bottom)
        {
            var res = new ValueRenderRectangle(X + left, Top + top,
                Width - (left + right),
                Height - (top + bottom), Offset);

            if (res is TRectangle fku)
                return fku;
            throw new InvalidOperationException();
        }

        private readonly Int32 _hash;
    }
}