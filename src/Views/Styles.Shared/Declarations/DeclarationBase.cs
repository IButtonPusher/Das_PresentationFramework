using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Das.Views.Styles.Declarations
{
    public abstract class DeclarationBase :  IStyleDeclaration
    {
        protected DeclarationBase(IStyleVariableAccessor variableAccessor, 
                                  DeclarationProperty property)
        {
            _variableAccessor = variableAccessor;
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

        /// <summary>
        /// Delimits by comma but keeps function calls (with parameters if applicable) as a single item
        /// </summary>
        protected static IEnumerable<String> GetMultiSplit(String value)
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
                    
                    case ',' when fnCount == 0:
                        yield return sb.ToString().Trim();
                        sb.Clear();
                        
                        break;
                    
                    default:
                        sb.Append(currentChar);
                        break;
                }
                
            }

            if (sb.Length > 0)
                yield return sb.ToString().Trim();

        }

        protected readonly IStyleVariableAccessor _variableAccessor;

        public DeclarationProperty Property { get; }
    }
}