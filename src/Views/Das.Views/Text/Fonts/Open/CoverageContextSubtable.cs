using System;
using System.Threading.Tasks;
using Das.Views.Layout;
using Das.Views.Validation;

namespace Das.Views.Text.Fonts.Open
{
    public struct CoverageContextSubtable
    {
        private const Int32 offsetFormat = 0;
        private const Int32 offsetGlyphCount = 2;
        private const Int32 offsetSubstCount = 4;
        private const Int32 offsetInputCoverage = 6;
        private const Int32 sizeOffset = 2;
        private readonly Int32 offset;

        private UInt16 Format(FontTable Table)
        {
            return Table.GetUShort(offset);
        }

        private UInt16 GlyphCount(FontTable Table)
        {
            return Table.GetUShort(offset + 2);
        }

        private UInt16 SubstCount(FontTable Table)
        {
            return Table.GetUShort(offset + 4);
        }

        private CoverageTable InputCoverage(FontTable Table,
                                            UInt16 index)
        {
            return new CoverageTable(offset + Table.GetUShort(offset + 6 + index * 2));
        }

        public ContextualLookupRecords ContextualLookups(FontTable Table)
        {
            return new ContextualLookupRecords(offset + 6 + GlyphCount(Table) * 2, SubstCount(Table));
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
            var flag = true;
            Int32 num1 = GlyphCount(Table);
            var num2 = FirstGlyph;
            for (UInt16 index = 0; (index < num1) & flag; ++index)
            {
                if (num2 >= AfterLastGlyph ||
                    InputCoverage(Table, index).GetGlyphIndex(Table, GlyphInfo.Glyphs[num2]) < 0)
                    flag = false;
                else
                    num2 = LayoutEngine.GetNextGlyphInLookup(Font, GlyphInfo, num2 + 1, LookupFlags, 1);
            }

            if (flag)
                ContextualLookups(Table)
                    .ApplyContextualLookups(Font, TableTag, Table, Metrics, CharCount, Charmap, GlyphInfo, Advances,
                        Offsets, LookupFlags, FirstGlyph, num2, Parameter, nestingLevel, out NextGlyph);
            return flag;
        }

        public static Boolean IsLookupCovered(FontTable table,
                                              UInt32[] glyphBits,
                                              UInt16 minGlyphId,
                                              UInt16 maxGlyphId)
        {
            return true;
        }

        public CoverageTable GetPrimaryCoverage(FontTable table)
        {
            return GlyphCount(table) > 0 ? InputCoverage(table, 0) : CoverageTable.InvalidCoverage;
        }

        public CoverageContextSubtable(Int32 Offset)
        {
            offset = Offset;
        }
    }
}
