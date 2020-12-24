using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;

namespace Das.Views.Styles.Declarations
{
    public class StyleFunctionCall
    {
        public StyleFunctionCall(String functionName,
                                 Func<IEnumerable<Object?>> getFunctionParameters,
                                 IStyleVariableAccessor variableAccessor)
        {
            _variableAccessor = variableAccessor;
            FunctionName = functionName;
            GetFunctionParameters = getFunctionParameters;
        }

        private StyleFunctionCall(String functionName,
                                  String functionReturnValue,
                                  IStyleVariableAccessor variableAccessor)
            : this(functionName, () => GetParameters(functionReturnValue), variableAccessor)
        {
            _fnResult = functionReturnValue;
        }

        private StyleFunctionCall(String functionName,
                                  IEnumerable<StyleFunctionCall> parameterGetters,
                                  IStyleVariableAccessor variableAccessor)
            : this(functionName, () => GetParameters(new List<StyleFunctionCall>(parameterGetters)),
                variableAccessor)
        {
            _nestedCalls = new List<StyleFunctionCall>(parameterGetters);
        }

        public String FunctionName { get; }

        private Func<IEnumerable<Object?>> GetFunctionParameters { get; }

        public static StyleFunctionCall BuildFromRawText(String rawText,
                                                         IStyleVariableAccessor variableAccessor)
        {
            var i = 0;
            return BuildFromRawTextImpl(rawText, ref i, variableAccessor);
        }

        public Object? GetValue()
        {
            var prmValues = GetFunctionParameters().ToArray();

            if (FunctionName == String.Empty)
            {
                if (prmValues.Length == 1)
                    return prmValues[0];

                return prmValues;
            }

            var prmValueTokens = prmValues;

            switch (FunctionName)
            {
                case "rgba":
                    if (prmValueTokens.Length == 2)
                        if (prmValueTokens[0] is IBrush brush &&
                            Double.TryParse(prmValueTokens[1]?.ToString() ?? "",
                                out var opacity))
                        {
                            brush = brush.GetWithOpacity(opacity);
                            return brush;
                        }

                    if (prmValueTokens.Length == 4)
                        if (Byte.TryParse(prmValueTokens[0].ToString(), out var red) &&
                            Byte.TryParse(prmValueTokens[1].ToString(), out var green) &&
                            Byte.TryParse(prmValueTokens[2].ToString(), out var blue) &&
                            Byte.TryParse(prmValueTokens[3].ToString(), out var alpha))
                            return new SolidColorBrush(red, green, blue, alpha);

                    break;

                case "rgb":
                    if (prmValueTokens.Length == 3)
                        if (Byte.TryParse(prmValueTokens[0].ToString(), out var red) &&
                            Byte.TryParse(prmValueTokens[1].ToString(), out var green) &&
                            Byte.TryParse(prmValueTokens[2].ToString(), out var blue))
                            return new SolidColorBrush(red, green, blue);
                    break;

                case "var":

                    var varValue = _variableAccessor.GetVariableValue<Object?>(prmValueTokens[0].ToString());
                    if (varValue == null && prmValueTokens.Length > 1) throw new NotImplementedException();

                    return varValue;

                default:
                    if (prmValues.Length > 0)
                        return prmValues;

                    break;
            }


            return default;
        }

        public override String ToString()
        {
            return "fn call: " + (FunctionName.Length > 0 ? FunctionName : _fnResult ?? "");
        }

        private static StyleFunctionCall BuildFromRawTextImpl(String rawText,
                                                              ref Int32 i,
                                                              IStyleVariableAccessor variableAccessor)
        {
            var fnName = String.Empty;
            var getters = new List<StyleFunctionCall>();
            var prmLiterals = String.Empty;

            var currentChar = 'ä';

            for (; i < rawText.Length; i++)
            {
                currentChar = rawText[i];

                switch (currentChar)
                {
                    case '(':
                        i++;

                        TryAddLiteralCall(ref prmLiterals, variableAccessor, getters);

                        var childCall = BuildFromRawTextImpl(rawText, ref i, variableAccessor);
                        getters.Add(childCall);
                        break;

                    case ')':
                        goto allDone;

                    case ',':
                        if (fnName.Length > 0 && prmLiterals.Length == 0 &&
                            getters.Count == 0)
                        {
                            // this is not a function call, it's a parameter list
                            prmLiterals = fnName;
                            fnName = String.Empty;
                        }
                        
                        
                        TryAddLiteralCall(ref prmLiterals, variableAccessor, getters);
                        break;

                    default:

                        if (getters.Count == 0)
                            fnName += currentChar;
                        else
                            prmLiterals += currentChar;
                        break;
                }
            }

            allDone:

            TryAddLiteralCall(ref prmLiterals, variableAccessor, getters);

            switch (getters.Count)
            {
                case 0: //no fn evaluation needed

                    if (currentChar == ')')
                        i--;

                    return new StyleFunctionCall(String.Empty, fnName, variableAccessor);

                case 1:
                    return new StyleFunctionCall(fnName, getters, variableAccessor);

                default:
                    return new StyleFunctionCall(fnName, getters, variableAccessor);
            }
        }

        private static StyleFunctionCall BuildLiteralCall(ref String prmLiterals,
                                                          IStyleVariableAccessor variableAccessor)
        {
            var literalCall = new StyleFunctionCall(String.Empty, prmLiterals.Trim(),
                variableAccessor);

            prmLiterals = String.Empty;
            return literalCall;
        }

        private String GetParameters()
        {
            if (_nestedCalls is { } calls)
                return String.Join(", ", calls.Select(p => p.GetValue()));

            return String.Empty;
        }

        private static IEnumerable<Object> GetParameters(String literal)
        {
            yield return literal;
        }

        private static IEnumerable<Object?> GetParameters(List<StyleFunctionCall> calls)
        {
            foreach (var call in calls)
                yield return call.GetValue();
        }

        private static void TryAddLiteralCall(ref String prmLiterals,
                                              IStyleVariableAccessor variableAccessor,
                                              List<StyleFunctionCall> getters)
        {
            if (prmLiterals.Length <= 0)
                return;

            var literalCall = BuildLiteralCall(ref prmLiterals, variableAccessor);
            getters.Add(literalCall);
            prmLiterals = String.Empty;
        }

        private readonly String? _fnResult;
        private readonly List<StyleFunctionCall>? _nestedCalls;
        private readonly IStyleVariableAccessor _variableAccessor;
    }
}