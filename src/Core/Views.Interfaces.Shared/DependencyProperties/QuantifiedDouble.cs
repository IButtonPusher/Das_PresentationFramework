using System;
using System.Threading.Tasks;
using Das.Extensions;

namespace Das.Views
{
    public readonly struct QuantifiedDouble
    {
        public QuantifiedDouble(Double quantity,
                                LengthUnits units)
        {
            Quantity = quantity;
            Units = units;

            IsUnitsEffectivelyPixels = units is LengthUnits.Px or LengthUnits.None;
        }

        public Boolean IsNotZero()
        {
            return Quantity.IsNotZero();
        }

        public Boolean IsZero()
        {
            return Quantity.IsZero();
        }

        public static readonly QuantifiedDouble Zero = new QuantifiedDouble(0, LengthUnits.None);

        public Double GetQuantity(Double available)
        {
            switch (Units)
            {
                case LengthUnits.Percent:
                    return available * (Quantity / 100);

                case LengthUnits.Px:
                case LengthUnits.None:
                    return Quantity;

                default:
                    throw new NotImplementedException();
            }
        }

        public QuantifiedDouble Transition(QuantifiedDouble target,
                                           Double percentComplete)
        {
            return new QuantifiedDouble(Quantity + (target.Quantity - Quantity) * percentComplete, Units);
        }

        public override String ToString()
        {
            return Quantity + " " + Units;
        }

        public static QuantifiedDouble Parse(String value)
        {
            if (TryParse(value, out var quantifiedValue))
                return quantifiedValue;

            throw new InvalidCastException();
        }

        public static Boolean TryParse(String value,
                                       out QuantifiedDouble quantifiedDouble)
        {
            var endOfValue = -1;

            for (var c = value.Length - 1; c >= 0; c--)
            {
                if (!Char.IsDigit(value[c]))
                    continue;

                endOfValue = c;
                break;
            }

            if (endOfValue == -1)
            {
                quantifiedDouble = default!;
                return false;
            }

            var unitStr = value.Substring(endOfValue + 1);

            LengthUnits units;
            if (unitStr == "%")
                units = LengthUnits.Percent;
            else if (ExtensionMethods.TryGetEnumValue<LengthUnits>(unitStr, out var u))
                units = u;
            else
                units = LengthUnits.None;
            

            if (!Double.TryParse(value.Substring(0, endOfValue + 1), out var val))
            {
                quantifiedDouble = default;
                return false;
            }

            quantifiedDouble = new QuantifiedDouble(val, units);
            return true;
        }

        public static QuantifiedDouble operator *(QuantifiedDouble value,
                                                  Double mult)
        {
            return new QuantifiedDouble(value.Quantity * mult, value.Units);
        }

        public static QuantifiedDouble operator +(QuantifiedDouble value,
                                                  QuantifiedDouble addMe)
        {
            return new QuantifiedDouble(value.Quantity + addMe.Quantity, value.Units);
        }

        public static QuantifiedDouble operator -(QuantifiedDouble value,
                                                  QuantifiedDouble addMe)
        {
            return new QuantifiedDouble(value.Quantity - addMe.Quantity, value.Units);
        }

        public static implicit operator QuantifiedDouble(Double value)
        {
            return new (value, LengthUnits.Px);
        }

        public static implicit operator Double(QuantifiedDouble @double)
        {
            return @double.Quantity;
        }

        private readonly Double Quantity;
        public readonly LengthUnits Units;

        public readonly Boolean IsUnitsEffectivelyPixels;
    }
}