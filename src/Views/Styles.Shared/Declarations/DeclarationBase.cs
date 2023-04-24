using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Styles.Functions;

namespace Das.Views.Styles.Declarations;

public abstract class DeclarationBase :  IStyleDeclaration
{
   protected DeclarationBase(//IStyleVariableAccessor variableAccessor, 
                             DeclarationProperty property)
   {
      //_variableAccessor = variableAccessor;
      Property = property;
   }

   protected static TEnum GetEnumValue<TEnum>(String name,
                                              TEnum defaultValue)
      where TEnum : struct
   {
      return GetEnumValue(name, defaultValue, true);
   }
        
   protected static TEnum GetEnumValue<TEnum>(String name,
                                              TEnum defaultValue,
                                              Boolean isThrowifInvalid)
      where TEnum : struct
   {
      if (name.Length == 0)
         return defaultValue;
            
      name = name.IndexOf('-') > 0
         ? name.Replace("-", "")
         : name;

      if (Enum.TryParse<TEnum>(name, true, out var value))
         return value;

      if (!isThrowifInvalid)
         return defaultValue;

      throw new InvalidOperationException();
   }

   protected static QuantifiedDouble GetQuantity(String token,
                                                 IStyleVariableAccessor variableAccessor)
   {
      return TryGetQuantity(token, variableAccessor, out var res)
         ? res
         : throw new InvalidCastException();
   }

   protected static Boolean TryGetQuantity(String token,
                                           IStyleVariableAccessor variableAccessor,
                                           out QuantifiedDouble quantity)
   {
      if (token.IndexOf("var", StringComparison.OrdinalIgnoreCase) == 0)
      {
         token = FunctionBuilder.GetValue<String>(token, variableAccessor);
      }

      return QuantifiedDouble.TryParse(token, out quantity);
   }

   protected static Boolean TryGetColor(String token,
                                        IStyleVariableAccessor variableAccessor,
                                        out IBrush brush)
   {
      //if (token.IndexOf("var", StringComparison.OrdinalIgnoreCase) == 0)
      {
         if (FunctionBuilder.GetValue(token, variableAccessor) is IBrush b)
         {
            brush = b;
            return true;
         }
      }
      brush = default!;
      return false;

      //return QuantifiedDouble.TryParse(token, out quantity);
   }

   /// <summary>
   /// Delimits by the specified character but keeps function calls (with parameters if applicable) as a single item
   /// </summary>
   protected static IEnumerable<String> GetMultiSplit(String value,
                                                      Char delimiter)
   {
      var sb = new StringBuilder();

      var fnCount = 0;

      for (var c = 0; c < value.Length; c++)
      {
         var currentChar = value[c];

         switch (currentChar)
         {
            case '(':
               fnCount++;
               goto default;

            case ')':
               fnCount--;
               goto default;

            default:
               if (currentChar == delimiter && fnCount == 0)
               {
                  yield return sb.ToString().Trim();
                  sb.Clear();
               }
               else
                  sb.Append(currentChar);
               break;
         }

      }

      if (sb.Length > 0)
         yield return sb.ToString().Trim();

   }

   //protected readonly IStyleVariableAccessor _variableAccessor;

   public DeclarationProperty Property { get; }

   public virtual Boolean Equals(IStyleDeclaration other)
   {
      return other.Property == Property;
   }
}