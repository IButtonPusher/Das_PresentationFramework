using System;

namespace Das.Views.Extended
{
    public readonly struct ValueVector3
    {
        public readonly Double X;
        public readonly Double Y;

        public readonly Double Z;

        public ValueVector3(Double x, Double y, Double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
