using System;
using System.Runtime.InteropServices;
// ReSharper disable All

namespace Das.OpenGL.Text
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Fixed26Dot6 : IEquatable<Fixed26Dot6>, IComparable<Fixed26Dot6>
    {
        private Int32 value;
        
        public Fixed26Dot6(Int32 value)
        {
            this.value = value << 6;
        }
        
        public Fixed26Dot6(Single value)
        {
            this.value = (Int32)(value * 64);
        }
        
        public Fixed26Dot6(Double value)
        {
            this.value = (Int32)(value * 64);
        }
        
        public Fixed26Dot6(Decimal value)
        {
            this.value = (Int32)(value * 64);
        }
      
        public Int32 Value => value;
        
        public static Fixed26Dot6 FromRawValue(Int32 value)
        {
            var f = new Fixed26Dot6();
            f.value = value;
            return f;
        }
        
        public static Fixed26Dot6 FromInt32(Int32 value) => new Fixed26Dot6(value);

       
        public static Fixed26Dot6 FromSingle(Single value) => new Fixed26Dot6(value);

       
        public static Fixed26Dot6 FromDouble(Double value) => new Fixed26Dot6(value);

       
        public static Fixed26Dot6 FromDecimal(Decimal value) => new Fixed26Dot6(value);

      
        public static Fixed26Dot6 Add(Fixed26Dot6 left, Fixed26Dot6 right) => FromRawValue(left.value + right.value);

       
        public static Fixed26Dot6 Subtract(Fixed26Dot6 left, Fixed26Dot6 right) => FromRawValue(left.value - right.value);

      
        public static Fixed26Dot6 Multiply(Fixed26Dot6 left, Fixed26Dot6 right)
        {
            var mul = left.value * (Int64)right.value;
            var ans = new Fixed26Dot6();
            ans.value = (Int32)(mul >> 6);
            return ans;
        }

       
        public static Fixed26Dot6 Divide(Fixed26Dot6 left, Fixed26Dot6 right)
        {
            var div = ((Int64)left.Value << 6) / right.value;
            var ans = new Fixed26Dot6();
            ans.value = (Int32)div;
            return ans;
        }
      
     
        public static implicit operator Fixed26Dot6(Int16 value) => new Fixed26Dot6(value);
      
        public static implicit operator Fixed26Dot6(Int32 value) => new Fixed26Dot6(value);

     
        public static implicit operator Fixed26Dot6(Single value) => new Fixed26Dot6(value);

       
        public static implicit operator Fixed26Dot6(Double value) => new Fixed26Dot6(value);

      
        public static implicit operator Fixed26Dot6(Decimal value) => new Fixed26Dot6(value);

       
        public static explicit operator Int32(Fixed26Dot6 value) => value.ToInt32();

      
        public static explicit operator Single(Fixed26Dot6 value) => value.ToSingle();
      
        public static implicit operator Double(Fixed26Dot6 value) => value.ToDouble();
        
        public static implicit operator Decimal(Fixed26Dot6 value) => value.ToDecimal();
        
        public static Fixed26Dot6 operator +(Fixed26Dot6 left, Fixed26Dot6 right) => Add(left, right);

       
        public static Fixed26Dot6 operator -(Fixed26Dot6 left, Fixed26Dot6 right) => Subtract(left, right);
       
        public static Fixed26Dot6 operator *(Fixed26Dot6 left, Fixed26Dot6 right) => Multiply(left, right);
        
        public static Fixed26Dot6 operator /(Fixed26Dot6 left, Fixed26Dot6 right) => Divide(left, right);
        
        public static Boolean operator ==(Fixed26Dot6 left, Fixed26Dot6 right) => left.Equals(right);
        
        public static Boolean operator !=(Fixed26Dot6 left, Fixed26Dot6 right) => !(left == right);

       
        public static Boolean operator <(Fixed26Dot6 left, Fixed26Dot6 right) => left.CompareTo(right) < 0;

        public static Boolean operator <=(Fixed26Dot6 left, Fixed26Dot6 right) => left.CompareTo(right) <= 0;

       
        public static Boolean operator >(Fixed26Dot6 left, Fixed26Dot6 right) => left.CompareTo(right) > 0;

      
        public static Boolean operator >=(Fixed26Dot6 left, Fixed26Dot6 right) => left.CompareTo(right) >= 0;

      
        public Int32 Floor() => value >> 6;

      
        public Int32 Round() => (value + 32) >> 6;

       
        public Int32 Ceiling() => (value + 63) >> 6;

      
        public Int32 ToInt32() => Floor();

     
        public Single ToSingle() => value / 64f;

      
        public Double ToDouble() => value / 64d;

     
        public Decimal ToDecimal() => value / 64m;
     
        public Boolean Equals(Fixed26Dot6 other) => value == other.value;
        
        public Int32 CompareTo(Fixed26Dot6 other) => value.CompareTo(other.value);
       
        public String ToString(IFormatProvider provider) => ToDecimal().ToString(provider);

        
        public String ToString(String format) => ToDecimal().ToString(format);

        
        public String ToString(String format, IFormatProvider provider) => ToDecimal().ToString(format, provider);

        public override String ToString() => ToDecimal().ToString();
        
        public override Int32 GetHashCode() => value.GetHashCode();

        public override Boolean Equals(Object obj)
        {
            if (obj is Fixed26Dot6)
                return Equals((Fixed26Dot6)obj);
            else if (obj is Int32)
                return value == ((Fixed26Dot6)obj).value;
            else
                return false;
        }
    }
}
