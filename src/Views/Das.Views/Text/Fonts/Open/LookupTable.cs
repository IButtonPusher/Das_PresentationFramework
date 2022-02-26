using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts.Open
{
    public struct LookupTable
    {
        private const Int32 offsetLookupType = 0;
        private const Int32 offsetLookupFlags = 2;
        private const Int32 offsetSubtableCount = 4;
        private const Int32 offsetSubtableArray = 6;
        private const Int32 sizeSubtableOffset = 2;
        private readonly Int32 offset;
        private readonly UInt16 lookupType;
        private readonly UInt16 lookupFlags;
        private readonly UInt16 subtableCount;

        public UInt16 LookupType()
        {
            return lookupType;
        }

        public UInt16 LookupFlags()
        {
            return lookupFlags;
        }

        public UInt16 SubTableCount()
        {
            return subtableCount;
        }

        public Int32 SubtableOffset(FontTable Table,
                                    UInt16 Index)
        {
            return offset + Table.GetOffset(offset + 6 + Index * 2);
        }

        public LookupTable(FontTable table,
                           Int32 Offset)
        {
            offset = Offset;
            lookupType = table.GetUShort(offset);
            lookupFlags = table.GetUShort(offset + 2);
            subtableCount = table.GetUShort(offset + 4);
        }
    }
}
