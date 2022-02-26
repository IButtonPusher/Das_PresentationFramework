using System;
using System.Threading.Tasks;
using Das.Views.Layout;
using Das.Views.Validation;

namespace Das.Views.Text.Fonts.Open
{
    public struct CoverageChainingSubtable
    {
        private const Int32 offsetFormat = 0;
        private const Int32 offsetBacktrackGlyphCount = 2;
        private const Int32 offsetBacktrackCoverageArray = 4;
        private const Int32 sizeGlyphCount = 2;
        private const Int32 sizeCoverageOffset = 2;
        private readonly Int32 offset;
        private readonly Int32 offsetInputGlyphCount;
        private readonly Int32 offsetLookaheadGlyphCount;

        public UInt16 Format(FontTable Table)
        {
            return Table.GetUShort(offset);
        }

        public UInt16 BacktrackGlyphCount(FontTable Table)
        {
            return Table.GetUShort(offset + 2);
        }

        public CoverageTable BacktrackCoverage(FontTable Table,
                                               UInt16 Index)
        {
            return new CoverageTable(offset + Table.GetUShort(offset + 2 + 2 + Index * 2));
        }

        public UInt16 InputGlyphCount(FontTable Table)
        {
            return Table.GetUShort(offset + offsetInputGlyphCount);
        }

        public CoverageTable InputCoverage(FontTable Table,
                                           UInt16 Index)
        {
            return new CoverageTable(offset + Table.GetUShort(offset + offsetInputGlyphCount + 2 + Index * 2));
        }

        public UInt16 LookaheadGlyphCount(FontTable Table)
        {
            return Table.GetUShort(offset + offsetLookaheadGlyphCount);
        }

        public CoverageTable LookaheadCoverage(FontTable Table,
                                               UInt16 Index)
        {
            return new CoverageTable(offset + Table.GetUShort(offset + offsetLookaheadGlyphCount + 2 + Index * 2));
        }

        public ContextualLookupRecords ContextualLookups(FontTable Table)
        {
            var offset = this.offset + offsetLookaheadGlyphCount + 2 + LookaheadGlyphCount(Table) * 2;
            return new ContextualLookupRecords(offset + 2, Table.GetUShort(offset));
        }

        public CoverageChainingSubtable(FontTable Table,
                                        Int32 Offset)
        {
            offset = Offset;
            offsetInputGlyphCount = 4 + Table.GetUShort(offset + 2) * 2;
            offsetLookaheadGlyphCount = offsetInputGlyphCount + 2 + Table.GetUShort(offset + offsetInputGlyphCount) * 2;
        }

        public unsafe Boolean Apply(IOpenTypeFont Font,
                                    OpenTypeTags TableTag,
                                    FontTable Table,
                                    LayoutMetrics Metrics,
                                    Int32 CharCount,
                                    UshortList Charmap,
                                    GlyphInfoList GlyphInfo,
                                    Int32* Advances,
                                    LayoutOffset* Offsets,
                                    UInt16 LookupFlags,
                                    Int32 FirstGlyph,
                                    Int32 AfterLastGlyph,
                                    UInt32 Parameter,
                                    Int32 nestingLevel,
                                    out Int32 NextGlyph)
        {
            Invariant.Assert(Format(Table) == 3);
            NextGlyph = FirstGlyph + 1;
            var length = GlyphInfo.Length;
            var num1 = BacktrackGlyphCount(Table);
            var num2 = InputGlyphCount(Table);
            var num3 = LookaheadGlyphCount(Table);
            if (FirstGlyph < num1 || FirstGlyph + num2 > AfterLastGlyph)
                return false;
            var flag = true;
            var index1 = FirstGlyph;
            CoverageTable coverageTable;
            for (UInt16 Index = 0; (Index < num1) & flag; ++Index)
            {
                index1 = LayoutEngine.GetNextGlyphInLookup(Font, GlyphInfo, index1 - 1, LookupFlags, -1);
                if (index1 >= 0)
                {
                    coverageTable = BacktrackCoverage(Table, Index);
                    if (coverageTable.GetGlyphIndex(Table, GlyphInfo.Glyphs[index1]) >= 0)
                        continue;
                }

                flag = false;
            }

            if (!flag)
                return false;
            var index2 = FirstGlyph;
            for (UInt16 Index = 0; (Index < num2) & flag; ++Index)
            {
                if (index2 < AfterLastGlyph)
                {
                    coverageTable = InputCoverage(Table, Index);
                    if (coverageTable.GetGlyphIndex(Table, GlyphInfo.Glyphs[index2]) >= 0)
                    {
                        index2 = LayoutEngine.GetNextGlyphInLookup(Font, GlyphInfo, index2 + 1, LookupFlags, 1);
                        continue;
                    }
                }

                flag = false;
            }

            if (!flag)
                return false;
            var AfterLastGlyph1 = index2;
            for (UInt16 Index = 0; (Index < num3) & flag; ++Index)
            {
                if (index2 < GlyphInfo.Length)
                {
                    coverageTable = LookaheadCoverage(Table, Index);
                    if (coverageTable.GetGlyphIndex(Table, GlyphInfo.Glyphs[index2]) >= 0)
                    {
                        index2 = LayoutEngine.GetNextGlyphInLookup(Font, GlyphInfo, index2 + 1, LookupFlags, 1);
                        continue;
                    }
                }

                flag = false;
            }

            if (flag)
                ContextualLookups(Table)
                    .ApplyContextualLookups(Font, TableTag, Table, Metrics, CharCount, Charmap, GlyphInfo, Advances,
                        Offsets, LookupFlags, FirstGlyph, AfterLastGlyph1, Parameter, nestingLevel, out NextGlyph);
            return flag;
        }

        public Boolean IsLookupCovered(FontTable table,
                                       UInt32[] glyphBits,
                                       UInt16 minGlyphId,
                                       UInt16 maxGlyphId)
        {
            var num1 = BacktrackGlyphCount(table);
            var num2 = InputGlyphCount(table);
            var num3 = LookaheadGlyphCount(table);
            for (UInt16 Index = 0; Index < num1; ++Index)
            {
                if (!BacktrackCoverage(table, Index).IsAnyGlyphCovered(table, glyphBits, minGlyphId, maxGlyphId))
                    return false;
            }

            for (UInt16 Index = 0; Index < num2; ++Index)
            {
                if (!InputCoverage(table, Index).IsAnyGlyphCovered(table, glyphBits, minGlyphId, maxGlyphId))
                    return false;
            }

            for (UInt16 Index = 0; Index < num3; ++Index)
            {
                if (!LookaheadCoverage(table, Index).IsAnyGlyphCovered(table, glyphBits, minGlyphId, maxGlyphId))
                    return false;
            }

            return true;
        }

        public CoverageTable GetPrimaryCoverage(FontTable table)
        {
            return InputGlyphCount(table) > 0 ? InputCoverage(table, 0) : CoverageTable.InvalidCoverage;
        }
    }
}
