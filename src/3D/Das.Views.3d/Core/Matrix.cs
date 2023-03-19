using System;
using System.Globalization;
using System.Threading.Tasks;
using Das.Views.Extended.Core;

namespace Das.Views.Extended
{
   public struct Matrix
   {
      /// <summary>
      ///    The first row/first column
      /// </summary>
      public Single M11;

      /// <summary>
      ///    The first row/second column
      /// </summary>
      public Single M12;

      public Single M13;

      public Single M14;

      public Single M21;

      public Single M22;

      public Single M23;

      public Single M24;

      public Single M31;

      public Single M32;

      public Single M33;

      public Single M34;

      public Single M41;

      public Single M42;

      public Single M43;

      public Single M44;

      public static readonly Matrix Identity = new Matrix {M11 = 1f, M22 = 1f, M33 = 1f, M44 = 1f};

      public static Matrix PerspectiveFovRH(Single fov,
                                            Single aspect,
                                            Single znear,
                                            Single zfar)
      {
         PerspectiveFovRH(fov, aspect, znear, zfar, out var result);
         return result;
      }

      public static void PerspectiveFovRH(Single fov,
                                          Single aspect,
                                          Single znear,
                                          Single zfar,
                                          out Matrix result)
      {
         var num1 = (Single) (1.0 / Math.Tan(fov * 0.5));
         var num2 = num1 / aspect;
         var right = znear / num2;
         var top = znear / num1;
         PerspectiveOffCenterRH(-right, right, -top, top, znear, zfar, out result);
      }

      public static void PerspectiveOffCenterRH(Single left,
                                                Single right,
                                                Single bottom,
                                                Single top,
                                                Single znear,
                                                Single zfar,
                                                out Matrix result)
      {
         PerspectiveOffCenterLH(left, right, bottom, top, znear, zfar, out result);
         result.M31 *= -1f;
         result.M32 *= -1f;
         result.M33 *= -1f;
         result.M34 *= -1f;
      }

      public static void PerspectiveOffCenterLH(Single left,
                                                Single right,
                                                Single bottom,
                                                Single top,
                                                Single znear,
                                                Single zfar,
                                                out Matrix result)
      {
         var num = zfar / (zfar - znear);
         result = new Matrix();
         result.M11 = (Single) (2.0 * znear / (right - (Double) left));
         result.M22 = (Single) (2.0 * znear / (top - (Double) bottom));
         result.M31 = (Single) ((left + (Double) right) / (left - (Double) right));
         result.M32 = (Single) ((top + (Double) bottom) / (bottom - (Double) top));
         result.M33 = num;
         result.M34 = 1f;
         result.M43 = -znear * num;
      }

      public static Matrix LookAtLH(ICamera camera)
      {
         var target = camera.FocalPoint;
         var eye = camera.Position;

         //Vector3 result1;
         var result1 = Vector3.Subtract(target, eye);

         result1.Normalize();

         var result2 = Vector3.Cross(Vector3.UnitY, result1);
         result2.Normalize();

         var result3 = Vector3.Cross(result1, result2);

         var result = Identity;
         result.M11 = result2.X;
         result.M21 = result2.Y;
         result.M31 = result2.Z;
         result.M12 = result3.X;
         result.M22 = result3.Y;
         result.M32 = result3.Z;
         result.M13 = result1.X;
         result.M23 = result1.Y;
         result.M33 = result1.Z;
         result.M41 = Vector3.Dot(result2, eye);
         result.M42 = Vector3.Dot(result3, eye);
         result.M43 = Vector3.Dot(result1, eye);

         result.M41 = -result.M41;
         result.M42 = -result.M42;
         result.M43 = -result.M43;

         return result;
      }


      public override String ToString()
      {
         return String.Format(CultureInfo.CurrentCulture,
            "[M11:{0} M12:{1} M13:{2} M14:{3}] [M21:{4} M22:{5} M23:{6} M24:{7}] [M31:{8} M32:{9} M33:{10} M34:{11}] [M41:{12} M42:{13} M43:{14} M44:{15}]",
            M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44);
      }

      public static Matrix LookAtLH(Vector3 eye,
                                    Vector3 target,
                                    Vector3 up)
      {
         var result1 = Vector3.Subtract(target, eye);
         result1.Normalize();

         var result2 = Vector3.Cross(up, result1);
         result2.Normalize();

         var result3 = Vector3.Cross(result1, result2);
         var result = Identity;
         result.M11 = result2.X;
         result.M21 = result2.Y;
         result.M31 = result2.Z;
         result.M12 = result3.X;
         result.M22 = result3.Y;
         result.M32 = result3.Z;
         result.M13 = result1.X;
         result.M23 = result1.Y;
         result.M33 = result1.Z;
         result.M41 = Vector3.Dot(result2, eye);
         result.M42 = Vector3.Dot(result3, eye);
         result.M43 = Vector3.Dot(result1, eye);
         result.M41 = -result.M41;
         result.M42 = -result.M42;
         result.M43 = -result.M43;

         return result;
      }

      public static Matrix RotationYawPitchRoll(Single yaw,
                                                Single pitch,
                                                Single roll)
      {
         var result1 = Quaternion.RotationYawPitchRoll(yaw, pitch, roll);
         return RotationQuaternion(result1);
      }

      public static Matrix RotationYawPitchRoll(I3DElement element)
      {
         var result1 = Quaternion.RotationYawPitchRoll(element);
         return RotationQuaternion(result1);
      }


