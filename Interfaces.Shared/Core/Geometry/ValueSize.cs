using System;
using System.Threading.Tasks;
using Das.Extensions;

namespace Das.Views.Core.Geometry
{
    public readonly struct ValueSize : ISize
    {
        public ValueSize(Double width, Double height)
        {
            Width = width;
            Height = height;
        }

        public Boolean Equals(ISize? other)
        {
            if (ReferenceEquals(null, other))
                return false;

            return Width.AreEqualEnough(other.Width) &&
                   Height.AreEqualEnough(other.Height);
        }

        public Double Width { get; }

        public Double Height { get; }

        public Boolean IsEmpty => Width.IsZero() || Height.IsZero();

        public override String ToString()
        {
            return "Width: " + Width + " Height: " + Height;
        }
    }
}