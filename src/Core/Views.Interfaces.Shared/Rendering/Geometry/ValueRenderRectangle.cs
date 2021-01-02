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
        : this(rect.Location, rect.Size, rect.Size.Offset, padding)
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

            TopLeft = new ValuePoint2D(X, Y);
            TopRight = new ValuePoint2D(X + Width, Y);
            BottomLeft = new ValuePoint2D(X, Y + Height);
            BottomRight = new ValuePoint2D(X + Width, Y + Height);


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

            TopLeft = new ValuePoint2D(X, Y);
            TopRight = new ValuePoint2D(X + Width, Y);
            BottomLeft = new ValuePoint2D(X, Y + Height);
            BottomRight = new ValuePoint2D(X + Width, Y + Height);

            _hash = 0;
            _hash = GeometryHelper.BuildRectHash(this);
        }

        public static readonly ValueRenderRectangle Empty = new ValueRenderRectangle(0, 0, 0, 0,
            ValuePoint2D.Empty);



        IPoint2D IRectangle.TopLeft => TopLeft;

        IPoint2D IRectangle.TopRight => TopRight;

        IPoint2D IRectangle.BottomLeft => BottomLeft;

        IPoint2D IRectangle.BottomRight => BottomRight;

        public readonly ValuePoint2D TopLeft;

        public readonly ValuePoint2D TopRight;

        public readonly ValuePoint2D BottomLeft;

        public readonly ValuePoint2D BottomRight;

        //public Point2D BottomLeft => new Point2D(Left, Top + Height);

        //public Point2D BottomRight => new Point2D(Left + Width, Top + Height);

        //public Point2D TopLeft => new Point2D(Left, Top);

        //public Point2D TopRight => new Point2D(Left + Width, Top);

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
            return GeometryHelper.MinusVertical(Size, subtract);
        }

        public ValueSize ToValueSize()
        {
            return GeometryHelper.ToValueSize(Size);
        }

        //ISize ISize.Divide(Double pct)
        //{
        //    return new ValueSize(Width * pct, Height * pct);
        //}

        //ISize ISize.PlusVertical(ISize adding)
        //{
        //    return PlusVertical(adding);
        //}

        //ISize ISize.Reduce(Thickness padding)
        //{
        //    return GeometryHelper.Reduce(this, padding);
        //}

        public ValueRenderRectangle ToFullRectangle()
        {
            return this;
        }

        public IRenderSize PlusVertical(ISize adding)
        {
            return GeometryHelper.PlusRenderVertical(Size, adding);
        }

        //IRenderSize IRenderSize.DeepCopy()
        //{
        //    return new RenderSize(Width, Height, Offset);
        //}

        //public IRenderSize Reduce(Thickness padding)
        //{
        //    return GeometryHelper.Reduce(Size, padding);
        //}

        //ISize ISize.Minus(ISize subtract)
        //{
        //    return GeometryHelper.Minus(this, subtract);
        //}

        public IRenderSize Minus(ISize subtract)
        {
            return GeometryHelper.Minus(Size, subtract);
        }

        public ValueRenderRectangle Move(IPoint2D point)
        {
            return new ValueRenderRectangle(X + point.X, Y + point.Y, Size, Offset);
        }

        IPoint2D IPointContainer.Location => Location;

        public ValuePoint2D Location => TopLeft;

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

        //ISize IDeepCopyable<ISize>.DeepCopy()
        //{
        //    return new ValueSize(Width, Height);
        //}

        //Boolean IEquatable<IRenderSize>.Equals(IRenderSize other)
        //{
        //    return false;
        //}

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
            return obj is IRenderRectangle r && GeometryHelper.AreRenderRectsEquals(this, r);
        }

        public Boolean Equals(IRenderRectangle other)
        {
            return GeometryHelper.AreRenderRectsEquals(this, other);
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

        public static ValueRenderRectangle operator *(ValueRenderRectangle rect, 
                                                      Double val)
        {
            if (val.AreEqualEnough(1))
                return rect;

            if (rect.IsEmpty)
                return rect;

            //if (rect == null)
            //    return null!;

            return new ValueRenderRectangle(rect.X * val,
                rect.Y * val,
                rect.Size.Width * val,
                rect.Size.Height * val,
                new ValuePoint2D(rect.Offset.X * val,
                    rect.Offset.Y * val));
        }

        private readonly Int32 _hash;
    }
}