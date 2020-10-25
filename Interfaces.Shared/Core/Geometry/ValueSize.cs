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

        public ISize Reduce(Thickness padding)
        {
            return GeometryHelper.Reduce(this, padding);
        }

        public Double Height { get; }

        public Boolean IsEmpty => Width.IsZero() || Height.IsZero();

        public ISize DeepCopy()
        {
            return new ValueSize(Width, Height);
        }

        public override String ToString()
        {
            return "Width: " + Width + " Height: " + Height;
        }
    }
}