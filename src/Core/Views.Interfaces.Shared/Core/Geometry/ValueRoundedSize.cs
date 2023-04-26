using System;

namespace Das.Views.Core.Geometry;

public readonly struct ValueRoundedSize : IRoundedSize
{
   public ValueRoundedSize(Int32 width,
                           Int32 height)
   {
      Width = width;
      Height = height;
   }

   public Boolean Equals(IRoundedSize other)
   {
      if (ReferenceEquals(null, other)) return false;

      return Width == other.Width &&
             Height == other.Height;
   }

   public Int32 Height { get; }

   public Int32 Width { get; }
}