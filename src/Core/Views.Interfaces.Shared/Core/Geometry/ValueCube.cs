using System;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Rendering.Geometry;

namespace Das.Views.Core.Geometry;

public readonly struct ValueCube : ICube
{
   public ValueCube(Double x,
                    Double y,
                    ISize size,
                    Double depth)
      : this(x, y, size.Width, size.Height, depth)
   {
   }

   public ValueCube(IPoint2D position,
                    ISize size,
                    Double depth)
      : this(position.X, position.Y, size.Width, size.Height, depth)
   {
   }

   public ValueCube(IPoint2D position,
                    Double width,
                    Double height,
                    Double depth)
      : this(position.X, position.Y, width, height, depth)
   {
   }

   public ValueCube(Double x,
                    Double y,
                    Double width,
                    Double height,
                    Double depth)
   {
      X = x;
      Y = y;
      Width = width;
      Height = height;
      Depth = depth;

      HasInfiniteDimension = Double.IsInfinity(Width) || Double.IsInfinity(Height);
   }

   public ValueCube(IRectangle start,
                    Double depth)
      : this(start.X, start.Y, start.Width, start.Height, depth)
   {
      Depth = depth;
   }

   public ValueCube(ValueRenderRectangle start,
                    Double depth)
   {
      X = start.X;
      Y = start.Y;
      Width = start.Width;
      Height = start.Height;

      Depth = depth;

      HasInfiniteDimension = Double.IsInfinity(Width) || Double.IsInfinity(Height);
   }

   IPoint2D IRectangle.TopLeft => TopLeft;

   IPoint2D IRectangle.TopRight => TopRight;

   IPoint2D IRectangle.BottomLeft => BottomLeft;

   IPoint2D IRectangle.BottomRight => BottomRight;

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

   public Boolean HasInfiniteDimension { get; }

   public IPoint2D Location => TopLeft;

   public ISize Size => new ValueSize(Width, Height);

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

   public Boolean IntersectsWith(IRectangle rect)
   {
      return GeometryHelper.IsRectanglesIntersect(this, rect);
   }

   void IRectangle.Union(IRectangle rect)
   {
      throw new NotSupportedException();
   }

   public Boolean Equals(IRectangle other)
   {
      return GeometryHelper.AreRectsEqual(this, other);
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

   public static ValueCube Empty = new ValueCube(0, 0, 0, 0, 0);

   public Double Depth { get; }
}