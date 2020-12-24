using System;

namespace Das.Views.Extended
{
    public struct Face
    {
        public Face(Int32 a, 
                    Int32 b, 
                    Int32 c)
        {
            A = a;
            B = b;
            C = c;
        }

        public Int32 A;
        public Int32 B;
        public Int32 C;

        public override String ToString()
        {
            return $"{A},{B},{C}";
        }
    }
}
