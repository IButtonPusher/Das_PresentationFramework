using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Geometry
{
    public readonly struct QuantifiedThickness : IBoxValue<QuantifiedDouble>
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
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;

            IsEmpty = left.IsZero() &&
                       right.IsZero() &&
                       top.IsZero() &&
                       bottom.IsZero();
        }

        public override String ToString()
        {
            return IsEmpty ? "0" : $"{_left} {_top} {_right} {_bottom}";
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

        public QuantifiedThickness Transition(QuantifiedThickness target,
                                              Double percentComplete)
        {
            if (percentComplete >= 1.0)
                return target;

            return new QuantifiedThickness(Left.Transition(target.Left, percentComplete),
                Top.Transition(target.Top, percentComplete),
                Right.Transition(target.Right, percentComplete),
                Bottom.Transition(target.Bottom, percentComplete));

            //return new QuantifiedThickness((target.Left - Left) * percentComplete,
            //    (target.Top - Top) * percentComplete,
            //    (target.Right - Right) * percentComplete,
            //    (target.Bottom - Bottom) * percentComplete);
        }

        public static readonly QuantifiedThickness Empty = new QuantifiedThickness(0, 0, 0, 0);

        public QuantifiedDouble Left => _left;

        public QuantifiedDouble Right => _right;

        public QuantifiedDouble Top => _top;

        public QuantifiedDouble Bottom => _bottom;

        private readonly QuantifiedDouble _left;
        private readonly QuantifiedDouble _top;
        private readonly QuantifiedDouble _right;
        private readonly QuantifiedDouble _bottom;

        public Boolean IsEmpty { get; }


        public ValueThickness GetValue<TSize>(TSize size)
            where TSize : ISize
        {
            return new ValueThickness(
                _left.GetQuantity(size.Width),
                _top.GetQuantity(size.Height),
                _right.GetQuantity(size.Width),
                _bottom.GetQuantity(size.Height));
        }
    }
}