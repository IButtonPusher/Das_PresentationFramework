
using System;

namespace Das.Views.Extended.Core
{
    public class Vector3 : Vector2, IPoint3d
    {
        public static readonly Vector3 UnitY = new Vector3(0.0f, 1f, 0.0f);
        
        /// <summary>The Z component of the vector.</summary>
        public float Z { get; set; }
        
        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString() => $"{X},{Y},{Z}";

        public Vector3() { }

        public static Vector3 TransformCoordinate(IPoint3d coordinate, Matrix transform)
        {
            var vector4 = new Vector4();
            vector4.X = (float)(coordinate.X * (double)transform.M11 + coordinate.Y * (double)transform.M21 + coordinate.Z * (double)transform.M31) + transform.M41;
            vector4.Y = (float)(coordinate.X * (double)transform.M12 + coordinate.Y * (double)transform.M22 + coordinate.Z * (double)transform.M32) + transform.M42;
            vector4.Z = (float)(coordinate.X * (double)transform.M13 + coordinate.Y * (double)transform.M23 + coordinate.Z * (double)transform.M33) + transform.M43;
            vector4.W = (float)(1.0 / (coordinate.X * (double)transform.M14 + coordinate.Y * (double)transform.M24 + coordinate.Z * (double)transform.M34 + transform.M44));
            var result = new Vector3(vector4.X * vector4.W, vector4.Y * vector4.W, vector4.Z * vector4.W);
            
            return result;
        }

 

        public static Vector3 TransformCoordinate(
            Vector3 coordinate,
            Matrix transform)
        {
            var vector4 = new Vector4();
            vector4.X = (float)(coordinate.X * (double)transform.M11 + coordinate.Y * (double)transform.M21 + coordinate.Z * (double)transform.M31) + transform.M41;
            vector4.Y = (float)(coordinate.X * (double)transform.M12 + coordinate.Y * (double)transform.M22 + coordinate.Z * (double)transform.M32) + transform.M42;
            vector4.Z = (float)(coordinate.X * (double)transform.M13 + coordinate.Y * (double)transform.M23 + coordinate.Z * (double)transform.M33) + transform.M43;
            vector4.W = (float)(1.0 / (coordinate.X * (double)transform.M14 + coordinate.Y * (double)transform.M24 + coordinate.Z * (double)transform.M34 + transform.M44));
            return new Vector3(vector4.X * vector4.W, vector4.Y * vector4.W, vector4.Z * vector4.W);
        }

        public static Vector3 Subtract(Vector3 left, Vector3 right) => new Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

        public static Vector3 Subtract(IPoint3d left, IPoint3d right) 
            => new Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

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

        public float Length() => (float)Math.Sqrt(X * (double)X + Y * (double)Y + Z * (double)Z);

        public static Vector3 Cross(Vector3 left, Vector3 right) => new Vector3((float)(left.Y * (double)right.Z - left.Z * (double)right.Y), (float)(left.Z * (double)right.X - left.X * (double)right.Z), (float)(left.X * (double)right.Y - left.Y * (double)right.X));

        public static Vector3 Cross(IPoint3d left, IPoint3d right) 
            => new Vector3((float)(left.Y * (double)right.Z - left.Z * 
                (double)right.Y), (float)(left.Z * (double)right.X - left.X * (double)right.Z), (float)(left.X * (double)right.Y - left.Y * (double)right.X));

        public static readonly Vector3 Zero = new Vector3();
        
        public static float Dot(Vector3 left, Vector3 right) => (float)(left.X * (double)right.X + left.Y * (double)right.Y + left.Z * (double)right.Z);

        public static float Dot(Vector3 left, IPoint3d right) => 
            (float)(left.X * (double)right.X + left.Y * (double)right.Y + 
            left.Z * (double)right.Z);

        public Vector3 Rotate(float x, float y, float z) => 
            new Vector3(X + x, Y + y, Z + z);

        IPoint3d IPoint3d.Rotate(float x, float y, float z) => Rotate(x, y, z);
    }
}
