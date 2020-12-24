using System;

namespace Das.Views.Styles.Functions
{
    public class LiteralFunction : IFunction
    {
        private readonly Object? _value;

        public LiteralFunction(Object? value)
        {
            _value = value;
        }

        public Object? GetValue()
        {
            return _value;
        }

        public override String ToString()
        {
            return "literal => " + _value;
        }
    }
}
