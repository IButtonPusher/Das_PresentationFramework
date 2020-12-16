using System;

namespace Das.Views.Core.Geometry
{
    public readonly struct ValueMinMax
    {
        public ValueMinMax(Int32 min, 
                           Int32 max)
        {
            Min = min;
            Max = max;
            IsEmpty = max == 0 && min == 0;
            Count = IsEmpty ? 0 : max - min + 1;
        }

        public static Boolean operator <(ValueMinMax left, ValueMinMax right)
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

        public static Boolean operator >(ValueMinMax left, ValueMinMax right)
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

        public static readonly ValueMinMax Empty = new ValueMinMax(0, 0);

        public override String ToString()
        {
            return "Min: " + Min + " Max: " + Max;
        }
    }
}
