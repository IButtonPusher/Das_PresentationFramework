using System;
using System.Threading.Tasks;
using Das.Views.Images.Svg.Helpers;

namespace Das.Views.Images.Svg
{
    public enum NumState
    {
        Invalid,
        Separator,
        Prefix,
        Integer,
        DecPlace,
        Fraction,
        Exponent,
        ExpPrefix,
        ExpValue
    }

    public ref struct CoordinateParserState
    {
        public NumState CurrNumState;
        public NumState NewNumState;
        public Int32 CharsPosition;
        public Int32 Position;
        public Boolean HasMore;

        public CoordinateParserState(ref ReadOnlySpan<Char> chars)
        {
            CurrNumState = NumState.Separator;
            NewNumState = NumState.Separator;
            CharsPosition = 0;
            Position = 0;
            HasMore = chars.Length > 0;
            if (Char.IsLetter(chars[0])) ++CharsPosition;
        }
    }

    public static class CoordinateParser
    {
        public static Boolean TryGetBool(out Boolean result,
                                         ref ReadOnlySpan<Char> chars,
                                         ref CoordinateParserState state)
        {
            var charsLength = chars.Length;

            while (state.CharsPosition < charsLength && state.HasMore)
            {
                switch (state.CurrNumState)
                {
                    case NumState.Separator:
                        var currentChar = chars[state.CharsPosition];
                        if (IsCoordSeparator(currentChar))
                            state.NewNumState = NumState.Separator;
                        else if (currentChar == '0')
                        {
                            result = false;
                            state.NewNumState = NumState.Separator;
                            state.Position = state.CharsPosition + 1;
                            return MarkState(true, ref state);
                        }
                        else if (currentChar == '1')
                        {
                            result = true;
                            state.NewNumState = NumState.Separator;
                            state.Position = state.CharsPosition + 1;
                            return MarkState(true, ref state);
                        }
                        else
                        {
                            result = false;
                            return MarkState(false, ref state);
                        }

                        break;
                    default:
                        result = false;
                        return MarkState(false, ref state);
                }

                ++state.CharsPosition;
            }

            result = false;
            return MarkState(false, ref state);
        }

