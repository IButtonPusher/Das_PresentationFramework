using System;
using System.Globalization;

namespace Das.Views.Extended.Core
{
    public struct Matrix 
    {
        public float M11;
        /// <summary>Value at row 1 column 2 of the matrix.</summary>
        public float M12;
        /// <summary>Value at row 1 column 3 of the matrix.</summary>
        public float M13;
        /// <summary>Value at row 1 column 4 of the matrix.</summary>
        public float M14;
        /// <summary>Value at row 2 column 1 of the matrix.</summary>
        public float M21;
        /// <summary>Value at row 2 column 2 of the matrix.</summary>
        public float M22;
        /// <summary>Value at row 2 column 3 of the matrix.</summary>
        public float M23;
        /// <summary>Value at row 2 column 4 of the matrix.</summary>
        public float M24;
        /// <summary>Value at row 3 column 1 of the matrix.</summary>
        public float M31;
        /// <summary>Value at row 3 column 2 of the matrix.</summary>
        public float M32;
        /// <summary>Value at row 3 column 3 of the matrix.</summary>
        public float M33;
        /// <summary>Value at row 3 column 4 of the matrix.</summary>
        public float M34;
        /// <summary>Value at row 4 column 1 of the matrix.</summary>
        public float M41;
        /// <summary>Value at row 4 column 2 of the matrix.</summary>
        public float M42;
        /// <summary>Value at row 4 column 3 of the matrix.</summary>
        public float M43;
        /// <summary>Value at row 4 column 4 of the matrix.</summary>
        public float M44;

        public static readonly Matrix Identity = new Matrix()
            { M11 = 1f, M22 = 1f, M33 = 1f, M44 = 1f };

        public static Matrix PerspectiveFovRH(float fov, float aspect, float znear, float zfar)
        {
            PerspectiveFovRH(fov, aspect, znear, zfar, out var result);
            return result;
        }

        public static void PerspectiveFovRH(
            float fov,
            float aspect,
            float znear,
            float zfar,
            out Matrix result)
        {
            var num1 = (float)(1.0 / Math.Tan(fov * 0.5));
            var num2 = num1 / aspect;
            var right = znear / num2;
            var top = znear / num1;
            PerspectiveOffCenterRH(-right, right, -top, top, znear, zfar, out result);
        }

        public static void PerspectiveOffCenterRH(
            float left,
            float right,
            float bottom,
            float top,
            float znear,
            float zfar,
            out Matrix result)
        {
            PerspectiveOffCenterLH(left, right, bottom, top, znear, zfar, out result);
            result.M31 *= -1f;
            result.M32 *= -1f;
            result.M33 *= -1f;
            result.M34 *= -1f;
        }

