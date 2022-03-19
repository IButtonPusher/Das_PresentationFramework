using System;
using System.Collections;
using System.Threading.Tasks;
using Das.Views.DependencyProperties;
using Das.Views.Localization;

namespace Das.Views.Collections
{
    public struct LocalValueEnumerator : IEnumerator
    {
        private Int32 _index;
        private readonly LocalValueEntry[] _snapshot;
        private readonly Int32 _count;

        public override Int32 GetHashCode()
        {
            // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
            return base.GetHashCode();
        }

        public override Boolean Equals(Object obj)
        {
            return obj is LocalValueEnumerator localValueEnumerator && _count == localValueEnumerator._count &&
                   _index == localValueEnumerator._index && _snapshot == localValueEnumerator._snapshot;
        }

        public static Boolean operator ==(LocalValueEnumerator obj1,
                                          LocalValueEnumerator obj2)
        {
            return obj1.Equals(obj2);
        }

        public static Boolean operator !=(LocalValueEnumerator obj1,
                                          LocalValueEnumerator obj2)
        {
            return !(obj1 == obj2);
        }

        public LocalValueEntry Current
        {
            get
            {
                if (_index == -1)
                    throw new InvalidOperationException(SR.Get("LocalValueEnumerationReset"));
                return _index < Count
                    ? _snapshot[_index]
                    : throw new InvalidOperationException(SR.Get("LocalValueEnumerationOutOfBounds"));
            }
        }

        Object IEnumerator.Current => Current;

        public Boolean MoveNext()
        {
            ++_index;
            return _index < Count;
        }

        public void Reset()
        {
            _index = -1;
        }

        public Int32 Count => _count;

        internal LocalValueEnumerator(LocalValueEntry[] snapshot,
                                      Int32 count)
        {
            _index = -1;
            _count = count;
            _snapshot = snapshot;
        }
    }
}
