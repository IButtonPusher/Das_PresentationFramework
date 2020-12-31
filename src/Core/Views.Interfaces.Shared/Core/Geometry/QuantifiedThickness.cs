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

        public override String ToString()
        {
            return IsEmpty ? "0" : $"{Left} {Top} {Right} {Bottom}";
        }

        public static QuantifiedThickness Parse(String value)
        {
            var tokens = value.Split();

            QuantifiedDouble top, bottom, left, right, leftRight, topBottom;

            switch (tokens.Length)
            {
                case 1:
                    var all = QuantifiedDouble.Parse(tokens[0]);
                    return new QuantifiedThickness(all);

                case 2:
                    leftRight = QuantifiedDouble.Parse(tokens[1]);
                    topBottom = QuantifiedDouble.Parse(tokens[0]);
                    return new QuantifiedThickness(leftRight, topBottom);

                case 4:

                    //top = QuantifiedDouble.Parse(tokens[1]);
                    //right = QuantifiedDouble.Parse(tokens[2]);
                    //bottom = QuantifiedDouble.Parse(tokens[3]);
                    //left = QuantifiedDouble.Parse(tokens[0]);

                    top = QuantifiedDouble.Parse(tokens[0]);
                    right = QuantifiedDouble.Parse(tokens[1]);
                    bottom = QuantifiedDouble.Parse(tokens[2]);
                    left = QuantifiedDouble.Parse(tokens[3]);
                    return new QuantifiedThickness(left, top, right, bottom);

                case 3:
                    top = QuantifiedDouble.Parse(tokens[0]);
                    right = QuantifiedDouble.Parse(tokens[1]);
                    left = right;
                    bottom = QuantifiedDouble.Parse(tokens[2]);
                    return new QuantifiedThickness(left, top, right, bottom);
            }

            throw new InvalidOperationException();
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