using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Extensions;


// ReSharper disable UnusedMember.Global

namespace Das.Views.Core.Geometry
{
    public class Rectangle : Size, IDeepCopyable<Rectangle>,
                             IRectangle,
                             IRoundedRectangle,
                             IEquatable<Rectangle>,
                             IEquatable<IRectangle>
                             //IEquatable<IRoundedRectangle>
    {
        public Rectangle() : this(0, 0, 0, 0)
        {
        }

        public Rectangle(IPoint2D location,
                         Double width,
                         Double height)
            : this(location.X, location.Y, width, height)
        {
        }

        public Rectangle(Double x,
                         Double y,
                         ISize size)
            : this(x, y, size.Width, size.Height)
        {
        }

        public Rectangle(IPoint2D location,
                         ISize size)
            : this(location.X, location.Y, size.Width, size.Height)
        {
        }

        public Rectangle(IRectangle start) : this(start, Thickness.Empty)
        {
        }

        public Rectangle(IRectangle start,
                         Thickness margin)
            : this(start.X + margin.Left, start.Y + margin.Top,
                start.Width - margin.Width,
                start.Height - margin.Height)
        {
        }

        public Rectangle(Double x,
                         Double y,
                         Double width,
                         Double height)
        {
            Left = x;
            Top = y;
            _w = width;
            _h = height;
        }

        public new Rectangle DeepCopy()
        {
            return new Rectangle(X, Y, Width, Height);
        }

        public Boolean Equals(IRectangle? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Left.Equals(other.X)
                   && Top.Equals(other.Y) &&
                   _w.Equals(other.Width) &&
                   _h.Equals(other.Height);
        }


        public Boolean Equals(IRoundedRectangle? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            IRoundedRectangle me = this;

            return me.X.Equals(other.X)
                   && me.Y.Equals(other.Y) &&
                   me.Width.Equals(other.Width) &&
                   me.Height.Equals(other.Height);
        }

        public Boolean Equals(Rectangle? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Left.Equals(other.Left)
                   && Top.Equals(other.Top) &&
                   _w.Equals(other._w) &&
                   _h.Equals(other._h);
        }

        public Double Top { get; set; }

        public Double Left { get; set; }

        public Point2D BottomLeft => new Point2D(Left, Top + Height);

        public Point2D BottomRight => new Point2D(Left + Width, Top + Height);

        public Double Bottom
        {
            get => Top + _h;
            set
            {
                if (value < Top)
                    throw new InvalidOperationException();
                _h = value - Top;
            }
        }

        public Double Right
        {
            get => Left + Size.Width;
            set
            {
                if (value < Left)
                    throw new InvalidOperationException();
                _w = value - Left;
            }
        }

        ISize IPointContainer.Size => Size;

        public Point2D Location
        {
            get => TopLeft;
            set => TopLeft = value;
        }

        public Point2D TopLeft
        {
            get => new Point2D(Left, Top);
            set
            {
                Left = value.X;
                Top = value.Y;
            }
        }

        public Point2D TopRight => new Point2D(Left + _w, Top);

        void IRectangle.Union(IRectangle rect)
        {
            if (IsEmpty)
            {
                Left = rect.X;
                Top = rect.Y;
                _w = rect.Width;
                _h = rect.Height;

                return;
            }

            if (rect.IsEmpty)
                return;


            var left = Math.Min(Left, rect.Left);
            var top = Math.Min(Top, rect.Top);


            // We need this check so that the math does not result in NaN
            if (Double.IsPositiveInfinity(rect.Width) || Double.IsPositiveInfinity(Width))
                _w = Double.PositiveInfinity;
            else
            {
                //  Max with 0 to prevent double weirdness from causing us to be (-epsilon..0)                    
                var maxRight = Math.Max(Right, rect.Right);
                _w = Math.Max(maxRight - left, 0);
            }

            // We need this check so that the math does not result in NaN
            if (Double.IsPositiveInfinity(rect.Height) || Double.IsPositiveInfinity(Height))
                _h = Double.PositiveInfinity;
            else
            {
                //  Max with 0 to prevent double weirdness from causing us to be (-epsilon..0)
                var maxBottom = Math.Max(Bottom, rect.Bottom);
                _h = Math.Max(maxBottom - top, 0);
            }

            Left = left;
            Top = top;
        }

        public Double X
        {
            get => Left;
            set => Left = value;
        }

        public Double Y
        {
            get => Top;
            set => Top = value;
        }

        public override Double Width
        {
            get => _w;
            set => _w = value;
        }

        public override Double Height
        {
            get => _h;
            set => _h = value;
        }

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

        public ValueRectangle GetUnion(IRoundedRectangle b)
        {
            return GeometryHelper.GetUnion(this, b);
            //var a = this as IRoundedRectangle;

            //var x1 = Math.Min(a.X, b.X);
            //var x2 = Math.Max(a.X + a.Width, b.X + b.Width);
            //var y1 = Math.Min(a.Y, b.Y);
            //var y2 = Math.Max(a.Y + a.Height, b.Y + b.Height);
            //return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        public ValueRectangle GetUnion(IEnumerable<IRoundedRectangle> others)
        {
            return GeometryHelper.GetUnion(this, others);

            //var me = this as IRoundedRectangle;

            //var x1 = me.X;
            //var x2 = me.X + me.Width;
            //var y1 = me.Y;
            //var y2 = me.Y + me.Height;

            //foreach (var b in others)
            //{
            //    x1 = Math.Min(x1, b.X);
            //    x2 = Math.Max(x2, b.X + b.Width);
            //    y1 = Math.Min(y1, b.Y);
            //    y2 = Math.Max(y2, b.Y + b.Height);
            //}

            //return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }


        Int32 IRoundedRectangle.Y => Convert.ToInt32(Y);

        Int32 IRoundedRectangle.Width => Convert.ToInt32(Width);

        Int32 IRoundedRectangle.Height => Convert.ToInt32(Height);

        Int32 IRoundedRectangle.X => Convert.ToInt32(X);


        public new static Rectangle Empty { get; } = new Rectangle(0, 0, 0, 0);

        public Size Size
        {
            get => new Size(_w, _h);
            set
            {
                _w = value.Width;
                _h = value.Height;
            }
        }

        public override Boolean Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            switch (obj)
            {
                case IRectangle rect:
                    return Equals(rect);
                case IRoundedRectangle rounded:
                    return Equals(rounded);
                default:
                    return false;
            }
        }

        public override Int32 GetHashCode()
        {
            if (_hash != 0)
                return _hash;

            IRoundedRectangle me = this;
            _hash = (Byte) me.X + ((Byte) me.Y << 4) +
                    ((Byte) me.Height << 16) + ((Byte) me.Width << 24);

            return _hash;
        }

        public static Rectangle operator +(Rectangle rect, Thickness margin)
        {
            if (margin == null)
                return rect.DeepCopy();

            return new Rectangle(rect.X, rect.Y,
                rect.Width - (margin.Left + margin.Right),
                rect.Height - (margin.Top + margin.Bottom));
        }

        public static Rectangle operator +(Rectangle rect, Point2D location)
        {
            if (location == null)
                return rect.DeepCopy();

            return new Rectangle(rect.X + location.X, rect.Y + location.Y,
                rect.Width, rect.Height);
        }

        public static Rectangle operator +(Rectangle rect, Size size)
        {
            if (size == null)
                return rect.DeepCopy();

            return new Rectangle(rect.X + size.Width, rect.Y + size.Height,
                rect.Width, rect.Height);
        }

        public static Rectangle? operator *(Rectangle? rect, Double val)
        {
            if (val.AreEqualEnough(1))
                return rect;

            if (rect == null)
                return null;

            return new Rectangle(rect.Location,
                rect.Size * val);
        }

        public override String ToString()
        {
            return $"x: {Left:0.0}, y: {Top:0.0} w: {_w:0.0} h: {_h:0.0}";
        }

        private Double _h;

        private Int32 _hash;
        private Double _w;
    }
}