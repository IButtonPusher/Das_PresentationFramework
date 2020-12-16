using System;

namespace Das.Views.Extended
{
    public class Quaternion : Vector4
    {
        public Quaternion(Single x, Single y, Single z, Single w) : base(x,y,z,w)
        {
            
        }

        public Quaternion()
        {
            
        }

        public static Quaternion RotationYawPitchRoll(I3DElement element)
        {
            var yaw = element.Rotation.Y;
            var pitch = element.Rotation.X;
            var roll = element.Rotation.Z;

            var num1 = roll * 0.5f;
            var num2 = pitch * 0.5f;
            var num3 = yaw * 0.5f;
            var num4 = (Single)Math.Sin(num1);
            var num5 = (Single)Math.Cos(num1);
            var num6 = (Single)Math.Sin(num2);
            var num7 = (Single)Math.Cos(num2);
            var num8 = (Single)Math.Sin(num3);
            var num9 = (Single)Math.Cos(num3);


            var result = new Quaternion(
            (Single)(num9 * (Double)num6 * num5 + num8 * (Double)num7 * num4),
            (Single)(num8 * (Double)num7 * num5 - num9 * (Double)num6 * num4),
            (Single)(num9 * (Double)num7 * num4 - num8 * (Double)num6 * num5),
            (Single)(num9 * (Double)num7 * num5 + num8 * (Double)num6 * num4));

            return result;
        }

        public static Quaternion RotationYawPitchRoll(
            Single yaw,
            Single pitch,
            Single roll)
        {
            var result = new Quaternion();

            var num1 = roll * 0.5f;
            var num2 = pitch * 0.5f;
            var num3 = yaw * 0.5f;
            var num4 = (Single)Math.Sin(num1);
            var num5 = (Single)Math.Cos(num1);
            var num6 = (Single)Math.Sin(num2);
            var num7 = (Single)Math.Cos(num2);
            var num8 = (Single)Math.Sin(num3);
            var num9 = (Single)Math.Cos(num3);
            result.X = (Single)(num9 * (Double)num6 * num5 + num8 * (Double)num7 * num4);
            result.Y = (Single)(num8 * (Double)num7 * num5 - num9 * (Double)num6 * num4);
            result.Z = (Single)(num9 * (Double)num7 * num4 - num8 * (Double)num6 * num5);
            result.W = (Single)(num9 * (Double)num7 * num5 + num8 * (Double)num6 * num4);

            return result;
        }
    }
}
