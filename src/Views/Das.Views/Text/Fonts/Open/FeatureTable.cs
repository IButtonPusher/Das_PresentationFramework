using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts
{
    public struct FeatureTable
    {
        private const Int32 offsetLookupCount = 2;
        private const Int32 offsetLookupIndexArray = 4;
        private const Int32 sizeLookupIndex = 2;
        private readonly Int32 offset;

        public UInt16 LookupCount(FontTable Table)
        {
            return Table.GetUShort(offset + 2);
        }

        public UInt16 LookupIndex(FontTable Table,
                                  UInt16 Index)
        {
            return Table.GetUShort(offset + 4 + Index * 2);
        }

        public FeatureTable(Int32 Offset)
        {
            offset = Offset;
        }

        public Boolean IsNull => offset == Int32.MaxValue;
    }
}
