using System;
using System.Threading.Tasks;

namespace Das.Views.Transforms
{
    public class TranslateTransform : ITransform
    {
        public TranslateTransform(QuantifiedDouble x)
            : this(x, QuantifiedDouble.Zero)
        {
        }

        public TranslateTransform(QuantifiedDouble x,
                                  QuantifiedDouble y)
        {
            X = x;
            Y = y;

            IsIdentity = X.IsZero() && Y.IsZero();

            Value = IsIdentity
                ? TransformationMatrix.Identity
                : new TransformationMatrix(1, 0, 0, 1, x, y);
        }

        public Boolean IsIdentity { get; }

        public TransformationMatrix Value { get; }

        public QuantifiedDouble X { get; }

        public QuantifiedDouble Y { get; }

        public override String ToString()
        {
            return "Translate - X: " + X + " Y: " + Y;
        }
    }
}