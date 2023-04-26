using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Das.Views.Core.Geometry;

// ReSharper disable once UnusedType.Global
public class RoundedSize : IRoundedSize
{
   public Int32 Width { get; set; }

   public Int32 Height { get; set; }

   public Boolean Equals(IRoundedSize other)
   {
      if (ReferenceEquals(null, other)) 
         return false;

      if (ReferenceEquals(this, other)) 
         return true;

      return Width == other.Width &&
             Height == other.Height;
   }

   public override Boolean Equals(Object obj)
   {
      return obj is IRoundedSize isize && Equals(isize);
   }

   [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
   public override Int32 GetHashCode()
   {
      unchecked
      {
         return (Width * 397) ^ Height;
      }
   }
}