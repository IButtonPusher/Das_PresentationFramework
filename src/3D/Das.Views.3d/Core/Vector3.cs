using System;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Extended.Core;

namespace Das.Views.Extended;

/// <summary>
///     Three-dimensional point (x,y,z)
/// </summary>
public class Vector3 : Vector2, IPoint3D, IEquatable<Vector3>
{
   public Vector3(Single x,
                  Single y,
                  Single z)
   {
      X = x;
      Y = y;
      Z = z;

      //            _hash = x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
   }

   public Vector3(Double x,
                  Double y,
                  Double z) : this((Single) x, (Single) y, (Single) z)
   {
   }

   public Vector3()
   {
   }

   public Boolean Equals(Vector3? other) => other is IPoint3D p && Equals(p);

   /// <summary>The Z component of the vector.</summary>
   public Single Z { get; set; }

   public Boolean Equals(IPoint3D? other) =>
      !ReferenceEquals(null, other) &&
      X.AreEqualEnough(other.X) &&
      Y.AreEqualEnough(other.Y) &&
      Z.AreEqualEnough(other.Z);

   IPoint3D IPoint3D.Rotate(Single x,
                            Single y,
                            Single z) =>
      Rotate(x, y, z);

   public static Vector3 Zero => new();

   public static Vector3 Cross(Vector3 left,
                               Vector3 right) =>
      new((Single) (left.Y * (Double) right.Z - left.Z * (Double) right.Y),
         (Single) (left.Z * (Double) right.X - left.X * (Double) right.Z),
         (Single) (left.X * (Double) right.Y - left.Y * (Double) right.X));

   public Vector3 Cross(Vector3 other) => Cross(this, other);

   public Vector3 DividedBy(Double a) => new(X / a, Y / a, Z / a);

   public Single Dot(Vector3 other) => Dot(this, other);

   public static Single Dot(Vector3 left,
                            Vector3 right) =>
      (Single) (left.X * (Double) right.X + left.Y * (Double) right.Y + left.Z * (Double) right.Z);

   public static Single Dot(Vector3 left,
                            IPoint3D right) =>
      (Single) (left.X * (Double) right.X + left.Y * (Double) right.Y +
                left.Z * (Double) right.Z);

   public Single Length() => (Single) Math.Sqrt(X * (Double) X + Y * (Double) Y + Z * (Double) Z);

   public Double LengthSquared() => Dot(this);

   public Vector3 Minus(Vector3 a) => new(X - a.X, Y - a.Y, Z - a.Z);


   public void Normalize()
   {
      var num1 = Length();
      if (num1 <= 9.99999997475243E-07)
         return;
      var num2 = 1f / num1;
      X *= num2;
      Y *= num2;
      Z *= num2;
   }

   public static Vector3 operator +(Vector3 a,
                                    Vector3 b) =>
      a.Plus(b);

   public static Vector3 operator +(Vector3 a,
                                    Single b) =>
      new(a.X + b, a.Y + b, a.Z + b);

   public static Vector3 operator /(Vector3 a,
                                    Double d) =>
      a.DividedBy(d);

   public static Vector3 operator *(Vector3 a,
                                    Double d) =>
      new(a.X * d, a.Y * d, a.Z * d);

   public static Vector3 operator -(Vector3 a,
                                    Vector3 b) =>
      a.Minus(b);

   public static Vector3 operator -(Vector3 a,
                                    Single b) =>
      new(a.X - b, a.Y - b, a.Z - b);

   public Vector3 Plus(Vector3 a) => new(X + a.X, Y + a.Y, Z + a.Z);

   public Vector3 Rotate(Single x,
                         Single y,
                         Single z) =>
      new((X + x) % 360,
         (Y + y) % 360,
         (Z + z) % 360);

   public static Vector3 Subtract(Vector3 left,
                                  Vector3 right) =>
      new(left.X - right.X,
         left.Y - right.Y,
         left.Z - right.Z);


   public static Vector3 Subtract(IPoint3D left,
                                  IPoint3D right) =>
      new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

   public override String ToString() => $"{X},{Y},{Z}";

   public static Vector3 TransformCoordinate(IPoint3D coordinate,
                                             Matrix transform)
   {
      var vector4 = new Vector4();
      vector4.X = (Single) (coordinate.X * (Double) transform.M11 + coordinate.Y * (Double) transform.M21 +
                            coordinate.Z * (Double) transform.M31) + transform.M41;
      vector4.Y = (Single) (coordinate.X * (Double) transform.M12 + coordinate.Y * (Double) transform.M22 +
                            coordinate.Z * (Double) transform.M32) + transform.M42;
      vector4.Z = (Single) (coordinate.X * (Double) transform.M13 + coordinate.Y * (Double) transform.M23 +
                            coordinate.Z * (Double) transform.M33) + transform.M43;
      vector4.W = (Single) (1.0 / (coordinate.X * (Double) transform.M14 + coordinate.Y * (Double) transform.M24 +
                                   coordinate.Z * (Double) transform.M34 + transform.M44));
      var result = new Vector3(vector4.X * vector4.W, vector4.Y * vector4.W, vector4.Z * vector4.W);

      return result;
   }

   public static readonly Vector3 UnitY = new(0.0f, 1f, 0.0f);
}