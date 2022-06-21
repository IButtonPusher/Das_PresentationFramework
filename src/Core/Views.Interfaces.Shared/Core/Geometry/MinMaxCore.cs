using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Das.Views.Core.Geometry
{
   public readonly struct MinMaxCore<T> : IMinMax<T>
        where T : IConvertible, IComparable
    {
        public MinMaxCore(T min,
                          T max)
        {
            Min = min;
            Max = max;

            _hash = min.GetHashCode() + (max.GetHashCode() << 16);

            IsEmpty = Min.CompareTo(default(T)) == 0 && Max.CompareTo(default(T)) == 0;
        }


        IMinMax<T> IMinMax<T>.Empty => Empty;

        public Boolean Overlaps(IMinMax<T> mm)
        {
            return this.DoOverlaps(mm);
        }

        public Boolean Contains(T item)
        {
            return this.DoContains(item);
        }

        public Boolean Contains(IMinMax<T> item)
        {
            return this.DoContains(item);
        }

        public IMinMax<T> Minus(IMinMax<T> item)
        {
            return this.DoMinus(item, (l,u) => new MinMaxCore<T>(l,u));
        }


        public static MinMaxCore<T> LeastCommonDenominator(IEnumerable<IMinMax<T>> items)
        {
            using (var itar = items.GetEnumerator())
            {
                IMinMax<T>? current;

                if (!itar.MoveNext() || (current = itar.Current) == null)
                    return Empty;

                var min = current.Min;
                var max = current.Max;

                while (itar.MoveNext() && (current = itar.Current) != null)
                {
                    if (current.Min.CompareTo(min) > 0)
                        min = current.Min;

                    if (current.Max.CompareTo(min) < 0)
                        max = current.Max;
                }

                if (min.CompareTo(max) > 0)
                    return Empty;

                return new MinMaxCore<T>(min, max);

            }
        }

        public Boolean IsEmpty { get; }

        public static MinMaxCore<T> Empty { get; } = new(default!, default!);

        public override Int32 GetHashCode()
        {
            return _hash;
        }

        public Boolean Equals(IMinMax<T> other)
        {
            return other.Min.Equals(Min) &&
                   other.Max.Equals(Max);
        }

        public override String ToString()
        {
            return "Min: " + Min + " Max: " + Max;
        }

        public T Max { get; }

        public T Min { get; }


        private readonly Int32 _hash;
    }

    
}
