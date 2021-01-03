using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Das.Views.Core.Drawing
{
    public class Color : IColor, IEquatable<Color>
    {
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

        public Byte A { get; }

        public Byte B { get; }

        public Byte R { get; }

        public SolidColorBrush ToBrush()
        {
            return _asBrush ??= new SolidColorBrush(this);
        }

        public Byte G { get; }

        public Boolean Equals(Color other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return other._hash == _hash;
        }

        public static Color Black => _black.Value;

        public static Color DarkGray => _darkGray.Value;

        public static Color Gray => _gray.Value;

        public static Color Green => _green.Value;

        public static Color LightGray => _lightGray.Value;

        public static Color Orange => _orange.Value;

        public static Color Pink => _pink.Value;

        public static Color Red => _red.Value;

        public static Color Transparent => _transparent.Value;

        public static Color White => _white.Value;

        public static Color Yellow => _yellow.Value;

        public static Color Purple => _purple.Value;

        public static Color FromRgb(Byte r, 
                                    Byte g, 
                                    Byte b)
        {
            return new Color(r, g, b);
        }

        public static Color FromArgb(Byte alpha,
                                     Byte r,
                                     Byte g,
                                     Byte b)
        {
            return new Color(alpha, r, g, b);
        }

        public static Color FromHex(String hex)
        {
            var start = hex[0] == '#' ? 1 : 0;

            switch (hex.Length - start)
            {
                case 6:
                    return FromRgb(Byte.Parse(hex.Substring(start, 2), NumberStyles.HexNumber),
                        Byte.Parse(hex.Substring(start + 2, 2), NumberStyles.HexNumber),
                        Byte.Parse(hex.Substring(start + 4, 2), NumberStyles.HexNumber));

                case 8:
                    return FromArgb(Byte.Parse(hex.Substring(start, 2), NumberStyles.HexNumber),
                        Byte.Parse(hex.Substring(start + 2, 2), NumberStyles.HexNumber),
                        Byte.Parse(hex.Substring(start + 4, 2), NumberStyles.HexNumber),
                        Byte.Parse(hex.Substring(start + 6, 2), NumberStyles.HexNumber));

                default:
                    throw new InvalidOperationException();
            }
        }

        public override Int32 GetHashCode()
        {
            return _hash;
        }

        public override String ToString()
        {
            return !Enum.IsDefined(typeof(Colors), _hash)
                ? $"{R}, {G}, {B} - {A}"
                : ((Colors) _hash).ToString();
        }

        private static readonly Lazy<Color> _green = new Lazy<Color>(()
            => new Color(0, 255, 0));

        private static readonly Lazy<Color> _pink = new Lazy<Color>(()
            => new Color(255, 106, 155));

        private static readonly Lazy<Color> _darkGray = new Lazy<Color>(()
            => new Color(169, 169, 169));

        private static readonly Lazy<Color> _lightGray = new Lazy<Color>(()
            => new Color(211, 211, 211));

        private static readonly Lazy<Color> _gray = new Lazy<Color>(()
            => new Color(128, 128, 128));

        private static readonly Lazy<Color> _black = new Lazy<Color>(()
            => new Color(0, 0, 0));

        private static readonly Lazy<Color> _orange = new Lazy<Color>(()
            => new Color(255, 127, 39));

        private static readonly Lazy<Color> _red = new Lazy<Color>(()
            => new Color(255, 0, 0));

        private static readonly Lazy<Color> _purple = new Lazy<Color>(()
            => new Color(128, 0, 128));

        private static readonly Lazy<Color> _transparent = new Lazy<Color>(()
            => new Color(0, 0, 0, 0));

        private static readonly Lazy<Color> _white = new Lazy<Color>(()
            => new Color(255, 255, 255));

        private static readonly Lazy<Color> _yellow = new Lazy<Color>(()
            => new Color(255, 255, 0, 255));

        private readonly Int32 _hash;
        private SolidColorBrush? _asBrush;
    }
}