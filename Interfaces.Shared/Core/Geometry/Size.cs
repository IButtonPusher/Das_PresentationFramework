using System;
using Das.Extensions;

namespace Das.Views.Core.Geometry
{
    public class Size : GeometryBase, IDeepCopyable<Size>,
        ISize
    {
        public virtual Double Width
        {
            get => _width;
            set => _width = value;
        }

        public virtual Double Height
        {
            get => _height;
            set => _height = value;
        }

        public static Size Empty { get; } = new Size(0, 0);

        public virtual Boolean IsEmpty => Width.IsZero() || Height.IsZero();

        protected Size()
        {
        }

        public Size(Double width, Double height)
        {
            _width = width;
            _height = height;
        }

        private Double _width;
        private Double _height;

        public static Size Add(params ISize[] sizes)
        {
            var width = 0.0;
            var height = 0.0;

            for (var c = 0; c < sizes.Length; c++)
            {
                var current = sizes[c];
                if (current == null)
                    continue;

                width += current.Width;
                height += current.Height;
            }

            return new Size(width, height);
        }

        // ReSharper disable once UnusedMember.Global
        public static Size Add(ISize size1, ISize size2)
        {
            if (size1 == null || size2 == null)
                throw new InvalidOperationException();

            return new Size(size1.Width + size2.Width,
                size1.Height + size2.Height);
        }

        public static Size operator +(Size size, Thickness margin)
        {
            if (margin == null)
                return size.DeepCopy();

            return new Size(size.Width + margin.Left + margin.Right,
                size.Height + margin.Top + margin.Bottom);
        }

        public static Size Subtract(ISize size, ISize size2)
        {
            if (size == null || size2 == null)
                throw new InvalidOperationException();

            return new Size(size.Width - size2.Width,
                size.Height - size2.Width);
        }

        public static Size operator -(Size size, Thickness margin)
        {
            if (margin == null)
                return size.DeepCopy();

            return new Size(size.Width - (margin.Left + margin.Right),
                size.Height - (margin.Top + margin.Bottom));
        }

        public static Size operator *(Size size, Double val)
        {
            if (val.AreEqualEnough(1))
                return size;

            if (size == null)
                return null;

            return new Size(size.Width * val, size.Height * val);
        }

        public Size DeepCopy() => new Size(Width, Height);

        public Rectangle CenteredIn(Size outerRect)
        {
            var wDiff = (outerRect.Width - Width) / 2;
            var hDiff = (outerRect.Height - Height) / 2;
            return new Rectangle(wDiff, hDiff, Width, Height);
        }

        public override String ToString() => Width.ToString("0.00") + ", " +
                                             Height.ToString("0.00");

        public Boolean Equals(ISize other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Width.AreEqualEnough(other.Width) &&
                   Height.AreEqualEnough(other.Height);
        }

        public override Boolean Equals(Object obj) => (obj is ISize isize && Equals(isize));

        public override Int32 GetHashCode()
        {
            unchecked
            {
                return (Width.GetHashCode() * 397) ^ Height.GetHashCode();
            }
        }
    }
}