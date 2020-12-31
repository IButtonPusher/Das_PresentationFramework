using System;

namespace Das.Views.Transforms
{
    public class TranslateTransform : ITransform
    {
        public QuantifiedDouble X { get; }

        public QuantifiedDouble Y { get; }

        public TranslateTransform(QuantifiedDouble x)
        : this(x, QuantifiedDouble.Zero)
        { }

        public TranslateTransform(QuantifiedDouble x,
                                  QuantifiedDouble y)
        {
            X = x;
            Y = y;

            IsIdentity = X.IsZero() && Y.IsZero();
        }

        public Boolean IsIdentity { get; }
    }
}
