using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;

namespace Das.Views.Styles.Functions
{
    public class ParameterizedFunction : IFunction
    {
        public ParameterizedFunction(String functionName,
                                     IEnumerable<IFunction> parameterValues,
                                     IStyleVariableAccessor variableAccessor)
        {
            _variableAccessor = variableAccessor;
            FunctionName = functionName;
            _parameterValues = new List<IFunction>(parameterValues);
        }

        public T[] GetParameterValues<T>()
        where T : IConvertible
        {
            var res = new T[_parameterValues.Count];

            for (var c = 0; c < _parameterValues.Count; c++)
            {
                var current = _parameterValues[c];

                switch (current.GetValue())
                {
                    case T good:
                        res[c] = good;
                        break;

                    case IConvertible goodEnough:
                        res[c] = (T)Convert.ChangeType(goodEnough, typeof(T));
                        break;

                    default:
                        throw new InvalidCastException();
                }

            }
            return res;
        }

        public Object?[] GetParameterValues() =>_parameterValues.Select(p => p.GetValue()).ToArray();

        public Object? GetValue()
        {
            //var parameterValues = _parameterValues.Select(p => p.GetValue()).ToArray();
            var parameterValues = GetParameterValues();

            switch (FunctionName)
            {
                case "rgba":
                    return CallRgba(parameterValues);


                case "rgb":
                    return CallRgb(parameterValues);

                case "var":
                    if (parameterValues.Length > 0)
                    {
                        var varName = parameterValues[0];
                        if (varName is String strVarName)
                        {
                            var val = _variableAccessor.GetVariableValue<Object?>(strVarName);
                            if (val != null)
                                return val;
                        }

                        if (parameterValues.Length > 1)
                        {
                            return new List<Object?>(parameterValues.Skip(1)).ToArray();
                        }
                    }

                    throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        public String FunctionName { get; }

        public override String ToString()
        {
            return "fn: " + FunctionName + "(" + String.Join(",", _parameterValues) + ")";
        }

        private static Object? CallRgb(Object?[] parameterValues)
        {
            if (parameterValues.Length == 1)
            {
                switch (parameterValues[0])
                {
                    case Object?[] objArr:
                        parameterValues = objArr;
                        break;
                    
                    case IBrush brush:
                        return brush;
                }
            }

            if (parameterValues.Length != 3 ||
                !TryGetByte(parameterValues[0], out var red) ||
                !TryGetByte(parameterValues[1], out var green) ||
                !TryGetByte(parameterValues[2], out var blue))
                return default;

            return new SolidColorBrush(red, green, blue);
        }

        private static Object? CallRgba(Object?[] parameterValues)
        {
            if (parameterValues.Length == 2)
            {
                var color = GetBrushFromParameter(parameterValues[0]);
                if (color == null)
                    return default;
                
                if (//parameterValues[0] is IBrush brush &&
                    color is { } brush && 
                    parameterValues[1] is String s1 &&
                    Double.TryParse(s1, out var opacity))
                {
                    brush = brush.GetWithOpacity(opacity);
                    return brush;
                }
            }

            if (parameterValues.Length == 4)
            {
                if (!TryGetByte(parameterValues[0], out var red) ||
                    !TryGetByte(parameterValues[1], out var green) ||
                    !TryGetByte(parameterValues[2], out var blue) ||
                    !TryGetDouble(parameterValues[3], out var alpha))
                    return default;

                return new SolidColorBrush(red, green, blue, alpha);
            }

            return default;
        }

        private static IBrush? GetBrushFromParameter(Object? paramValue)
        {
            switch (paramValue)
            {
                case IBrush good:
                    return good;
                
                
                case Object?[] parameterValues:
                    if (parameterValues.Length != 3 ||
                        !TryGetByte(parameterValues[0], out var red) ||
                        !TryGetByte(parameterValues[1], out var green) ||
                        !TryGetByte(parameterValues[2], out var blue))
                        return default;

                    return new SolidColorBrush(red, green, blue);

                case String str:
                    if (str.StartsWith("#"))
                    {

                    }
                    else
                    {

                    }
                    break;
            }

            return default;
        }

        private static Boolean TryGetByte(Object? value,
                                          out Byte me)
        {
            if (value == null)
            {
                me = default;
                return false;
            }

            return Byte.TryParse(value.ToString(), out me);
        }
        
        private static Boolean TryGetDouble(Object? value,
                                          out Double me)
        {
            if (value == null)
            {
                me = default;
                return false;
            }

            return Double.TryParse(value.ToString(), out me);
        }

        private readonly List<IFunction> _parameterValues;
        private readonly IStyleVariableAccessor _variableAccessor;
    }
}