      public static Matrix RotationQuaternion(Quaternion rotation)
      {
         var num1 = rotation.X * rotation.X;
         var num2 = rotation.Y * rotation.Y;
         var num3 = rotation.Z * rotation.Z;
         var num4 = rotation.X * rotation.Y;
         var num5 = rotation.Z * rotation.W;
         var num6 = rotation.Z * rotation.X;
         var num7 = rotation.Y * rotation.W;
         var num8 = rotation.Y * rotation.Z;
         var num9 = rotation.X * rotation.W;
         var result = Identity;
         result.M11 = (Single) (1.0 - 2.0 * (num2 + (Double) num3));
         result.M12 = (Single) (2.0 * (num4 + (Double) num5));
         result.M13 = (Single) (2.0 * (num6 - (Double) num7));
         result.M21 = (Single) (2.0 * (num4 - (Double) num5));
         result.M22 = (Single) (1.0 - 2.0 * (num3 + (Double) num1));
         result.M23 = (Single) (2.0 * (num8 + (Double) num9));
         result.M31 = (Single) (2.0 * (num6 + (Double) num7));
         result.M32 = (Single) (2.0 * (num8 - (Double) num9));
         result.M33 = (Single) (1.0 - 2.0 * (num2 + (Double) num1));

         return result;
      }


      public static Matrix Translation(IPoint3D value)
      {
         var result = Identity;
         result.M41 = value.X;
         result.M42 = value.Y;
         result.M43 = value.Z;
         return result;
      }

      public static void Translation(Single x,
                                     Single y,
                                     Single z,
                                     out Matrix result)
      {
         result = Identity;
         result.M41 = x;
         result.M42 = y;
         result.M43 = z;
      }

      public static Matrix operator *(Matrix left,
                                      Matrix right)
      {
         return Multiply(left, right);
      }


      public static Matrix Multiply(Matrix left,
                                    Matrix right)
      {
         var result = new Matrix();
         result.M11 = (Single) (left.M11 * (Double) right.M11 + left.M12 * (Double) right.M21 +
                                left.M13 * (Double) right.M31 + left.M14 * (Double) right.M41);
         result.M12 = (Single) (left.M11 * (Double) right.M12 + left.M12 * (Double) right.M22 +
                                left.M13 * (Double) right.M32 + left.M14 * (Double) right.M42);
         result.M13 = (Single) (left.M11 * (Double) right.M13 + left.M12 * (Double) right.M23 +
                                left.M13 * (Double) right.M33 + left.M14 * (Double) right.M43);
         result.M14 = (Single) (left.M11 * (Double) right.M14 + left.M12 * (Double) right.M24 +
                                left.M13 * (Double) right.M34 + left.M14 * (Double) right.M44);
         result.M21 = (Single) (left.M21 * (Double) right.M11 + left.M22 * (Double) right.M21 +
                                left.M23 * (Double) right.M31 + left.M24 * (Double) right.M41);
         result.M22 = (Single) (left.M21 * (Double) right.M12 + left.M22 * (Double) right.M22 +
                                left.M23 * (Double) right.M32 + left.M24 * (Double) right.M42);
         result.M23 = (Single) (left.M21 * (Double) right.M13 + left.M22 * (Double) right.M23 +
                                left.M23 * (Double) right.M33 + left.M24 * (Double) right.M43);
         result.M24 = (Single) (left.M21 * (Double) right.M14 + left.M22 * (Double) right.M24 +
                                left.M23 * (Double) right.M34 + left.M24 * (Double) right.M44);
         result.M31 = (Single) (left.M31 * (Double) right.M11 + left.M32 * (Double) right.M21 +
                                left.M33 * (Double) right.M31 + left.M34 * (Double) right.M41);
         result.M32 = (Single) (left.M31 * (Double) right.M12 + left.M32 * (Double) right.M22 +
                                left.M33 * (Double) right.M32 + left.M34 * (Double) right.M42);
         result.M33 = (Single) (left.M31 * (Double) right.M13 + left.M32 * (Double) right.M23 +
                                left.M33 * (Double) right.M33 + left.M34 * (Double) right.M43);
         result.M34 = (Single) (left.M31 * (Double) right.M14 + left.M32 * (Double) right.M24 +
                                left.M33 * (Double) right.M34 + left.M34 * (Double) right.M44);
         result.M41 = (Single) (left.M41 * (Double) right.M11 + left.M42 * (Double) right.M21 +
                                left.M43 * (Double) right.M31 + left.M44 * (Double) right.M41);
         result.M42 = (Single) (left.M41 * (Double) right.M12 + left.M42 * (Double) right.M22 +
                                left.M43 * (Double) right.M32 + left.M44 * (Double) right.M42);
         result.M43 = (Single) (left.M41 * (Double) right.M13 + left.M42 * (Double) right.M23 +
                                left.M43 * (Double) right.M33 + left.M44 * (Double) right.M43);
         result.M44 = (Single) (left.M41 * (Double) right.M14 + left.M42 * (Double) right.M24 +
                                left.M43 * (Double) right.M34 + left.M44 * (Double) right.M44);
         return result;
      }

      public static Matrix Multiply(Matrix left,
                                    Single right)
      {
         var result = new Matrix();

         result.M11 = left.M11 * right;
         result.M12 = left.M12 * right;
         result.M13 = left.M13 * right;
         result.M14 = left.M14 * right;
         result.M21 = left.M21 * right;
         result.M22 = left.M22 * right;
         result.M23 = left.M23 * right;
         result.M24 = left.M24 * right;
         result.M31 = left.M31 * right;
         result.M32 = left.M32 * right;
         result.M33 = left.M33 * right;
         result.M34 = left.M34 * right;
         result.M41 = left.M41 * right;
         result.M42 = left.M42 * right;
         result.M43 = left.M43 * right;
         result.M44 = left.M44 * right;

         return result;
      }
   }
}
