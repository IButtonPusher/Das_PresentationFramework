using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Geometry
{
    public readonly struct QuantifiedThickness
    {
        public QuantifiedThickness(QuantifiedDouble all)
            : this(all, all, all, all)
        {

        }

        public QuantifiedThickness(QuantifiedDouble leftRight,
                                   QuantifiedDouble topBottom)
            : this(leftRight, topBottom, leftRight, topBottom)
        {

        }

        public QuantifiedThickness(QuantifiedDouble left,
                                   QuantifiedDouble top,
                                   QuantifiedDouble right,
                                   QuantifiedDouble bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;

            IsEmpty = left.IsZero() &&
                       right.IsZero() &&
                       top.IsZero() &&
                       bottom.IsZero();
        }

        public static readonly QuantifiedThickness Empty = new QuantifiedThickness(0, 0, 0, 0);

        private readonly QuantifiedDouble Left;
        private readonly QuantifiedDouble Top;
        private readonly QuantifiedDouble Right;
        private readonly QuantifiedDouble Bottom;

        public Boolean IsEmpty { get; }


        public ValueThickness GetValue<TSize>(TSize size)
            where TSize : ISize
        {
            return new ValueThickness(
                Left.GetQuantity(size.Width),
                Top.GetQuantity(size.Height),
                Right.GetQuantity(size.Width),
                Bottom.GetQuantity(size.Height));
        }
    }
}