using System;
using System.Threading.Tasks;

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

      public static readonly ValueVector3 Origin = new ValueVector3(0, 0, 0);

      Boolean IEquatable<IPoint3D>.Equals(IPoint3D other)
      {
         throw new NotImplementedException();
      }

      public override String ToString()
      {
         return $"{X},{Y},{Z}";
      }

      Single IPoint3D.X => (Single)X;

      Single IPoint3D.Y => (Single)Y;

      Single IPoint3D.Z => (Single)Z;

      IPoint3D IPoint3D.Rotate(Single x,
                               Single y,
                               Single z)
      {
         throw new NotImplementedException();
      }
   }
}
