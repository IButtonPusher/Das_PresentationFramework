using System;
using System.Threading.Tasks;
using Das.Extensions;

namespace Das.Views.Core.Geometry;

public class Thickness : Size,
                         IDeepCopyable<Thickness>,
                         IThickness
{
   public Thickness(Double uniformLength)
   {
      Left = Right = Top = Bottom = uniformLength;
   }

   public Thickness(Double leftAndRight,
                    Double topAndBottom)
      : this(leftAndRight, topAndBottom, leftAndRight, topAndBottom)
        
   {}
        
   public Thickness(Double left,
                    Double top,
                    Double right,
                    Double bottom)
   {
      Left = left;
      Top = top;
      Right = right;
      Bottom = bottom;
   }

   public new Thickness DeepCopy()
   {
      return new Thickness(Left, Top, Right, Bottom);
   }

   public Double Left { get; }

   public Double Top { get; }

   public Boolean AreAllSidesEqual()
   {
      return Left.AreEqualEnough(Top) && Left.AreEqualEnough(Right) &&
             Left.AreEqualEnough(Bottom);
   }

   public Double Right { get; }

   public Double Bottom { get; }

   public override Double Width => Left + Right;

   public override Double Height => Top + Bottom;

   //public override Boolean IsEmpty => Height.IsZero() && Width.IsZero();

   public new static Thickness Empty { get; } = new Thickness(0);

   public static Thickness operator +(Thickness size, Thickness margin)
   {
      if (margin == null)
         return size.DeepCopy();

      return new Thickness(size.Left + margin.Left, margin.Top + size.Top,
         margin.Right + size.Right, margin.Bottom + size.Bottom);
   }

   public static explicit operator Thickness(Double uniform)
   {
      return new Thickness(uniform);
   }

   public static explicit operator Thickness(Int32 uniform)
   {
      return new Thickness(uniform);
   }

   public static Thickness operator *(Thickness size, Double val)
   {
      if (val.AreEqualEnough(1))
         return size;

      if (size == null)
         return null!;

      return new Thickness(size.Left * val, size.Top * val, size.Right * val,
         size.Bottom * val);
   }

   public override String ToString()
   {
      return "L: " + Left.ToString("0.0") +
             " T: " + Top.ToString("0.0") +
             " R: " + Right.ToString("0.0") +
             " B: " + Bottom.ToString("0.0");
   }
}