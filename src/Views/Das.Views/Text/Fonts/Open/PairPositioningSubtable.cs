using System;
using System.Threading.Tasks;
using Das.Views.Layout;
using Das.Views.Validation;

namespace Das.Views.Text.Fonts.Open
{
    public struct PairPositioningSubtable
    {
        private const Int32 offsetFormat = 0;
        private const Int32 offsetCoverage = 2;
        private const Int32 offsetValueFormat1 = 4;
        private const Int32 offsetValueFormat2 = 6;
        private const Int32 offsetFormat1PairSetCount = 8;
        private const Int32 offsetFormat1PairSetArray = 10;
        private const Int32 sizeFormat1PairSetOffset = 2;
        private const Int32 offsetFormat2ClassDef1 = 8;
        private const Int32 offsetFormat2ClassDef2 = 10;
        private const Int32 offsetFormat2Class1Count = 12;
        private const Int32 offsetFormat2Class2Count = 14;
        private const Int32 offsetFormat2ValueRecordArray = 16;
        private readonly Int32 offset;

        private UInt16 Format(FontTable Table)
        {
            return Table.GetUShort(offset);
        }

        private CoverageTable Coverage(FontTable Table)
        {
            return new CoverageTable(offset + Table.GetOffset(offset + 2));
        }

        private UInt16 FirstValueFormat(FontTable Table)
        {
            return Table.GetUShort(offset + 4);
        }

        private UInt16 SecondValueFormat(FontTable Table)
        {
            return Table.GetUShort(offset + 6);
        }

        private PairSetTable Format1PairSet(FontTable Table,
                                            UInt16 Index)
        {
            Invariant.Assert(Format(Table) == 1);
            return new PairSetTable(offset + Table.GetUShort(offset + 10 + Index * 2), FirstValueFormat(Table),
                SecondValueFormat(Table));
        }

        private ClassDefTable Format2Class1Table(FontTable Table)
        {
            Invariant.Assert(Format(Table) == 2);
            return new ClassDefTable(offset + Table.GetUShort(offset + 8));
        }

        private ClassDefTable Format2Class2Table(FontTable Table)
        {
            Invariant.Assert(Format(Table) == 2);
            return new ClassDefTable(offset + Table.GetUShort(offset + 10));
        }

        private UInt16 Format2Class1Count(FontTable Table)
        {
            Invariant.Assert(Format(Table) == 2);
            return Table.GetUShort(offset + 12);
        }

        private UInt16 Format2Class2Count(FontTable Table)
        {
            Invariant.Assert(Format(Table) == 2);
            return Table.GetUShort(offset + 14);
        }

        private ValueRecordTable Format2FirstValueRecord(FontTable Table,
                                                         UInt16 Class2Count,
                                                         UInt16 Class1Index,
                                                         UInt16 Class2Index)
        {
            Invariant.Assert(Format(Table) == 2);
            var Format1 = FirstValueFormat(Table);
            var Format2 = SecondValueFormat(Table);
            var num = ValueRecordTable.Size(Format1) + ValueRecordTable.Size(Format2);
            return new ValueRecordTable(offset + 16 + (Class1Index * Class2Count + Class2Index) * num, offset, Format1);
        }

        private ValueRecordTable Format2SecondValueRecord(FontTable Table,
                                                          UInt16 Class2Count,
                                                          UInt16 Class1Index,
                                                          UInt16 Class2Index)
        {
            Invariant.Assert(Format(Table) == 2);
            Int32 Format1 = FirstValueFormat(Table);
            var Format2 = SecondValueFormat(Table);
            Int32 num1 = ValueRecordTable.Size((UInt16)Format1);
            var num2 = num1 + ValueRecordTable.Size(Format2);
            return new ValueRecordTable(offset + 16 + (Class1Index * Class2Count + Class2Index) * num2 + num1, offset,
                Format2);
        }

