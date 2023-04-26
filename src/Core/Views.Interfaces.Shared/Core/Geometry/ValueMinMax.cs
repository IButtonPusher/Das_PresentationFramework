using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Geometry;

public readonly struct ValueMinMax : IEquatable<ValueMinMax>,
                                     IMinMax<Int32>
{
   public ValueMinMax(Int32 min,
                      Int32 max)
   {
      Min = min;
      Max = max;
      IsEmpty = max == 0 && min == 0;
      Count = IsEmpty ? 0 : max - min + 1;

      _hash = min + (max << 16);
   }

   public static Boolean operator <(ValueMinMax left,
                                    ValueMinMax right)
   {
      if (left.Min < right.Min)
      {
         return left.Max <= right.Max;
      }

      if (left.Min == right.Min)
         return left.Max < right.Max;

      return false;
   }

   public Int32 GetValueInRange(Int32 proposed)
   {
      if (proposed <= Min)
         return Min;

      if (proposed >= Max)
         return Max;

      return proposed;
   }

   public static Boolean operator >(ValueMinMax left,
                                    ValueMinMax right)
   {
      if (left.Min > right.Min)
      {
         return left.Max >= right.Max;
      }

      if (left.Min == right.Min)
         return left.Max > right.Max;

      return false;
   }


   public readonly Boolean IsEmpty;

   public readonly Int32 Min;
   public readonly Int32 Max;
   public readonly Int32 Count;

   private readonly Int32 _hash;

   public static readonly ValueMinMax Empty = new ValueMinMax(0, 0);

   Int32 IMinMax<Int32>.Max => Max;

   Int32 IMinMax<Int32>.Min => Min;

   Boolean IMinMax<Int32>.IsEmpty => IsEmpty;

   IMinMax<Int32> IMinMax<Int32>.Empty => Empty;

   public Boolean Overlaps(IMinMax<Int32> mm)
   {
      return this.DoOverlaps(mm);
   }

   public Boolean Contains(Int32 item)
   {
      return this.DoContains(item);
   }

   public Boolean Contains(IMinMax<Int32> item)
   {
      return this.DoContains(item);
   }

   public IMinMax<Int32> Minus(IMinMax<Int32> item)
   {
      return this.DoMinus(item, (l,
                                 u) => new ValueMinMax(l, u));
   }


   public Boolean Equals(ValueMinMax other)
   {
      return other.Min == Min && other.Max == Max;
   }

   public override Int32 GetHashCode()
   {
      return _hash;
   }

   public override Boolean Equals(Object obj)
   {
      return obj is ValueMinMax vmm && Equals(vmm);
   }

   public Boolean Equals(IMinMax<Int32> other)
   {
      return other.Min == Min && other.Max == Max;
   }

   public override String ToString()
   {
      return "Min: " + Min + " Max: " + Max;
   }
}