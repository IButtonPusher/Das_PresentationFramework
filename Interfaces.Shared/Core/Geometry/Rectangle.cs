using System;

// ReSharper disable UnusedMember.Global

namespace Das.Views.Core.Geometry
{
    public class Rectangle : Size, IDeepCopyable<Rectangle>, IRectangle, IRoundedRectangle,
        IEquatable<Rectangle>, IEquatable<IRectangle>, IEquatable<IRoundedRectangle>
    {
        public Rectangle() : this(0, 0, 0, 0)
        {
        }

        private Double _x;
        private Double _y;
        private Double _w;
        private Double _h;

        public Rectangle(IPoint location, Double width, Double height)
            : this(location.X, location.Y, width, height)
        {
        }

        public Rectangle(Double x, Double y, ISize size)
            : this(x, y, size.Width, size.Height)
        {
        }

        public Rectangle(IPoint location, ISize size)
            : this(location.X, location.Y, size.Width, size.Height)
        {
        }

        public Rectangle(IRectangle start) : this(start, Thickness.Empty)
        {
        }

        public Rectangle(IRectangle start, Thickness margin)
            : this(start.X + margin.Left, start.Y + margin.Top,
                start.Width - (margin.Left + margin.Right),
                start.Height - (margin.Top + margin.Bottom))
        {
        }

        public Rectangle(Double x, Double y, Double width, Double height)
        {
            _x = x;
            _y = y;
            _w = width;
            _h = height;
        }

        public Double Top
        {
            get => _y;
            set => _y = value;
        }

        public Double Left
        {
            get => _x;
            set => _x = value;
        }

        public Point BottomLeft => new Point(Left, Top + Height);
        public Point BottomRight => new Point(Left + Width, Top + Height);

        public Double Bottom
        {
            get => _y + _h;
            set
            {
                if (value < Top)
                    throw new InvalidOperationException();
                _h = value - _y;
            }
        }

        public Double Right
        {
            get => _x + Size.Width;
            set
            {
                if (value < _x)
                    throw new InvalidOperationException();
                _w = value - _x;
            }
        }

        public ISize Size
        {
            get => new Size(_w, _h);
            set
            {
                _w = value.Width;
                _h = value.Height;
            }
        }

        public Point Location
        {
            get => TopLeft;
            set => TopLeft = value;
        }

        public Point TopLeft
        {
            get => new Point(_x, _y);
            set
            {
                _x = value.X;
                _y = value.Y;
            }
        }

        public Point TopRight => new Point(_x + _w, _y);

        public Double X
        {
            get => _x;
            set => _x = value;
        }

        Int32 IRoundedRectangle.Y => Convert.ToInt32(Y);

        Int32 IRoundedRectangle.Width => Convert.ToInt32(Width);

        Int32 IRoundedRectangle.Height => Convert.ToInt32(Height);

        Int32 IRoundedRectangle.X => Convert.ToInt32(X);

        public Double Y
        {
            get => _y;
            set => _y = value;
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

        public Boolean Contains(IPoint point)
        {
            if (point == null)
                return false;

            return point.X >= Left
                   && point.X <= Right
                   && point.Y >= Top
                   && point.Y <= Bottom;
        }

        public Boolean Contains(Int32 x, Int32 y) => x >= Left
                                              && x <= Right
                                              && y >= Top
                                              && y <= Bottom;

        public Boolean Contains(Double x, Double y) => x >= Left
                                                    && x <= Right
                                                    && y >= Top
                                                    && y <= Bottom;

        public static Rectangle operator +(Rectangle rect, Thickness margin)
        {
            if (margin == null)
                return rect.DeepCopy();

            return new Rectangle(rect.X, rect.Y,
                rect.Width - (margin.Left + margin.Right),
                rect.Height - (margin.Top + margin.Bottom));
        }

        public static Rectangle operator +(Rectangle rect, Point location)
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

        public static Rectangle operator *(Rectangle rect, Double val)
        {
            if (val.AreEqualEnough(1))
                return rect;

            if (rect == null)
                return null;

            return new Rectangle(rect.Location, rect * val);
        }


        public new static Rectangle Empty { get; } = new Rectangle(0, 0, 0, 0);

        IPoint IDeepCopyable<IPoint>.DeepCopy() => new Point(X, Y);

        public Boolean Equals(Rectangle other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _x.Equals(other._x)
                   && _y.Equals(other._y) &&
                   _w.Equals(other._w) &&
                   _h.Equals(other._h);
        }

        public Boolean Equals(IRectangle other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _x.Equals(other.X)
                   && _y.Equals(other.Y) &&
                   _w.Equals(other.Width) &&
                   _h.Equals(other.Height);
        }


        public Boolean Equals(IRoundedRectangle other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            IRoundedRectangle me = this;

            return me.X.Equals(other.X)
                   && me.Y.Equals(other.Y) &&
                   me.Width.Equals(other.Width) &&
                   me.Height.Equals(other.Height);
        }

        public override String ToString() => $"x: {_x:0.0}, y: {_y:0.0} w: {_w:0.0} h: {_h:0.0}";

        public new Rectangle DeepCopy() => new Rectangle(X, Y, Width, Height);

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

        private Int32 _hash;

        public override Int32 GetHashCode()
        {
            if (_hash != 0)
                return _hash;

            IRoundedRectangle me = this;
            _hash = (Byte) me.X + ((Byte) me.Y << 4) +
                    ((Byte) me.Height << 16) + ((Byte) me.Width << 24);

            return _hash;
        }
    }
}