        public unsafe Boolean Apply(IOpenTypeFont Font,
                                    FontTable Table,
                                    LayoutMetrics Metrics,
                                    GlyphInfoList GlyphInfo,
                                    UInt16 LookupFlags,
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
            var glyph1 = GlyphInfo.Glyphs[FirstGlyph];
            var nextGlyphInLookup = LayoutEngine.GetNextGlyphInLookup(Font, GlyphInfo, FirstGlyph + 1, LookupFlags, 1);
            if (nextGlyphInLookup >= AfterLastGlyph)
                return false;
            var glyph2 = GlyphInfo.Glyphs[nextGlyphInLookup];
            ValueRecordTable valueRecordTable1;
            ValueRecordTable valueRecordTable2;
            switch (Format(Table))
            {
                case 1:
                    var glyphIndex = Coverage(Table).GetGlyphIndex(Table, glyph1);
                    if (glyphIndex == -1)
                        return false;
                    var pairSetTable = Format1PairSet(Table, (UInt16)glyphIndex);
                    var pairValue = pairSetTable.FindPairValue(Table, glyph2);
                    if (pairValue == -1)
                        return false;
                    valueRecordTable1 =
                        pairSetTable.FirstValueRecord(Table, (UInt16)pairValue, FirstValueFormat(Table));
                    valueRecordTable2 =
                        pairSetTable.SecondValueRecord(Table, (UInt16)pairValue, SecondValueFormat(Table));
                    break;
                case 2:
                    if (Coverage(Table).GetGlyphIndex(Table, glyph1) == -1)
                        return false;
                    var Class1Index = Format2Class1Table(Table).GetClass(Table, glyph1);
                    if (Class1Index >= Format2Class1Count(Table))
                        return false;
                    var Class2Index = Format2Class2Table(Table).GetClass(Table, glyph2);
                    if (Class2Index >= Format2Class2Count(Table))
                        return false;
                    var Class2Count = Format2Class2Count(Table);
                    valueRecordTable1 = Format2FirstValueRecord(Table, Class2Count, Class1Index, Class2Index);
                    valueRecordTable2 = Format2SecondValueRecord(Table, Class2Count, Class1Index, Class2Index);
                    break;
                default:
                    return false;
            }

            valueRecordTable1.AdjustPos(Table, Metrics, ref Offsets[FirstGlyph], ref Advances[FirstGlyph]);
            valueRecordTable2.AdjustPos(Table, Metrics, ref Offsets[nextGlyphInLookup],
                ref Advances[nextGlyphInLookup]);
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

        public PairPositioningSubtable(Int32 Offset)
        {
            offset = Offset;
        }

        private struct PairSetTable
        {
            private const Int32 offsetPairValueCount = 0;
            private const Int32 offsetPairValueArray = 2;
            private const Int32 offsetPairValueSecondGlyph = 0;
            private const Int32 offsetPairValueValue1 = 2;
            private readonly Int32 offset;
            private readonly UInt16 pairValueRecordSize;
            private readonly UInt16 secondValueRecordOffset;

            public UInt16 PairValueCount(FontTable Table)
            {
                return Table.GetUShort(offset);
            }

            public UInt16 PairValueGlyph(FontTable Table,
                                         UInt16 Index)
            {
                return Table.GetUShort(offset + 2 + Index * pairValueRecordSize);
            }

            public ValueRecordTable FirstValueRecord(FontTable Table,
                                                     UInt16 Index,
                                                     UInt16 Format)
            {
                return new ValueRecordTable(offset + 2 + Index * pairValueRecordSize + 2, offset, Format);
            }

            public ValueRecordTable SecondValueRecord(FontTable Table,
                                                      UInt16 Index,
                                                      UInt16 Format)
            {
                return new ValueRecordTable(offset + 2 + Index * pairValueRecordSize + secondValueRecordOffset, offset,
                    Format);
            }

            public Int32 FindPairValue(FontTable Table,
                                       UInt16 Glyph)
            {
                var num = PairValueCount(Table);
                for (UInt16 Index = 0; Index < num; ++Index)
                {
                    if (PairValueGlyph(Table, Index) == Glyph)
                        return Index;
                }

                return -1;
            }

            public PairSetTable(Int32 Offset,
                                UInt16 firstValueRecordSize,
                                UInt16 secondValueRecordSize)
            {
                secondValueRecordOffset = (UInt16)(2U + ValueRecordTable.Size(firstValueRecordSize));
                pairValueRecordSize =
                    (UInt16)(secondValueRecordOffset + (UInt32)ValueRecordTable.Size(secondValueRecordSize));
                offset = Offset;
            }
        }
    }
}
