using System;
using System.Threading.Tasks;
using Das.Views.Extended.Core;

namespace Das.Views.Extended
{
   public readonly struct ValueVector3 : IPoint3D
   {
      public readonly Double X;
      public readonly Double Y;

      public readonly Double Z;

      public ValueVector3(Double x,
                          Double y,
                          Double z)
      {
         X = x;
         Y = y;
         Z = z;
      }

      public static readonly ValueVector3 Origin = new(0, 0, 0);

      Boolean IEquatable<IPoint3D>.Equals(IPoint3D other) => throw new NotImplementedException();

      public override String ToString() => $"{X},{Y},{Z}";

      Single IPoint3D.X => (Single)X;

      Single IPoint3D.Y => (Single)Y;

      Single IPoint3D.Z => (Single)Z;

      public static ValueVector3 operator +(ValueVector3 a,
                                       ValueVector3 b) =>
         new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

      public static ValueVector3 operator +(ValueVector3 a,
                                            Single b) =>
         new(a.X + b, a.Y + b, a.Z + b);

      public static ValueVector3 operator /(ValueVector3 a,
                                            Double d) =>
         new(a.X / d, a.Y / d, a.Z / d);

      public static ValueVector3 operator *(ValueVector3 a,
                                            Double d) =>
         new(a.X * d, a.Y * d, a.Z * d);

      public static ValueVector3 operator -(ValueVector3 a,
                                            ValueVector3 b) =>
         new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

      public static ValueVector3 operator -(ValueVector3 a,
                                            Single b) =>
         new(a.X - b, a.Y - b, a.Z - b);

      IPoint3D IPoint3D.Rotate(Single x,
                               Single y,
                               Single z) =>
         Rotate(x, y, z);

      public ValueVector3 Rotate(Single x,
                                 Single y,
                                 Single z) =>
         new((X + x) % 360,
            (Y + y) % 360,
            (Z + z) % 360);
   }
}
