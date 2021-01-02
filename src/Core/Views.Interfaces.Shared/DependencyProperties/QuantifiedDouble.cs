﻿using System;
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
            //quantifiedDouble = Zero;
            //return false;

            //var units = unitStr == "%" 
            //    ? LengthUnits.Percent 
            //    : ExtensionMethods.GetEnumValue(unitStr, LengthUnits.None, false);

            if (!Double.TryParse(value.Substring(0, endOfValue + 1), out var val))
            {
                quantifiedDouble = default;
                return false;
            }
            //var val = Double.Parse(value.Substring(0, endOfValue + 1));

            quantifiedDouble = new QuantifiedDouble(val, units);
            return true;
        }

        public static implicit operator QuantifiedDouble(Double value)
        {
            return new QuantifiedDouble(value, LengthUnits.Px);
        }

        public static implicit operator Double(QuantifiedDouble @double)
        {
            return @double.Quantity;
        }

        private readonly Double Quantity;
        public readonly LengthUnits Units;
    }
}