        public static void PerspectiveOffCenterLH(
            float left,
            float right,
            float bottom,
            float top,
            float znear,
            float zfar,
            out Matrix result)
        {
            var num = zfar / (zfar - znear);
            result = new Matrix();
            result.M11 = (float)(2.0 * znear / (right - (double)left));
            result.M22 = (float)(2.0 * znear / (top - (double)bottom));
            result.M31 = (float)((left + (double)right) / (left - (double)right));
            result.M32 = (float)((top + (double)bottom) / (bottom - (double)top));
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


        public override string ToString() => 
            String.Format(CultureInfo.CurrentCulture, "[M11:{0} M12:{1} M13:{2} M14:{3}] [M21:{4} M22:{5} M23:{6} M24:{7}] [M31:{8} M32:{9} M33:{10} M34:{11}] [M41:{12} M42:{13} M43:{14} M44:{15}]", (object)M11, (object)M12, (object)M13, (object)M14, (object)M21, (object)M22, (object)M23, (object)M24, (object)M31, (object)M32, (object)M33, (object)M34, (object)M41, (object)M42, (object)M43, (object)M44);

        public static Matrix LookAtLH(
            Vector3 eye,
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

        public static Matrix RotationYawPitchRoll(float yaw, float pitch, float roll)
        {
            var result1 = Quaternion.RotationYawPitchRoll(yaw, pitch, roll);
            return RotationQuaternion(result1);
        }

        public static Matrix RotationYawPitchRoll(I3dElement element)
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
            result.M11 = (float)(1.0 - 2.0 * (num2 + (double)num3));
            result.M12 = (float)(2.0 * (num4 + (double)num5));
            result.M13 = (float)(2.0 * (num6 - (double)num7));
            result.M21 = (float)(2.0 * (num4 - (double)num5));
            result.M22 = (float)(1.0 - 2.0 * (num3 + (double)num1));
            result.M23 = (float)(2.0 * (num8 + (double)num9));
            result.M31 = (float)(2.0 * (num6 + (double)num7));
            result.M32 = (float)(2.0 * (num8 - (double)num9));
            result.M33 = (float)(1.0 - 2.0 * (num2 + (double)num1));

            return result;
        }




        public static Matrix Translation(IPoint3d value)
        {
            var result = Identity;
            result.M41 = value.X;
            result.M42 = value.Y;
            result.M43 = value.Z;
            return result;
        }

        public static void Translation(float x, float y, float z, out Matrix result)
        {
            result = Identity;
            result.M41 = x;
            result.M42 = y;
            result.M43 = z;
        }

        public static Matrix operator *(Matrix left, Matrix right) => Multiply(left, right);


        public static Matrix Multiply(Matrix left, Matrix right)
        {
            var result = new Matrix();
            result.M11 = (float)(left.M11 * (double)right.M11 + left.M12 * (double)right.M21 + left.M13 * (double)right.M31 + left.M14 * (double)right.M41);
            result.M12 = (float)(left.M11 * (double)right.M12 + left.M12 * (double)right.M22 + left.M13 * (double)right.M32 + left.M14 * (double)right.M42);
            result.M13 = (float)(left.M11 * (double)right.M13 + left.M12 * (double)right.M23 + left.M13 * (double)right.M33 + left.M14 * (double)right.M43);
            result.M14 = (float)(left.M11 * (double)right.M14 + left.M12 * (double)right.M24 + left.M13 * (double)right.M34 + left.M14 * (double)right.M44);
            result.M21 = (float)(left.M21 * (double)right.M11 + left.M22 * (double)right.M21 + left.M23 * (double)right.M31 + left.M24 * (double)right.M41);
            result.M22 = (float)(left.M21 * (double)right.M12 + left.M22 * (double)right.M22 + left.M23 * (double)right.M32 + left.M24 * (double)right.M42);
            result.M23 = (float)(left.M21 * (double)right.M13 + left.M22 * (double)right.M23 + left.M23 * (double)right.M33 + left.M24 * (double)right.M43);
            result.M24 = (float)(left.M21 * (double)right.M14 + left.M22 * (double)right.M24 + left.M23 * (double)right.M34 + left.M24 * (double)right.M44);
            result.M31 = (float)(left.M31 * (double)right.M11 + left.M32 * (double)right.M21 + left.M33 * (double)right.M31 + left.M34 * (double)right.M41);
            result.M32 = (float)(left.M31 * (double)right.M12 + left.M32 * (double)right.M22 + left.M33 * (double)right.M32 + left.M34 * (double)right.M42);
            result.M33 = (float)(left.M31 * (double)right.M13 + left.M32 * (double)right.M23 + left.M33 * (double)right.M33 + left.M34 * (double)right.M43);
            result.M34 = (float)(left.M31 * (double)right.M14 + left.M32 * (double)right.M24 + left.M33 * (double)right.M34 + left.M34 * (double)right.M44);
            result.M41 = (float)(left.M41 * (double)right.M11 + left.M42 * (double)right.M21 + left.M43 * (double)right.M31 + left.M44 * (double)right.M41);
            result.M42 = (float)(left.M41 * (double)right.M12 + left.M42 * (double)right.M22 + left.M43 * (double)right.M32 + left.M44 * (double)right.M42);
            result.M43 = (float)(left.M41 * (double)right.M13 + left.M42 * (double)right.M23 + left.M43 * (double)right.M33 + left.M44 * (double)right.M43);
            result.M44 = (float)(left.M41 * (double)right.M14 + left.M42 * (double)right.M24 + left.M43 * (double)right.M34 + left.M44 * (double)right.M44);
            return result;
        }

        public static Matrix Multiply(Matrix left, float right)
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
