using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Das.Views.Styles.Functions
{
    public static class FunctionBuilder
    {
        public static IFunction GetFunction(String text,
                                            IStyleVariableAccessor variableAccessor)
        {
            return GetFunctionImpl(text, true, variableAccessor);
        }

        public static T GetValue<T>(String text,
                                    IStyleVariableAccessor variableAccessor)
        {
            var fn = GetFunction(text, variableAccessor);
            switch (fn.GetValue())
            {
                case T good:
                    return good;

                default:
                    throw new InvalidCastException();
            }
        }

        public static Object? GetValue(String text,
                                       IStyleVariableAccessor variableAccessor)
        {
            var fn = GetFunction(text, variableAccessor);
            return fn.GetValue();
        }

        private static IFunction GetFunctionImpl(String text,
                                                 Boolean isDissectLiterals,
                                                 IStyleVariableAccessor variableAccessor)
        {
            GetFunctionParts(text, out var functionName, 
                out var functionParameterMeat,
                out var literalValue);

            if (literalValue != null)
            {
                if (isDissectLiterals)
                    return GetFunctionFromLiteral(literalValue, variableAccessor);
                return new LiteralFunction(literalValue.Trim());
            }

            if (functionName != null && functionParameterMeat != null)
            {
                var paramTokens = GetFunctionTokens(functionParameterMeat).ToArray();

                if (paramTokens.Length == 0)
                    return new ParameterizedFunction(functionName, Enumerable.Empty<IFunction>(),
                        variableAccessor);
                
                var paramValues = new List<IFunction>();
                foreach (var paramToken in paramTokens)
                {
                    var paramFunc = GetFunctionImpl(paramToken, true, variableAccessor);
                    paramValues.Add(paramFunc);
                }

                return new ParameterizedFunction(functionName, paramValues, variableAccessor);
            }

            throw new NotImplementedException();
        }


        private static IFunction GetFunctionFromLiteral(String literalValue,
                                                        IStyleVariableAccessor variableAccessor)
        {
            var literalTokens = GetFunctionTokens(literalValue).ToArray();

            if (literalTokens.Length == 1)
            {
                return GetFunctionImpl(literalTokens[0], false, variableAccessor);
            }

            var values = new List<IFunction>();
            foreach (var paramToken in literalTokens)
            {
                var paramFunc = GetFunctionImpl(paramToken, true, variableAccessor);
                values.Add(paramFunc);
            }

            return new MultiFunction(values);
        }

        private static void GetFunctionParts(String text,
                                             out String? functionName,
                                             out String? functionParameterMeat,
                                             out String? literalValue)
        {
            var sb = new StringBuilder();

            functionName = default;
            functionParameterMeat = default;
            literalValue = default;
                

            var parenthesisCount = 0;
            var isSingleQuoteOpen = false;
            var isDoubleQuoteOpen = false;

            var hasHitValidComma = false;

            for (var c = 0; c < text.Length; c++)
            {
                var currentChar = text[c];
                var appendCurrent = true;

                switch (currentChar)
                {
                    case '(':
                        if (!isSingleQuoteOpen && !isDoubleQuoteOpen && !hasHitValidComma)
                        {
                            parenthesisCount++;

                            if (parenthesisCount == 1)
                            {
                                functionName = sb.ToString();
                                sb.Clear();
                                appendCurrent = false;
                            }
                        }

                        break;
                    
                    case ')':
                        if (!isSingleQuoteOpen && !isDoubleQuoteOpen)
                        {
                            parenthesisCount--;

                            if (parenthesisCount == 0)
                            {
                                appendCurrent = false;
                            }
                            
                        }

                        break;
                    
                    case '\'':
                        isSingleQuoteOpen = !isSingleQuoteOpen;
                        break;
                    
                    case '"':
                        isDoubleQuoteOpen = !isDoubleQuoteOpen;
                        break;
                    
                    case ',':
                        if (!isSingleQuoteOpen && !isDoubleQuoteOpen && functionName == null)
                            hasHitValidComma = true;
                        break;
                }

                if (appendCurrent)
                    sb.Append(currentChar);
            }

            if (functionName == null && sb.Length > 0)
                literalValue = sb.ToString();
            else if (functionName != null)
                functionParameterMeat = sb.ToString();

        }
        
        private static IEnumerable<String> GetFunctionTokens(String text)
        {
            var sb = new StringBuilder();
            
            var parenthesisCount = 0;
            var isSingleQuoteOpen = false;
            var isDoubleQuoteOpen = false;

            for (var c = 0; c < text.Length; c++)
            {
                var currentChar = text[c];
                var appendCurrent = true;

                switch (currentChar)
                {
                    case '(':
                        if (!isSingleQuoteOpen && !isDoubleQuoteOpen)
                            parenthesisCount++;
                        break;
                    
                    case ')':
                        if (!isSingleQuoteOpen && !isDoubleQuoteOpen)
                            parenthesisCount--;
                        break;
                    
                    case '\'':
                        isSingleQuoteOpen = !isSingleQuoteOpen;
                        break;
                    
                    case '"':
                        isDoubleQuoteOpen = !isDoubleQuoteOpen;
                        break;
                    
                    case ',':
                        if (!isSingleQuoteOpen && !isDoubleQuoteOpen && parenthesisCount == 0)
                        {
                            yield return sb.ToString();
                            sb.Clear();
                            appendCurrent = false;
                        }

                        break;
                }

                if (appendCurrent)
                    sb.Append(currentChar);
            }

            if (sb.Length > 0)
                yield return sb.ToString();


        }

    }
}
