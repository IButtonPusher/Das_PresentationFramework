using System;
using System.Threading.Tasks;

namespace Das.Views.Data
{
    // This struct records DependencyProperty/value pairs. We use the struct
    // extensively because LocalValueEnumerators may not be cached safely.
    // It is identical to base's LocalValueEntry except that it adds setters.
    public struct PropertyValue
    {
        public IDependencyProperty Property
        {
            get => _property;
            set => _property = value;
        }

        public Object? Value
        {
            get => _value;
            set => _value = value;
        }

        private IDependencyProperty _property;

        private Object? _value;
    }
}
