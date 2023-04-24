using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Das.Views.Styles.Declarations;

public abstract class MultiValueDeclaration<T> : DeclarationBase,
                                                 IStyleMultiValueDeclaration<T>
{
   protected MultiValueDeclaration(IEnumerable<T> values,
                                   //IStyleVariableAccessor variableAccessor,
                                   DeclarationProperty property)
      : base(/*variableAccessor, */property)
   {
      _values = new List<T>(values);
   }

   IEnumerable<T> IStyleMultiValueDeclaration<T>.Values
   {
      get
      {
         foreach (var v in _values)
            yield return v;
      }
   }

   public IEnumerable<T> Values => _values;

   ///// <summary>
   /////     Delimits by comma but keeps function calls (with parameters if applicable) as a single item
   ///// </summary>
   //protected static IEnumerable<String> GetMultiSplit(String value)
   //{
   //    var sb = new StringBuilder();

   //    var fnCount = 0;

   //    for (var c = 0; c < value.Length; c++)
   //    {
   //        var currentChar = value[c];

   //        switch (currentChar)
   //        {
   //            case '(':
   //                fnCount++;
   //                goto default;

   //            case ')':
   //                fnCount--;
   //                goto default;

   //            case ',' when fnCount == 0:
   //                yield return sb.ToString().Trim();
   //                sb.Clear();

   //                break;

   //            default:
   //                sb.Append(currentChar);
   //                break;
   //        }
   //    }

   //    if (sb.Length > 0)
   //        yield return sb.ToString().Trim();
   //}

   private readonly List<T> _values;
}