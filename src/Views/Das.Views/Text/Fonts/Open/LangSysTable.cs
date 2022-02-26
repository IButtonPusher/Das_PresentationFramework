using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts
{
    public struct LangSysTable
    {
        private const Int32 offsetRequiredFeature = 2;
        private const Int32 offsetFeatureCount = 4;
        private const Int32 offsetFeatureIndexArray = 6;
        private const Int32 sizeFeatureIndex = 2;
        private readonly Int32 offset;

        public FeatureTable FindFeature(FontTable Table,
                                        FeatureList Features,
                                        UInt32 FeatureTag)
        {
            var num = FeatureCount(Table);
            for (UInt16 Index = 0; Index < num; ++Index)
            {
                var featureIndex = GetFeatureIndex(Table, Index);
                if ((Int32)Features.FeatureTag(Table, featureIndex) == (Int32)FeatureTag)
                    return Features.FeatureTable(Table, featureIndex);
            }

            return new FeatureTable(Int32.MaxValue);
        }

        public FeatureTable RequiredFeature(FontTable Table,
                                            FeatureList Features)
        {
            var Index = Table.GetUShort(offset + 2);
            return Index != UInt16.MaxValue ? Features.FeatureTable(Table, Index) : new FeatureTable(Int32.MaxValue);
        }

        public UInt16 FeatureCount(FontTable Table)
        {
            return Table.GetUShort(offset + 4);
        }

        public UInt16 GetFeatureIndex(FontTable Table,
                                      UInt16 Index)
        {
            return Table.GetUShort(offset + 6 + Index * 2);
        }

        public LangSysTable(Int32 Offset)
        {
            offset = Offset;
        }

        public Boolean IsNull => offset == Int32.MaxValue;
    }
}
