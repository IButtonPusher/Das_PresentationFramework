using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts.Open
{
    public struct LookupList
    {
        private const Int32 offsetLookupCount = 0;
        private const Int32 LookupOffsetArray = 2;
        private const Int32 sizeLookupOffset = 2;
        private readonly Int32 offset;

        public UInt16 LookupCount(FontTable Table)
        {
            return Table.GetUShort(offset);
        }

        public LookupTable Lookup(FontTable Table,
                                  UInt16 Index)
        {
            return new LookupTable(Table, offset + Table.GetUShort(offset + 2 + Index * 2));
        }

        public LookupList(Int32 Offset)
        {
            offset = Offset;
        }
    }
}
