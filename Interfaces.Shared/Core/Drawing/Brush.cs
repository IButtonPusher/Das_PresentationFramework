using System;

namespace Das.Views.Core.Drawing
{
    public class Brush : IEquatable<Brush>, IBrush
    {
        public Brush(Color color)
        {
            Color = color;
        }

        public static Brush FromRgb(Byte r, Byte g, Byte b) => new Brush(new Color(r, g, b));

        public static Brush White { get; } = new Brush(Color.White);

        public static Brush Black { get; } = new Brush(Color.Black);

        public static Brush DarkGray => _darkGray.Value;

        private static readonly Lazy<Brush> _darkGray = new Lazy<Brush>(()
            => new Brush(Color.DarkGray));


        public static Brush Red => _red.Value;

        private static readonly Lazy<Brush> _red = new Lazy<Brush>(()
            => new Brush(Color.Red));

        public Color Color { get; }

        IColor IBrush.Color => Color;

        public virtual Boolean IsInvisible => Color.A == 0;

        public override String ToString() => "Brush: " + Color;

        public Boolean Equals(Brush? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Color, other.Color);
        }

        public override Boolean Equals(Object obj)
        {
            if (obj is Brush b)
                return Equals(b);

            return false;
        }

        public override Int32 GetHashCode() => (Color != null ? Color.GetHashCode() : 0);
    }
}