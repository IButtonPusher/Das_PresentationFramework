using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Drawing
{
    public class SolidColorBrush : IEquatable<SolidColorBrush>, IBrush
    {
        public SolidColorBrush(Color color)
        {
            Color = color;
        }

        //IColor IBrush.Color => Color;

        public Boolean Equals(SolidColorBrush? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Color, other.Color);
        }

        public static SolidColorBrush Black { get; } = new SolidColorBrush(Color.Black);

        public Color Color { get; }

        public static SolidColorBrush DarkGray => _darkGray.Value;

        public static SolidColorBrush LightGray => _lightGray.Value;

        public virtual Boolean IsInvisible => Color.A == 0;


        public static SolidColorBrush Red => _red.Value;

        public static SolidColorBrush White { get; } = new SolidColorBrush(Color.White);

        public Boolean Equals(IBrush other)
        {
            return other is SolidColorBrush scb && Equals(scb);
        }

        public override Boolean Equals(Object obj)
        {
            if (obj is SolidColorBrush b)
                return Equals(b);

            return false;
        }

        public static SolidColorBrush FromRgb(Byte r, Byte g, Byte b)
        {
            return new SolidColorBrush(new Color(r, g, b));
        }

        public override Int32 GetHashCode()
        {
            return Color != null ? Color.GetHashCode() : 0;
        }

        public override String ToString()
        {
            return "Brush: " + Color;
        }

        private static readonly Lazy<SolidColorBrush> _darkGray = new Lazy<SolidColorBrush>(()
            => new SolidColorBrush(Color.DarkGray));

        private static readonly Lazy<SolidColorBrush> _lightGray = new Lazy<SolidColorBrush>(()
            => new SolidColorBrush(Color.LightGray));

        private static readonly Lazy<SolidColorBrush> _red = new Lazy<SolidColorBrush>(()
            => new SolidColorBrush(Color.Red));
    }
}