using System;
using System.Threading.Tasks;

namespace Das.Views.Styles
{
    public readonly struct AssignedStyle : IEquatable<AssignedStyle>,
                                           IStyleSetter
    {
        public AssignedStyle(StyleSetterType setterType,
                             StyleSelector selector,
                             Object? value = null)
        {
            SetterType = setterType;
            Selector = selector;
            Value = value;

            _hash = (Int32) setterType + ((Int32) selector << 16);
        }

        public readonly StyleSetterType SetterType;
        public readonly StyleSelector Selector;
        public Object? Value { get; }

        private readonly Int32 _hash;

        public Boolean Equals(AssignedStyle other)
        {
            return other.SetterType == SetterType && other.Selector == Selector;
        }

        public Boolean Equals(IStyleSetter other)
        {
            return other is AssignedStyle assigned && Equals(assigned);
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
                return SetterType + " = " + Value;

            return SetterType + ":" + Selector + " = " + Value;
        }
    }
}