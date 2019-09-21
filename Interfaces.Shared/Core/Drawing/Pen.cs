using System;

namespace Das.Views.Core.Drawing
{
    public class Pen : IPen, IEquatable<IPen>
    {
        private readonly Int32 _hash;

        public Pen(Color color, Int32 thickness)
        {
            Color = color;
            Thickness = thickness;

            unchecked
            {
                _hash = 17;
                _hash = _hash * 31 + Color.GetHashCode();
                _hash = _hash * 31 + thickness;
            }
        }

        public Color Color { get; }

        IColor IPen.Color => Color;
        public Int32 Thickness { get; }

        public override Int32 GetHashCode() => _hash;


        public override Boolean Equals(Object obj)
        {
            if (obj is IPen pen)
                return Equals(pen);
            return false;
        }

        public Boolean Equals(IPen other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Equals(Color, other.Color) && Thickness == other.Thickness;
        }
    }
}