using System;
using System.Runtime.InteropServices;
// ReSharper disable All

namespace Das.OpenGL.Text
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Fixed26Dot6 : IEquatable<Fixed26Dot6>, IComparable<Fixed26Dot6>
    {
        private int value;
        
        public Fixed26Dot6(int value)
        {
            this.value = value << 6;
        }
        
        public Fixed26Dot6(float value)
        {
            this.value = (int)(value * 64);
        }
        
        public Fixed26Dot6(double value)
        {
            this.value = (int)(value * 64);
        }
        
        public Fixed26Dot6(decimal value)
        {
            this.value = (int)(value * 64);
        }
      
        public int Value => value;
        
        public static Fixed26Dot6 FromRawValue(int value)
        {
            var f = new Fixed26Dot6();
            f.value = value;
            return f;
        }
        
        public static Fixed26Dot6 FromInt32(int value) => new Fixed26Dot6(value);

       
        public static Fixed26Dot6 FromSingle(float value) => new Fixed26Dot6(value);

       
        public static Fixed26Dot6 FromDouble(double value) => new Fixed26Dot6(value);

       
        public static Fixed26Dot6 FromDecimal(decimal value) => new Fixed26Dot6(value);

      
        public static Fixed26Dot6 Add(Fixed26Dot6 left, Fixed26Dot6 right) => FromRawValue(left.value + right.value);

       
        public static Fixed26Dot6 Subtract(Fixed26Dot6 left, Fixed26Dot6 right) => FromRawValue(left.value - right.value);

      
        public static Fixed26Dot6 Multiply(Fixed26Dot6 left, Fixed26Dot6 right)
        {
            var mul = left.value * (long)right.value;
            var ans = new Fixed26Dot6();
            ans.value = (int)(mul >> 6);
            return ans;
        }

       
        public static Fixed26Dot6 Divide(Fixed26Dot6 left, Fixed26Dot6 right)
        {
            var div = ((long)left.Value << 6) / right.value;
            var ans = new Fixed26Dot6();
            ans.value = (int)div;
            return ans;
        }
      
     
        public static implicit operator Fixed26Dot6(short value) => new Fixed26Dot6(value);
      
        public static implicit operator Fixed26Dot6(int value) => new Fixed26Dot6(value);

     
        public static implicit operator Fixed26Dot6(float value) => new Fixed26Dot6(value);

       
        public static implicit operator Fixed26Dot6(double value) => new Fixed26Dot6(value);

      
        public static implicit operator Fixed26Dot6(decimal value) => new Fixed26Dot6(value);

       
        public static explicit operator int(Fixed26Dot6 value) => value.ToInt32();

      
        public static explicit operator float(Fixed26Dot6 value) => value.ToSingle();
      
        public static implicit operator double(Fixed26Dot6 value) => value.ToDouble();
        
        public static implicit operator decimal(Fixed26Dot6 value) => value.ToDecimal();
        
        public static Fixed26Dot6 operator +(Fixed26Dot6 left, Fixed26Dot6 right) => Add(left, right);

       
        public static Fixed26Dot6 operator -(Fixed26Dot6 left, Fixed26Dot6 right) => Subtract(left, right);
       
        public static Fixed26Dot6 operator *(Fixed26Dot6 left, Fixed26Dot6 right) => Multiply(left, right);
        
        public static Fixed26Dot6 operator /(Fixed26Dot6 left, Fixed26Dot6 right) => Divide(left, right);
        
        public static bool operator ==(Fixed26Dot6 left, Fixed26Dot6 right) => left.Equals(right);
        
        public static bool operator !=(Fixed26Dot6 left, Fixed26Dot6 right) => !(left == right);

       
        public static bool operator <(Fixed26Dot6 left, Fixed26Dot6 right) => left.CompareTo(right) < 0;

        public static bool operator <=(Fixed26Dot6 left, Fixed26Dot6 right) => left.CompareTo(right) <= 0;

       
        public static bool operator >(Fixed26Dot6 left, Fixed26Dot6 right) => left.CompareTo(right) > 0;

      
        public static bool operator >=(Fixed26Dot6 left, Fixed26Dot6 right) => left.CompareTo(right) >= 0;

      
        public int Floor() => value >> 6;

      
        public int Round() => (value + 32) >> 6;

       
        public int Ceiling() => (value + 63) >> 6;

      
        public int ToInt32() => Floor();

     
        public float ToSingle() => value / 64f;

      
        public double ToDouble() => value / 64d;

     
        public decimal ToDecimal() => value / 64m;
     
        public bool Equals(Fixed26Dot6 other) => value == other.value;
        
        public int CompareTo(Fixed26Dot6 other) => value.CompareTo(other.value);
       
        public string ToString(IFormatProvider provider) => ToDecimal().ToString(provider);

        
        public string ToString(string format) => ToDecimal().ToString(format);

        
        public string ToString(string format, IFormatProvider provider) => ToDecimal().ToString(format, provider);

        public override string ToString() => ToDecimal().ToString();
        
        public override int GetHashCode() => value.GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj is Fixed26Dot6)
                return Equals((Fixed26Dot6)obj);
            else if (obj is int)
                return value == ((Fixed26Dot6)obj).value;
            else
                return false;
        }
    }
}
