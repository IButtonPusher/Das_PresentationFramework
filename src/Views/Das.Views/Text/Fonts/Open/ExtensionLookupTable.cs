using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts.Open
{
    public struct ExtensionLookupTable
    {
        private const Int32 offsetFormat = 0;
        private const Int32 offsetLookupType = 2;
        private const Int32 offsetExtensionOffset = 4;
        private readonly Int32 offset;

        public UInt16 LookupType(FontTable Table)
        {
            return Table.GetUShort(offset + 2);
        }

        public Int32 LookupSubtableOffset(FontTable Table)
        {
            return offset + (Int32)Table.GetUInt(offset + 4);
        }

        public ExtensionLookupTable(Int32 Offset)
        {
            offset = Offset;
        }
    }
}
