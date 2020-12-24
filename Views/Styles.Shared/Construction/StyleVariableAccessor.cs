using System;
using System.Collections.Generic;
using Das.Views.Styles;

namespace Das.Views.Construction.Styles
{
    public class StyleVariableAccessor : IStyleVariableAccessor
    {
        public StyleVariableAccessor()
        {
            _lock = new Object();
            _values = new Dictionary<String, Object?>();
            _promises = new Dictionary<String, Delegate>();
        }

        public T GetVariableValue<T>(String variableName)
        {
            lock (_lock)
            {
                if (_values.TryGetValue(variableName, out var value))
                {
                    switch (value)
                    {
                        case T good:
                            return good;

                        default:
                            throw new InvalidCastException();
                    }
                }

                if (!_promises.TryGetValue(variableName, out var hope))
                    return default!;

                switch (hope)
                {
                    case Func<T> good:
                        return good();

                    default:
                        throw new InvalidCastException();
                }
            }
        }

        public void SetVariableValue<T>(String variableName, 
                                        T value)
        {
            lock (_lock)
            {
                _values[variableName] = value;
            }
        }

        public void SetVariableValue<T>(String variableName, 
                                        Func<T> value)
        {
            lock (_lock)
                _promises[variableName] = value;
        }

        private readonly Object _lock;
        private readonly Dictionary<String, Object?> _values;
        private readonly Dictionary<String, Delegate> _promises;
    }
}
