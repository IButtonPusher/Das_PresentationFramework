using System;
using System.Collections.Generic;

namespace Das.Views.Styles.Functions
{
    public class MultiFunction : IFunction
    {
        public MultiFunction(IEnumerable<IFunction> functions)
        {
            _functions = new List<IFunction>(functions);
        }

        Object? IFunction.GetValue()
        {
            throw new NotImplementedException();
        }

        public override String ToString()
        {
            return "multi => (" + String.Join(", ", _functions) + ")";
        }

        private readonly List<IFunction> _functions;

        public IEnumerable<IFunction> Functions => _functions;
    }
}
