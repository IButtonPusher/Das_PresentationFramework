using System;
using System.Threading.Tasks;

namespace Das.Views.Data
{
    [Serializable]
    public struct SecurityCriticalDataForSet<T>
    {
        private T _value;

        internal SecurityCriticalDataForSet(T value)
        {
            _value = value;
        }

        internal T Value
        {
            get => _value;
            set => _value = value;
        }
    }
}
