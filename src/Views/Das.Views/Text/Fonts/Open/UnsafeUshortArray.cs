using System;
using System.Threading.Tasks;
using Das.Views.Data;
using Das.Views.Validation;

namespace Das.Views.Text.Fonts.Open
{
    public class UnsafeUshortArray : UshortBuffer
    {
        internal unsafe UnsafeUshortArray(CheckedUShortPointer array,
                                          Int32 arrayLength)
        {
            _array = array.Probe(0, arrayLength);
            _arrayLength.Value = arrayLength;
        }

        public override unsafe UInt16 this[Int32 index]
        {
            get
            {
                Invariant.Assert(index >= 0 && index < _arrayLength.Value);
                return _array[index];
            }
            set
            {
                Invariant.Assert(index >= 0 && index < _arrayLength.Value);
                _array[index] = value;
            }
        }

        public override Int32 Length => _arrayLength.Value;

        private readonly unsafe UInt16* _array;
        private SecurityCriticalDataForSet<Int32> _arrayLength;
    }
}
