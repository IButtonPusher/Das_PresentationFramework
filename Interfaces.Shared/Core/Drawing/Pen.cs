using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Drawing
{
    public class Pen : IPen, 
                       IEquatable<IPen>
    {
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

        public Boolean Equals(IPen other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Equals(Color, other.Color) && Thickness == other.Thickness;
        }

        IColor IPen.Color => Color;

        public Int32 Thickness { get; }

        public Color Color { get; }


        public override Boolean Equals(Object obj)
        {
            if (obj is IPen pen)
                return Equals(pen);
            return false;
        }

        public override Int32 GetHashCode()
        {
            return _hash;
        }

        private readonly Int32 _hash;
    }
}