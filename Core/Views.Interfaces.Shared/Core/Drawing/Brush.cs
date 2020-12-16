using System;

namespace Das.Views.Core.Drawing
{
    public class SolidColorBrush : IEquatable<SolidColorBrush>, IBrush
    {
        public SolidColorBrush(Color color)
        {
            Color = color;
        }

        public static SolidColorBrush FromRgb(Byte r, Byte g, Byte b) => new SolidColorBrush(new Color(r, g, b));

        public static SolidColorBrush White { get; } = new SolidColorBrush(Color.White);

        public static SolidColorBrush Black { get; } = new SolidColorBrush(Color.Black);

        public static SolidColorBrush DarkGray => _darkGray.Value;

        private static readonly Lazy<SolidColorBrush> _darkGray = new Lazy<SolidColorBrush>(()
            => new SolidColorBrush(Color.DarkGray));


        public static SolidColorBrush Red => _red.Value;

        private static readonly Lazy<SolidColorBrush> _red = new Lazy<SolidColorBrush>(()
            => new SolidColorBrush(Color.Red));

        public Color Color { get; }

        IColor IBrush.Color => Color;

        public virtual Boolean IsInvisible => Color.A == 0;

        public override String ToString() => "Brush: " + Color;

        public Boolean Equals(SolidColorBrush? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Color, other.Color);
        }

        public override Boolean Equals(Object obj)
        {
            if (obj is SolidColorBrush b)
                return Equals(b);

            return false;
        }

        public override Int32 GetHashCode() => (Color != null ? Color.GetHashCode() : 0);
    }
}