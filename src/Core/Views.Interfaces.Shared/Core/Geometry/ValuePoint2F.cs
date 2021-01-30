using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Geometry
{
    public readonly struct ValuePoint2F : IPoint2F
    {
        public ValuePoint2F(Single x,
                            Single y)
        {
            X = x;
            Y = y;
        }

        public Boolean Equals(IPoint2F other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y);
        }

        public Single X { get; }

        public Single Y { get; }

        public static readonly ValuePoint2F Empty = new(0, 0);
    }
}
