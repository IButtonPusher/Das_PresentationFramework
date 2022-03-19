using System;
using System.Threading.Tasks;
using Das.Extensions;

namespace Das.Views.Core.Geometry
{
   public class Size : GeometryBase, IDeepCopyable<Size>,
                       ISize
   {
      protected Size()
      {
      }

      public Size(Double width,
                  Double height)
      {
         _width = width;
         _height = height;
         UpdateHasInfinite();
      }

      public Size DeepCopy()
      {
         return new Size(Width, Height);
      }

      public virtual Double Width
      {
          get => _width;
          set
          {
              if (Equals(_width, value))
                  return;

              _width = value;
              UpdateHasInfinite();
          }
      }

      private void UpdateHasInfinite()
      {
          HasInfiniteDimension = Double.IsInfinity(_width) ||
                                 Double.IsInfinity(_height);
      }

      public Boolean HasInfiniteDimension { get; private set; }

      public virtual Double Height
      {
          get => _height;
          set
          {
              if (Equals(_height, value))
                  return;
              
              _height = value;
              UpdateHasInfinite();
          }
      }

      public Boolean IsEmpty => Width.IsZero() && Height.IsZero();

      public Boolean Equals(ISize? other)
      {
         return GeometryHelper.AreSizesEqual(this, other);
      }

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
      public static Size Add(ISize size1,
                             ISize size2)
      {
         if (size1 == null || size2 == null)
            throw new InvalidOperationException();

         return new Size(size1.Width + size2.Width,
            size1.Height + size2.Height);
      }

      public ISize PlusVertical(ISize adding)
      {
         return GeometryHelper.PlusVertical(this, adding);
      }

      public override Boolean Equals(Object obj)
      {
         return obj is ISize isize && Equals(isize);
      }

      public override Int32 GetHashCode()
      {
         unchecked
         {
            return (Width.GetHashCode() * 397) ^ Height.GetHashCode();
         }
      }

      public static Size operator +(Size size,
                                    Thickness margin)
      {
         if (margin == null)
            return size.DeepCopy();

         return new Size(size.Width + margin.Left + margin.Right,
            size.Height + margin.Top + margin.Bottom);
      }

      public static implicit operator ValueSize(Size size)
      {
         return new ValueSize(size.Width, size.Height);
      }

      public static Size operator *(Size size,
                                    Double val)
      {
         if (val.AreEqualEnough(1))
            return size;

         if (size == null)
            return null!;

         return new Size(size.Width * val, size.Height * val);
      }

      public static Size operator -(Size size,
                                    Thickness margin)
      {
         if (margin == null)
            return size.DeepCopy();

         return new Size(size.Width - (margin.Left + margin.Right),
            size.Height - (margin.Top + margin.Bottom));
      }

      public static Size Subtract(ISize size,
                                  ISize size2)
      {
         if (size == null || size2 == null)
            throw new InvalidOperationException();

         return new Size(size.Width - size2.Width,
            size.Height - size2.Height);
      }

      public override String ToString()
      {
         return "Width: " + Width + " Height: " + Height;
      }

      public static implicit operator Size(ValueSize value)
      {
         return new Size(value.Width, value.Height);
      }

      public static Size Empty { get; } = new Size(0, 0);

      private Double _height;

      private Double _width;
   }
}
