using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Das.Views.Images.Svg.Helpers
{
    public static class StringParser
    {
        public static Double ToDouble(ref ReadOnlySpan<Char> value)
        {
            #if NETSTANDARD2_1 || NETCORE || NETCOREAPP2_1 || NETCOREAPP3_1 || NET5_0
            return double.Parse(value, NumberStyles.Any, Format);
            #else
            return Double.Parse(value.ToString(), NumberStyles.Any, Format);
            #endif
        }

        public static Single ToFloat(ref ReadOnlySpan<Char> value)
        {
            #if NETSTANDARD2_1 || NETCORE || NETCOREAPP2_1 || NETCOREAPP3_1 || NET5_0
            return float.Parse(value, NumberStyles.Float, Format);
            #else
            return Single.Parse(value.ToString(), NumberStyles.Float, Format);
            #endif
        }

        public static Single ToFloatAny(ref ReadOnlySpan<Char> value)
        {
            #if NETSTANDARD2_1 || NETCORE || NETCOREAPP2_1 || NETCOREAPP3_1 || NET5_0
            return float.Parse(value, NumberStyles.Any, Format);
            #else
            return Single.Parse(value.ToString(), NumberStyles.Any, Format);
            #endif
        }

        public static Int32 ToInt(ref ReadOnlySpan<Char> value)
        {
            #if NETSTANDARD2_1 || NETCORE || NETCOREAPP2_1 || NETCOREAPP3_1 || NET5_0
            return int.Parse(value, NumberStyles.Integer, Format);
            #else
            return Int32.Parse(value.ToString(), NumberStyles.Integer, Format);
            #endif
        }

        private static readonly CultureInfo Format = CultureInfo.InvariantCulture;
    }
}
