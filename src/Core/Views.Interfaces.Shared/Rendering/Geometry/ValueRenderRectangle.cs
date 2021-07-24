using System;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering.Geometry
{
   public readonly struct ValueRenderRectangle : IRenderRectangle,
                                                 IEquatable<ValueRenderRectangle>
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
         //TopRight = new ValuePoint2D(X + Width, Y);
         //BottomLeft = new ValuePoint2D(X, Y + Height);
         //BottomRight = new ValuePoint2D(X + Width, Y + Height);

         IsEmpty = Width.IsZero() && Height.IsZero();

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

      public Boolean IntersectsWith(IRectangle rect)
      {
         return GeometryHelper.IsRectanglesIntersect(this, rect);
      }

      public ValueRenderRectangle(ValueRenderRectangle basis,
                                  ValuePoint2D moveBy)
      {
         X = basis.X + moveBy.X;
         Y = basis.Y + moveBy.Y;

         Width = basis.Width;
         Height = basis.Height;
         Offset = basis.Offset;

         TopLeft = new ValuePoint2D(X, Y);
         IsEmpty = Width.IsZero() && Height.IsZero();

         _hash = 0;
         _hash = GeometryHelper.BuildRectHash(this);
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

         TopLeft = new ValuePoint2D(x, y);
         //TopRight = new ValuePoint2D(X + Width, Y);
         //BottomLeft = new ValuePoint2D(X, Y + Height);
         //BottomRight = new ValuePoint2D(X + Width, Y + Height);

         IsEmpty = width.IsZero() && height.IsZero();

         _hash = 0;
         _hash = GeometryHelper.BuildRectHash(this);
      }

      public static readonly ValueRenderRectangle Empty = new(0, 0, 0, 0,
         ValuePoint2D.Empty);


      IPoint2D IRectangle.TopLeft => TopLeft;

      IPoint2D IRectangle.TopRight => new ValuePoint2D(X + Width, Y);

      IPoint2D IRectangle.BottomLeft => new ValuePoint2D(X, Y + Height);

      IPoint2D IRectangle.BottomRight => new ValuePoint2D(X + Width, Y + Height);

      public readonly ValuePoint2D TopLeft;

      //public readonly ValuePoint2D TopRight;

      //public readonly ValuePoint2D BottomLeft;

      //public readonly ValuePoint2D BottomRight;


      public Double X { get; }

      public Double Y { get; }

      public Double Bottom => Y + Height;

      public Double Left => X;

      public Double Right => X + Width;

      public Double Top => Y;

      public Double Height { get; }

      public Boolean IsEmpty { get; }

      public Double Width { get; }

      public IPoint2D Offset { get; }


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

      public Boolean Equals(IRectangle other)
      {
         return GeometryHelper.AreRectsEqual(this, other);
      }

      public Boolean Equals(ValueRenderRectangle other)
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
