using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Das.Extensions;


// ReSharper disable UnusedMember.Global

namespace Das.Views.Core.Geometry
{
   public class Rectangle : Size, IDeepCopyable<Rectangle>,
                            IRectangle,
                            IRoundedRectangle,
                            IEquatable<Rectangle>
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
         return new(X, Y, Width, Height);
      }

      public virtual void Reset()
      {
         Left = Top = _w = _h = 0;
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

      public Boolean Equals(IRectangle? other)
      {
         if (ReferenceEquals(null, other)) return false;
         if (ReferenceEquals(this, other)) return true;
         return Left.Equals(other.X)
                && Top.Equals(other.Y) &&
                _w.Equals(other.Width) &&
                _h.Equals(other.Height);
      }

      public Double Top
      {
         get => _top;
         set => _top = value;
      }

      public Double Left
      {
         get => _left;
         set => _left = value;
      }

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

      IPoint2D IPointContainer.Location => Location;

      IPoint2D IRectangle.TopLeft => TopLeft;

      IPoint2D IRectangle.TopRight => TopRight;

      IPoint2D IRectangle.BottomLeft => BottomLeft;

      IPoint2D IRectangle.BottomRight => BottomRight;

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

      public Boolean IntersectsWith(IRectangle rect)
      {
         return GeometryHelper.IsRectanglesIntersect(this, rect);
      }


      public Boolean Contains(IPoint2D point2D)
      {
         return GeometryHelper.IsRectangleContains(this, point2D);
      }

      public Boolean Contains(Int32 x,
                              Int32 y)
      {
         return GeometryHelper.IsRectangleContains(this, x, y);
      }

      public Boolean Contains(Double x,
                              Double y)
      {
         return GeometryHelper.IsRectangleContains(this, x, y);
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

      public ValueIntRectangle GetUnion(IRoundedRectangle b)
      {
         return GeometryHelper.GetUnion(this, b);
      }

      public ValueIntRectangle GetUnion(IEnumerable<IRoundedRectangle> others)
      {
         return GeometryHelper.GetUnion(this, others);
      }


      Int32 IRoundedRectangle.Y => Convert.ToInt32(Y);

      Int32 IRoundedRectangle.Width => Convert.ToInt32(Width);

      Int32 IRoundedRectangle.Height => Convert.ToInt32(Height);

      Int32 IRoundedRectangle.X => Convert.ToInt32(X);

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

      [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
      public override Int32 GetHashCode()
      {
         if (_hash != 0)
            return _hash;

         _hash = GeometryHelper.BuildRectHash(this);

         return _hash;
      }

      public static Rectangle operator +(Rectangle rect,
                                         Thickness margin)
      {
         if (margin == null)
            return rect.DeepCopy();

         return new Rectangle(rect.X, rect.Y,
            rect.Width - (margin.Left + margin.Right),
            rect.Height - (margin.Top + margin.Bottom));
      }

      public static Rectangle operator +(Rectangle rect,
                                         Point2D location)
      {
         if (location == null)
            return rect.DeepCopy();

         return new Rectangle(rect.X + location.X, rect.Y + location.Y,
            rect.Width, rect.Height);
      }

      public static Rectangle operator +(Rectangle rect,
                                         Size size)
      {
         if (size == null)
            return rect.DeepCopy();

         return new Rectangle(rect.X + size.Width, rect.Y + size.Height,
            rect.Width, rect.Height);
      }

      public static Rectangle? operator *(Rectangle? rect,
                                          Double val)
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

      public Point2D BottomLeft => new(Left, Top + Height);

      public Point2D BottomRight => new(Left + Width, Top + Height);

      public Point2D Location
      {
         get => TopLeft;
         set => TopLeft = value;
      }


      public Point2D TopLeft
      {
         get => new(Left, Top);
         set
         {
            Left = value.X;
            Top = value.Y;
         }
      }

      public Point2D TopRight => new Point2D(Left + _w, Top);


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

      protected Double _h;

      private Int32 _hash;
      protected Double _left;
      protected Double _top;
      protected Double _w;
   }
}
