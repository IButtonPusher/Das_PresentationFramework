using System;
using System.Collections.Generic;

namespace Das.Views.Styles.Declarations
{
    public class QuantifiedNumericDeclaration<TQuantity> : DeclarationBase
        where TQuantity : struct
    {
        public QuantifiedNumericDeclaration(String value,
                                            Dictionary<String, TQuantity> quantitySearch,
                                            TQuantity defaultValue,
                                            IStyleVariableAccessor variableAccessor,
                                            DeclarationProperty property)
            : base(variableAccessor, property)
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
                return;

            var unitStr = value.Substring(endOfValue + 1);

            if (quantitySearch.TryGetValue(unitStr, out var enumVal))
                Units = enumVal;
            else
                Units = GetEnumValue(unitStr, defaultValue);

            Value = Double.Parse(value.Substring(0, endOfValue + 1));
        }

        protected static Boolean IsValidQuantity(String value,
                                                 TQuantity invalidValue,
                                                 Dictionary<String, TQuantity> quantitySearch)
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
                return false;

            var unitStr = value.Substring(endOfValue + 1);
            if (!quantitySearch.TryGetValue(unitStr, out _))
            {
                if (Equals(GetEnumValue(unitStr, invalidValue), invalidValue))
                    return false;
            }

            return Double.TryParse(value.Substring(0, endOfValue + 1), out _);
        }

        public override String ToString()
        {
            return Property + ": " + Value + " " + Units;
        }

        public Double Value { get; }

        public TQuantity Units { get; }

    }
}
