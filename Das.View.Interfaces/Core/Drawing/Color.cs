using System;

namespace Das.Views.Core.Drawing
{
    public class Color : IColor, IEquatable<Color>
    {
        public byte A { get; }

        public byte B { get; }

        public byte R { get; }

        public byte G { get; }

        private readonly Int32 _hash;

        public Color(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;

            _hash = r + (g << 8) + (b << 16) + (a << 24);
        }

        public Color(byte r, byte g, byte b)
            : this(255, r, g, b)
        {
        }

        public static Color FromRgb(byte r, byte g, byte b) => new Color(r,g,b);

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

        public bool Equals(Color other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return other._hash == _hash;
        }

        public override int GetHashCode() => _hash;

        public override string ToString() => !Enum.IsDefined(typeof(Colors), _hash)
            ? $"{R}, {G}, {B} - {A}"
            : ((Colors) _hash).ToString();
    }
}