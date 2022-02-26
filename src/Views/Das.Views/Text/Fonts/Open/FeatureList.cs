using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts
{
    public struct FeatureList
    {
        private const Int32 offsetFeatureCount = 0;
        private const Int32 offsetFeatureRecordArray = 2;
        private const Int32 sizeFeatureRecord = 6;
        private const Int32 offsetFeatureRecordTag = 0;
        private const Int32 offsetFeatureRecordOffset = 4;
        private readonly Int32 offset;

        public UInt16 FeatureCount(FontTable Table)
        {
            return Table.GetUShort(offset);
        }

        public UInt32 FeatureTag(FontTable Table,
                                 UInt16 Index)
        {
            return Table.GetUInt(offset + 2 + Index * 6);
        }

        public FeatureTable FeatureTable(FontTable Table,
                                         UInt16 Index)
        {
            return new FeatureTable(offset + Table.GetUShort(offset + 2 + Index * 6 + 4));
        }

        public FeatureList(Int32 Offset)
        {
            offset = Offset;
        }
    }
}
