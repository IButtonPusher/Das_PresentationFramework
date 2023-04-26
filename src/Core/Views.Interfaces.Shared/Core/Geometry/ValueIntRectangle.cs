using System;
using System.Collections.Generic;

namespace Das.Views.Core.Geometry;

public readonly struct ValueIntRectangle : IRoundedRectangle
{
   public ValueIntRectangle(Int32 x, 
                            Int32 y,
                            Int32 width,
                            Int32 height)
   {
      Location = new Point2D(x,y);
      Size = new ValueSize(width, height);
      Height = height;
      Width = width;
      X = x;
      Y = y;
   }

   public ValueIntRectangle(IPoint2D pos,
                            ISize size)
      : this(Convert.ToInt32(pos.X),
         Convert.ToInt32(pos.Y),
         Convert.ToInt32(size.Width),
         Convert.ToInt32(size.Height))
   {
            
   }

   public IPoint2D Location { get; }

   public ISize Size { get; }

   public Boolean Contains(IPoint2D point)
   {
      return point.X >= X
             && point.X <= X + Width
             && point.Y >= Y
             && point.Y <= Y + Height;
   }

   public Boolean Contains(Int32 x, 
                           Int32 y)
   {
      return x >= X
             && x <= X + Width
             && y >= Y
             && y <= Y + Height;
   }

   public Boolean Contains(Double x, 
                           Double y)
   {
      return x >= X
             && x <= X + Width
             && y >= Y
             && y <= Y + Height;
   }

   public override String ToString()
   {
      return $"x: {X:0.0}, y: {Y:0.0} w: {Width:0.0} h: {Height:0.0}";
   }

   public Int32 Height { get; }

   public Int32 Width { get; }

   public Int32 X { get; }

   public Int32 Y { get; }

   public ValueIntRectangle GetUnion(IRoundedRectangle other)
   {
      return GeometryHelper.GetUnion(this, other);
   }

   public ValueIntRectangle GetUnion(IEnumerable<IRoundedRectangle> others)
   {
      return GeometryHelper.GetUnion(this, others);
   }

   public Boolean Equals(IRoundedRectangle? other)
   {
      return other is { } ok && ok.X == X &&
             ok.Y == Y && ok.Width == Width && ok.Height == Height;
   }
}