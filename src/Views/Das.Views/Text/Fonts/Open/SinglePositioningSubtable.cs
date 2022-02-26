using System;
using System.Threading.Tasks;
using Das.Views.Layout;
using Das.Views.Validation;

namespace Das.Views.Text.Fonts.Open
{
    public struct SinglePositioningSubtable
    {
        private const Int32 offsetFormat = 0;
        private const Int32 offsetCoverage = 2;
        private const Int32 offsetValueFormat = 4;
        private const Int32 offsetFormat1Value = 6;
        private const Int32 offsetFormat2ValueCount = 6;
        private const Int32 offsetFormat2ValueArray = 8;
        private readonly Int32 offset;

        private UInt16 Format(FontTable Table)
        {
            return Table.GetUShort(offset);
        }

        private CoverageTable Coverage(FontTable Table)
        {
            return new CoverageTable(offset + Table.GetOffset(offset + 2));
        }

        private UInt16 ValueFormat(FontTable Table)
        {
            return Table.GetUShort(offset + 4);
        }

        private ValueRecordTable Format1ValueRecord(FontTable Table)
        {
            Invariant.Assert(Format(Table) == 1);
            return new ValueRecordTable(offset + 6, offset, ValueFormat(Table));
        }

        private ValueRecordTable Format2ValueRecord(FontTable Table,
                                                    UInt16 Index)
        {
            Invariant.Assert(Format(Table) == 2);
            return new ValueRecordTable(offset + 8 + Index * ValueRecordTable.Size(ValueFormat(Table)), offset,
                ValueFormat(Table));
        }

        public unsafe Boolean Apply(FontTable Table,
                                    LayoutMetrics Metrics,
                                    GlyphInfoList GlyphInfo,
                                    Int32* Advances,
                                    LayoutOffset* Offsets,
                                    Int32 FirstGlyph,
                                    Int32 AfterLastGlyph,
                                    out Int32 NextGlyph)
        {
            Invariant.Assert(FirstGlyph >= 0);
            Invariant.Assert(AfterLastGlyph <= GlyphInfo.Length);
            NextGlyph = FirstGlyph + 1;
            var length = GlyphInfo.Length;
            var glyph = GlyphInfo.Glyphs[FirstGlyph];
            var glyphIndex = Coverage(Table).GetGlyphIndex(Table, glyph);
            if (glyphIndex == -1)
                return false;
            ValueRecordTable valueRecordTable;
            switch (Format(Table))
            {
                case 1:
                    valueRecordTable = Format1ValueRecord(Table);
                    break;
                case 2:
                    valueRecordTable = Format2ValueRecord(Table, (UInt16)glyphIndex);
                    break;
                default:
                    return false;
            }

            valueRecordTable.AdjustPos(Table, Metrics, ref Offsets[FirstGlyph], ref Advances[FirstGlyph]);
            return true;
        }

        public Boolean IsLookupCovered(FontTable table,
                                       UInt32[] glyphBits,
                                       UInt16 minGlyphId,
                                       UInt16 maxGlyphId)
        {
            return Coverage(table).IsAnyGlyphCovered(table, glyphBits, minGlyphId, maxGlyphId);
        }

        public CoverageTable GetPrimaryCoverage(FontTable table)
        {
            return Coverage(table);
        }

        public SinglePositioningSubtable(Int32 Offset)
        {
            offset = Offset;
        }
    }
}
