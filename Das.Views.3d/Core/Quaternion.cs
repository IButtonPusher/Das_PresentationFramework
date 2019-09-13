using System;

namespace Das.Views.Extended.Core
{
    public class Quaternion : Vector4
    {
        public Quaternion(float x, float y, float z, float w) : base(x,y,z,w)
        {
            
        }

        public Quaternion()
        {
            
        }

        public static Quaternion RotationYawPitchRoll(I3dElement element)
        {
            var yaw = element.Rotation.Y;
            var pitch = element.Rotation.X;
            var roll = element.Rotation.Z;

            var num1 = roll * 0.5f;
            var num2 = pitch * 0.5f;
            var num3 = yaw * 0.5f;
            var num4 = (float)Math.Sin(num1);
            var num5 = (float)Math.Cos(num1);
            var num6 = (float)Math.Sin(num2);
            var num7 = (float)Math.Cos(num2);
            var num8 = (float)Math.Sin(num3);
            var num9 = (float)Math.Cos(num3);


            var result = new Quaternion(
            (float)(num9 * (double)num6 * num5 + num8 * (double)num7 * num4),
            (float)(num8 * (double)num7 * num5 - num9 * (double)num6 * num4),
            (float)(num9 * (double)num7 * num4 - num8 * (double)num6 * num5),
            (float)(num9 * (double)num7 * num5 + num8 * (double)num6 * num4));

            return result;
        }

        public static Quaternion RotationYawPitchRoll(
            float yaw,
            float pitch,
            float roll)
        {
            var result = new Quaternion();

            var num1 = roll * 0.5f;
            var num2 = pitch * 0.5f;
            var num3 = yaw * 0.5f;
            var num4 = (float)Math.Sin(num1);
            var num5 = (float)Math.Cos(num1);
            var num6 = (float)Math.Sin(num2);
            var num7 = (float)Math.Cos(num2);
            var num8 = (float)Math.Sin(num3);
            var num9 = (float)Math.Cos(num3);
            result.X = (float)(num9 * (double)num6 * num5 + num8 * (double)num7 * num4);
            result.Y = (float)(num8 * (double)num7 * num5 - num9 * (double)num6 * num4);
            result.Z = (float)(num9 * (double)num7 * num4 - num8 * (double)num6 * num5);
            result.W = (float)(num9 * (double)num7 * num5 + num8 * (double)num6 * num4);

            return result;
        }
    }
}
