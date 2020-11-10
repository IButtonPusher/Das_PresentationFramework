using System;
using System.Threading.Tasks;

#pragma warning disable 8618

namespace AsyncResults.Enumerable
{
    public abstract class CurrentValueContainer<T> : AsyncEnumerator
    {
        internal T CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                HasCurrentValue = true;
            }
        }

        internal Boolean HasCurrentValue { get; private set; }

        private T _currentValue;
    }
}