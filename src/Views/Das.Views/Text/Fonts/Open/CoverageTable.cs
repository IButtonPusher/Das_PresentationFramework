using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts.Open
{
    public struct CoverageTable
    {
        private const Int32 offsetFormat = 0;
        private const Int32 offsetFormat1GlyphCount = 2;
        private const Int32 offsetFormat1GlyphArray = 4;
        private const Int32 sizeFormat1GlyphId = 2;
        private const Int32 offsetFormat2RangeCount = 2;
        private const Int32 offsetFormat2RangeRecordArray = 4;
        private const Int32 sizeFormat2RangeRecord = 6;
        private const Int32 offsetFormat2RangeRecordStart = 0;
        private const Int32 offsetFormat2RangeRecordEnd = 2;
        private const Int32 offsetFormat2RangeRecordStartIndex = 4;
        private readonly Int32 offset;

        public UInt16 Format(FontTable Table)
        {
            return Table.GetUShort(offset);
        }

        public UInt16 Format1GlyphCount(FontTable Table)
        {
            return Table.GetUShort(offset + 2);
        }

        public UInt16 Format1Glyph(FontTable Table,
                                   UInt16 Index)
        {
            return Table.GetUShort(offset + 4 + Index * 2);
        }

        public UInt16 Format2RangeCount(FontTable Table)
        {
            return Table.GetUShort(offset + 2);
        }

        public UInt16 Format2RangeStartGlyph(FontTable Table,
                                             UInt16 Index)
        {
            return Table.GetUShort(offset + 4 + Index * 6);
        }

        public UInt16 Format2RangeEndGlyph(FontTable Table,
                                           UInt16 Index)
        {
            return Table.GetUShort(offset + 4 + Index * 6 + 2);
        }

        public UInt16 Format2RangeStartCoverageIndex(FontTable Table,
                                                     UInt16 Index)
        {
            return Table.GetUShort(offset + 4 + Index * 6 + 4);
        }

        public Int32 GetGlyphIndex(FontTable Table,
                                   UInt16 glyph)
        {
            switch (Format(Table))
            {
                case 1:
                    UInt16 num1 = 0;
                    var num2 = Format1GlyphCount(Table);
                    while (num1 < num2)
                    {
                        var Index = (UInt16)((num1 + num2) >> 1);
                        var num3 = Format1Glyph(Table, Index);
                        if (glyph < num3)
                        {
                            num2 = Index;
                        }
                        else
                        {
                            if (glyph <= num3)
                                return Index;
                            num1 = (UInt16)(Index + 1U);
                        }
                    }

                    return -1;
                case 2:
                    UInt16 num4 = 0;
                    var num5 = Format2RangeCount(Table);
                    while (num4 < num5)
                    {
                        var Index = (UInt16)((num4 + num5) >> 1);
                        if (glyph < Format2RangeStartGlyph(Table, Index))
                        {
                            num5 = Index;
                        }
                        else
                        {
                            if (glyph <= Format2RangeEndGlyph(Table, Index))
                                return glyph - Format2RangeStartGlyph(Table, Index) +
                                       Format2RangeStartCoverageIndex(Table, Index);
                            num4 = (UInt16)(Index + 1U);
                        }
                    }

                    return -1;
                default:
                    return -1;
            }
        }

        public Boolean IsAnyGlyphCovered(FontTable table,
                                         UInt32[] glyphBits,
                                         UInt16 minGlyphId,
                                         UInt16 maxGlyphId)
        {
            switch (Format(table))
            {
                case 1:
                    var num1 = Format1GlyphCount(table);
                    if (num1 == 0)
                        return false;
                    var num2 = Format1Glyph(table, 0);
                    var num3 = Format1Glyph(table, (UInt16)(num1 - 1U));
                    if (maxGlyphId < num2 || minGlyphId > num3)
                        return false;
                    for (UInt16 Index = 0; Index < num1; ++Index)
                    {
                        var num4 = Format1Glyph(table, Index);
                        if (num4 <= maxGlyphId && num4 >= minGlyphId && (glyphBits[num4 >> 5] & 1 << (num4 % 32)) != 0L)
                            return true;
                    }

                    return false;
                case 2:
                    var num5 = Format2RangeCount(table);
                    if (num5 == 0)
                        return false;
                    var num6 = Format2RangeStartGlyph(table, 0);
                    var num7 = Format2RangeEndGlyph(table, (UInt16)(num5 - 1U));
                    if (maxGlyphId < num6 || minGlyphId > num7)
                        return false;
                    for (UInt16 Index = 0; Index < num5; ++Index)
                    {
                        Int32 num8 = Format2RangeStartGlyph(table, Index);
                        var num9 = Format2RangeEndGlyph(table, Index);
                        for (var index = (UInt16)num8; index <= num9; ++index)
                        {
                            if (index <= maxGlyphId && index >= minGlyphId &&
                                (glyphBits[index >> 5] & 1 << (index % 32)) != 0L)
                                return true;
                        }
                    }

                    return false;
                default:
                    return true;
            }
        }

        public static CoverageTable InvalidCoverage => new CoverageTable(-1);

        public Boolean IsInvalid => offset == -1;

        public CoverageTable(Int32 Offset)
        {
            offset = Offset;
        }
    }
}
