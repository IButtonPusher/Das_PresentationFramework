using System;
using System.Threading.Tasks;
using Das.Extensions;

namespace Das.Views.Core.Geometry
{
    public readonly struct DoubleValueMinMax : IEquatable<DoubleValueMinMax>,
                                               IMinMax<Double>
    {
        public DoubleValueMinMax(Double min,
                                 Double max)
        {
            Min = min;
            Max = max;
            IsEmpty = max == 0 && min == 0;
            Count = IsEmpty ? 0 : max - min + 1;

            _hash = min.GetHashCode() + (max.GetHashCode() << 16);
        }

        public static Boolean operator <(DoubleValueMinMax left,
                                         DoubleValueMinMax right)
        {
            if (left.Min < right.Min)
            {
                return left.Max <= right.Max;
            }

            if (left.Min.AreEqualEnough(right.Min))
                return left.Max < right.Max;

            return false;
        }

        public Double GetValueInRange(Double proposed)
        {
            if (proposed <= Min)
                return Min;

            if (proposed >= Max)
                return Max;

            return proposed;
        }

        public static Boolean operator >(DoubleValueMinMax left,
                                         DoubleValueMinMax right)
        {
            if (left.Min > right.Min)
            {
                return left.Max >= right.Max;
            }

            if (left.Min.AreEqualEnough(right.Min))
                return left.Max > right.Max;

            return false;
        }


        public readonly Boolean IsEmpty;

        public readonly Double Min;
        public readonly Double Max;
        public readonly Double Count;

        private readonly Int32 _hash;

        public static readonly DoubleValueMinMax Empty = new DoubleValueMinMax(0, 0);

        Double IMinMax<Double>.Max => Max;

        Double IMinMax<Double>.Min => Min;

        Boolean IMinMax<Double>.IsEmpty => IsEmpty;

        IMinMax<Double> IMinMax<Double>.Empty => Empty;

        public Boolean Overlaps(IMinMax<Double> mm)
        {
            return this.DoOverlaps(mm);
        }

        public Boolean Contains(Double item)
        {
            return this.DoContains(item);
        }

        public Boolean Contains(IMinMax<Double> item)
        {
            return this.DoContains(item);
        }

        public IMinMax<Double> Minus(IMinMax<Double> item)
        {
            return this.DoMinus(item, (l,u) => new DoubleValueMinMax(l,u));
        }

        //public IMinMax<IConvertible> ToConvertible()
        //{
        //    return new MinMaxCore(Min, Max);
        //}

        public Boolean Equals(DoubleValueMinMax other)
        {
            return other.Min.AreEqualEnough(Min) && 
                   other.Max.AreEqualEnough(Max);
        }

        public override Int32 GetHashCode()
        {
            return _hash;
        }

        public override Boolean Equals(Object obj)
        {
            return obj is DoubleValueMinMax vmm && Equals(vmm);
        }

        public Boolean Equals(IMinMax<Double> other)
        {
            return other.Min.AreEqualEnough(Min) && 
                   other.Max.AreEqualEnough(Max);
        }

        public override String ToString()
        {
            return "Min: " + Min + " Max: " + Max;
        }
    }
}
