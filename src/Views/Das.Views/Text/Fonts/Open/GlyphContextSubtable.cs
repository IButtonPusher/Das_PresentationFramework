using System;
using System.Threading.Tasks;
using Das.Views.Layout;
using Das.Views.Validation;

namespace Das.Views.Text.Fonts.Open
{
    public struct GlyphContextSubtable
    {
        private const Int32 offsetFormat = 0;
        private const Int32 offsetCoverage = 2;
        private const Int32 offsetSubRuleSetCount = 4;
        private const Int32 offsetSubRuleSetArray = 6;
        private const Int32 sizeRuleSetOffset = 2;
        private readonly Int32 offset;

        public UInt16 Format(FontTable Table)
        {
            return Table.GetUShort(offset);
        }

        private CoverageTable Coverage(FontTable Table)
        {
            return new CoverageTable(offset + Table.GetUShort(offset + 2));
        }

        private SubRuleSet RuleSet(FontTable Table,
                                   Int32 Index)
        {
            return new SubRuleSet(offset + Table.GetUShort(offset + 6 + Index * 2));
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
            Invariant.Assert(Format(Table) == 1);
            NextGlyph = FirstGlyph + 1;
            var length = GlyphInfo.Length;
            var index = FirstGlyph;
            var glyph = GlyphInfo.Glyphs[index];
            var glyphIndex = Coverage(Table).GetGlyphIndex(Table, glyph);
            if (glyphIndex < 0)
                return false;
            var subRuleSet = RuleSet(Table, glyphIndex);
            var num = subRuleSet.RuleCount(Table);
            var flag = false;
            for (UInt16 Index = 0; !flag && Index < num; ++Index)
                flag = subRuleSet.Rule(Table, Index)
                                 .Apply(Font, TableTag, Table, Metrics, CharCount, Charmap, GlyphInfo, Advances,
                                     Offsets, LookupFlags, FirstGlyph, AfterLastGlyph, Parameter, nestingLevel,
                                     out NextGlyph);
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
            return Coverage(table);
        }

        public GlyphContextSubtable(Int32 Offset)
        {
            offset = Offset;
        }

        private class SubRuleSet
        {
            public SubRuleSet(Int32 Offset)
            {
                offset = Offset;
            }

            public UInt16 RuleCount(FontTable Table)
            {
                return Table.GetUShort(offset);
            }

            public SubRule Rule(FontTable Table,
                                UInt16 Index)
            {
                return new SubRule(offset + Table.GetUShort(offset + 2 + Index * 2));
            }

            private const Int32 offsetRuleCount = 0;
            private const Int32 offsetRuleArray = 2;
            private const Int32 sizeRuleOffset = 2;
            private readonly Int32 offset;
        }

        private class SubRule
        {
            public SubRule(Int32 Offset)
            {
                offset = Offset;
            }

            public UInt16 GlyphCount(FontTable Table)
            {
                return Table.GetUShort(offset);
            }

            public UInt16 SubstCount(FontTable Table)
            {
                return Table.GetUShort(offset + 2);
            }

            public UInt16 GlyphId(FontTable Table,
                                  Int32 Index)
            {
                return Table.GetUShort(offset + 4 + (Index - 1) * 2);
            }

            public ContextualLookupRecords ContextualLookups(FontTable Table)
            {
                return new ContextualLookupRecords(offset + 4 + (GlyphCount(Table) - 1) * 2, SubstCount(Table));
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
                var flag = true;
                NextGlyph = FirstGlyph + 1;
                Int32 num = GlyphCount(Table);
                var index = FirstGlyph;
                for (UInt16 Index = 1; (Index < num) & flag; ++Index)
                {
                    index = LayoutEngine.GetNextGlyphInLookup(Font, GlyphInfo, index + 1, LookupFlags, 1);
                    flag = index < AfterLastGlyph && GlyphId(Table, Index) == GlyphInfo.Glyphs[index];
                }

                if (flag)
                    ContextualLookups(Table)
                        .ApplyContextualLookups(Font, TableTag, Table, Metrics, CharCount, Charmap, GlyphInfo, Advances,
                            Offsets, LookupFlags, FirstGlyph, index + 1, Parameter, nestingLevel, out NextGlyph);
                return flag;
            }

            private const Int32 offsetGlyphCount = 0;
            private const Int32 offsetSubstCount = 2;
            private const Int32 offsetInput = 4;
            private const Int32 sizeCount = 2;
            private const Int32 sizeGlyphId = 2;
            private readonly Int32 offset;
        }
    }
}
