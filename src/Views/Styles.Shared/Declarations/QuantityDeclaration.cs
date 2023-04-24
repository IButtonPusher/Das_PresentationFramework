using System;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Das.Views.Styles.Declarations;

public class QuantityDeclaration : ValueDeclaration<QuantifiedDouble>
{
   public QuantityDeclaration(String value,
                              //IStyleVariableAccessor variableAccessor,
                              DeclarationProperty property)
      : base(QuantifiedDouble.Parse(value),  /*variableAccessor, */property)
   {
           
   }

   public static Boolean IsValidQuantity(String value)
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

      if (unitStr.Length > 0)
      {
         var units = unitStr == "%"
            ? LengthUnits.Percent
            : GetEnumValue(unitStr, LengthUnits.Invalid);
         if (units == LengthUnits.Invalid) 
            return false;
      }

      return Double.TryParse(value.Substring(0, endOfValue + 1), out _);
   }

   public LengthUnits Units => Value.Units;
}