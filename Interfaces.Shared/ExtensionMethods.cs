using System;

namespace Das.Views
{
    public static class ExtensionMethods
    {
        public static Boolean AreEqualEnough(this Double d1, Double d2)
            => Math.Abs(d1 - d2) < 0.00001;

        public static Boolean AreEqualEnough(this Single f1,Single f2)
            => Math.Abs(f1 - f2) < 0.00001f;

        public static Boolean AreEqualEnough(this Int32 i1, Double d1)
            => Convert.ToInt32(d1) == i1;
    }
}