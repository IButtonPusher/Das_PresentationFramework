using System;
using System.Threading.Tasks;
using Das.Extensions;


namespace Das.Views.Core.Geometry
{
    public readonly struct ValueSize : ISize
    {
        public ValueSize(Double width,
                         Double height)
        {
            Width = width;
            Height = height;

            IsEmpty = Width.IsZero() && Height.IsZero();
            IsValid = Width.IsNotZero() && Height.IsNotZero() &&
                      !Double.IsInfinity(Width) && !Double.IsInfinity(Height);
        }

        public ValueSize(ISize size) 
            : this(size.Width, size.Height)
        {
            
        }


        //ISize ISize.Divide(Double pct)
        //{
        //    return GeometryHelper.Divide(this, pct);
        //}

        public Boolean Equals(ISize? other)
        {
            if (ReferenceEquals(null, other))
                return false;

            return Width.AreEqualEnough(other.Width) &&
                   Height.AreEqualEnough(other.Height);
        }

        public Double Width { get; }

        //public ISize Reduce(Thickness padding)
        //{
        //    return GeometryHelper.Reduce(this, padding);
        //}

        //public ISize Minus(ISize subtract)
        //{
        //    return GeometryHelper.Minus(this, subtract);
        //}

        //public ValueSize PlusVertical(ISize adding)
        //{
        //    return GeometryHelper.PlusVertical(this, adding);
        //}

        /// <summary>
        /// Returns a ValueSize with the minimum value of width and height between this
        /// object and the provided parameter value
        /// </summary>
        public ValueSize LeastCommonDenominator(ISize other)
        {
            if (IsEmpty || other.IsEmpty)
                return Empty;

            return new ValueSize(Math.Min(Width, other.Width),
                Math.Min(Height, other.Height));
        }

        //ISize ISize.PlusVertical(ISize adding)
        //{
        //    return GeometryHelper.PlusVertical(this, adding);
        //}

        public static ValueSize operator +(ValueSize size, 
                                           Thickness margin)
        {
            if (margin.IsEmpty)
                return size;

            return new ValueSize(size.Width + margin.Left + margin.Right,
                size.Height + margin.Top + margin.Bottom);
        }

        public Double Height { get; }

        public Boolean IsEmpty { get; }
        
        /// <summary>
        /// A ValueSize is valid if the height and width are greater than 0 and neither are positive
        /// or negative infinity
        /// </summary>
        public Boolean IsValid { get; }

        public ISize DeepCopy()
        {
            return new ValueSize(Width, Height);
        }

        public Double CenterY(ISize item)
        {
            return GeometryHelper.CenterY(this, item);
        }

        public Double CenterX(ISize item)
        {
            return GeometryHelper.CenterX(this, item);
        }

        public static readonly ValueSize Empty = new ValueSize(0, 0);

        public override String ToString()
        {
            return "Width: " + Width + " Height: " + Height;
        }
    }
}