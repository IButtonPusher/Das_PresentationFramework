using System;

namespace Das.Views.Core.Drawing
{
    public class Color : IColor, IEquatable<Color>
    {
        public Byte A { get; }

        public Byte B { get; }

        public Byte R { get; }

        public Byte G { get; }

        private readonly Int32 _hash;

        public Color(Byte a, Byte r, Byte g, Byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;

            _hash = r + (g << 8) + (b << 16) + (a << 24);
        }

        public Color(Byte r, Byte g, Byte b)
            : this(255, r, g, b)
        {
        }

        public static Color FromRgb(Byte r, Byte g, Byte b) => new Color(r,g,b);

        public static Color White => new Color(255, 255, 255);

        public static Color Red => new Color(255, 0, 0);

        public static Color Black => new Color(0, 0, 0);

        public static Color Transparent => new Color(0, 0, 0, 0);

        public static Color Orange => new Color(255, 127, 39);

        public static Color Yellow => new Color(255, 255, 0, 255);

        public static Color Green => _green.Value;

        private static readonly Lazy<Color> _green = new Lazy<Color>(()
            => new Color(0, 255, 0));

        public static Color Pink => _pink.Value;

        private static readonly Lazy<Color> _pink = new Lazy<Color>(()
            => new Color(255, 106, 155));

        public static Color DarkGray => _darkGray.Value;

        private static readonly Lazy<Color> _darkGray = new Lazy<Color>(()
            => new Color(30, 30, 30));

        public Boolean Equals(Color other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return other._hash == _hash;
        }

        public override Int32 GetHashCode() => _hash;

        public override String ToString() => !Enum.IsDefined(typeof(Colors), _hash)
            ? $"{R}, {G}, {B} - {A}"
            : ((Colors) _hash).ToString();
    }
}