        public static Boolean TryGetFloat(out Single result,
                                          ref ReadOnlySpan<Char> chars,
                                          ref CoordinateParserState state)
        {
            var charsLength = chars.Length;

            while (state.CharsPosition < charsLength && state.HasMore)
            {
                var currentChar = chars[state.CharsPosition];

                switch (state.CurrNumState)
                {
                    case NumState.Separator:
                        if (Char.IsNumber(currentChar))
                            state.NewNumState = NumState.Integer;
                        else if (IsCoordSeparator(currentChar))
                            state.NewNumState = NumState.Separator;
                        else
                            switch (currentChar)
                            {
                                case '.':
                                    state.NewNumState = NumState.DecPlace;
                                    break;
                                case '+':
                                case '-':
                                    state.NewNumState = NumState.Prefix;
                                    break;
                                default:
                                    state.NewNumState = NumState.Invalid;
                                    break;
                            }

                        break;
                    case NumState.Prefix:
                        if (Char.IsNumber(currentChar))
                            state.NewNumState = NumState.Integer;
                        else if (currentChar == '.')
                            state.NewNumState = NumState.DecPlace;
                        else
                            state.NewNumState = NumState.Invalid;
                        break;
                    case NumState.Integer:
                        if (Char.IsNumber(currentChar))
                            state.NewNumState = NumState.Integer;
                        else if (IsCoordSeparator(currentChar))
                            state.NewNumState = NumState.Separator;
                        else
                            switch (currentChar)
                            {
                                case '.':
                                    state.NewNumState = NumState.DecPlace;
                                    break;
                                case 'E':
                                case 'e':
                                    state.NewNumState = NumState.Exponent;
                                    break;
                                case '+':
                                case '-':
                                    state.NewNumState = NumState.Prefix;
                                    break;
                                default:
                                    state.NewNumState = NumState.Invalid;
                                    break;
                            }

                        break;
                    case NumState.DecPlace:
                        if (Char.IsNumber(currentChar))
                            state.NewNumState = NumState.Fraction;
                        else if (IsCoordSeparator(currentChar))
                            state.NewNumState = NumState.Separator;
                        else
                            switch (currentChar)
                            {
                                case 'E':
                                case 'e':
                                    state.NewNumState = NumState.Exponent;
                                    break;
                                case '+':
                                case '-':
                                    state.NewNumState = NumState.Prefix;
                                    break;
                                default:
                                    state.NewNumState = NumState.Invalid;
                                    break;
                            }

                        break;
                    case NumState.Fraction:
                        if (Char.IsNumber(currentChar))
                            state.NewNumState = NumState.Fraction;
                        else if (IsCoordSeparator(currentChar))
                            state.NewNumState = NumState.Separator;
                        else
                            switch (currentChar)
                            {
                                case '.':
                                    state.NewNumState = NumState.DecPlace;
                                    break;
                                case 'E':
                                case 'e':
                                    state.NewNumState = NumState.Exponent;
                                    break;
                                case '+':
                                case '-':
                                    state.NewNumState = NumState.Prefix;
                                    break;
                                default:
                                    state.NewNumState = NumState.Invalid;
                                    break;
                            }

                        break;
                    case NumState.Exponent:
                        if (Char.IsNumber(currentChar))
                            state.NewNumState = NumState.ExpValue;
                        else if (IsCoordSeparator(currentChar))
                            state.NewNumState = NumState.Invalid;
                        else
                            switch (currentChar)
                            {
                                case '+':
                                case '-':
                                    state.NewNumState = NumState.ExpPrefix;
                                    break;
                                default:
                                    state.NewNumState = NumState.Invalid;
                                    break;
                            }

                        break;
                    case NumState.ExpPrefix:
                        if (Char.IsNumber(currentChar))
                            state.NewNumState = NumState.ExpValue;
                        else
                            state.NewNumState = NumState.Invalid;
                        break;
                    case NumState.ExpValue:
                        if (Char.IsNumber(currentChar))
                            state.NewNumState = NumState.ExpValue;
                        else if (IsCoordSeparator(currentChar))
                            state.NewNumState = NumState.Separator;
                        else
                            switch (currentChar)
                            {
                                case '.':
                                    state.NewNumState = NumState.DecPlace;
                                    break;
                                case '+':
                                case '-':
                                    state.NewNumState = NumState.Prefix;
                                    break;
                                default:
                                    state.NewNumState = NumState.Invalid;
                                    break;
                            }

                        break;
                }

                if (state.CurrNumState != NumState.Separator && state.NewNumState < state.CurrNumState)
                {
                    var value = chars.Slice(state.Position, state.CharsPosition - state.Position);
                    result = StringParser.ToFloat(ref value);
                    state.Position = state.CharsPosition;
                    state.CurrNumState = state.NewNumState;
                    return MarkState(true, ref state);
                }

                if (state.NewNumState != state.CurrNumState && state.CurrNumState == NumState.Separator)
                    state.Position = state.CharsPosition;

                if (state.NewNumState == NumState.Invalid)
                {
                    result = Single.MinValue;
                    return MarkState(false, ref state);
                }

                state.CurrNumState = state.NewNumState;
                ++state.CharsPosition;
            }

            if (state.CurrNumState == NumState.Separator || !state.HasMore || state.Position >= charsLength)
            {
                result = Single.MinValue;
                return MarkState(false, ref state);
            }

            {
                var value = chars.Slice(state.Position, charsLength - state.Position);
                result = StringParser.ToFloat(ref value);
                state.Position = charsLength;
                return MarkState(true, ref state);
            }
        }

        private static Boolean IsCoordSeparator(Char value)
        {
            switch (value)
            {
                case ' ':
                case '\t':
                case '\n':
                case '\r':
                case ',':
                    return true;
            }

            return false;
        }

        private static Boolean MarkState(Boolean hasMode,
                                         ref CoordinateParserState state)
        {
            state.HasMore = hasMode;
            ++state.CharsPosition;
            return hasMode;
        }
    }
}
