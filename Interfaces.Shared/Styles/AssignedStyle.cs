using System;

namespace Das.Views.Styles
{
    public readonly struct AssignedStyle : IEquatable<AssignedStyle>
    {
        public AssignedStyle(StyleSetter setter, 
                             StyleSelector selector, 
                             Object? value = null)
        {
            Setter = setter;
            Selector = selector;
            Value = value;

            _hash = (Int32) setter + ((Int32) selector << 16);
        }

        public readonly StyleSetter Setter;
        public readonly StyleSelector Selector;
        public readonly Object? Value;

        private readonly Int32 _hash;

        public Boolean Equals(AssignedStyle other)
        {
            return other.Setter == Setter && other.Selector == Selector;
        }

        public override Boolean Equals(Object obj)
        {
            return obj is AssignedStyle assigned && Equals(assigned);
        }

        public override Int32 GetHashCode()
        {
            return _hash;
        }

        public override String ToString()
        {
            if (Selector == StyleSelector.None)
                return Setter + " = " + Value;

            return Setter + ":" + Selector + " = " + Value;
        }
    }
}
