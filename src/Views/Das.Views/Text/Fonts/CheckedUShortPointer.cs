using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts
{
    public struct CheckedUShortPointer
    {
        private CheckedPointer _checkedPointer;

        public unsafe CheckedUShortPointer(UInt16* pointer,
                                           Int32 length)
        {
            _checkedPointer = new CheckedPointer(pointer, length * 2);
        }

        public unsafe UInt16* Probe(Int32 offset,
                                    Int32 length)
        {
            return (UInt16*)_checkedPointer.Probe(offset * 2, length * 2);
        }
    }
}
