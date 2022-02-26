using System;
using System.Threading.Tasks;
using Das.Views.Layout;
using Das.Views.Validation;

namespace Das.Views.Text.Fonts.Open
{
    internal struct GlyphChainingSubtable
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

        public Boolean IsLookupCovered(FontTable table,
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

        public GlyphChainingSubtable(Int32 Offset)
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

            public static UInt16 GlyphCount(FontTable Table,
                                            Int32 Offset)
            {
                return Table.GetUShort(Offset);
            }

            public static UInt16 GlyphId(FontTable Table,
                                         Int32 Offset)
            {
                return Table.GetUShort(Offset);
            }

            public ContextualLookupRecords ContextualLookups(FontTable Table,
                                                             Int32 CurrentOffset)
            {
                return new ContextualLookupRecords(CurrentOffset + 2, Table.GetUShort(CurrentOffset));
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
                var offset = this.offset;
                Int32 num1 = GlyphCount(Table, offset);
                var Offset1 = offset + 2;
                var index1 = FirstGlyph;
                for (UInt16 index2 = 0; (index2 < num1) & flag; ++index2)
                {
                    index1 = LayoutEngine.GetNextGlyphInLookup(Font, GlyphInfo, index1 - 1, LookupFlags, -1);
                    if (index1 < 0)
                    {
                        flag = false;
                    }
                    else
                    {
                        flag = GlyphId(Table, Offset1) == GlyphInfo.Glyphs[index1];
                        Offset1 += 2;
                    }
                }

                if (!flag)
                    return false;
                Int32 num2 = GlyphCount(Table, Offset1);
                var Offset2 = Offset1 + 2;
                var index3 = FirstGlyph;
                for (UInt16 index4 = 1; (index4 < num2) & flag; ++index4)
                {
                    index3 = LayoutEngine.GetNextGlyphInLookup(Font, GlyphInfo, index3 + 1, LookupFlags, 1);
                    if (index3 >= AfterLastGlyph)
                    {
                        flag = false;
                    }
                    else
                    {
                        flag = GlyphId(Table, Offset2) == GlyphInfo.Glyphs[index3];
                        Offset2 += 2;
                    }
                }

                if (!flag)
                    return false;
                var AfterLastGlyph1 = index3 + 1;
                Int32 num3 = GlyphCount(Table, Offset2);
                var num4 = Offset2 + 2;
                for (UInt16 index5 = 0; (index5 < num3) & flag; ++index5)
                {
                    index3 = LayoutEngine.GetNextGlyphInLookup(Font, GlyphInfo, index3 + 1, LookupFlags, 1);
                    if (index3 >= GlyphInfo.Length)
                    {
                        flag = false;
                    }
                    else
                    {
                        flag = GlyphId(Table, num4) == GlyphInfo.Glyphs[index3];
                        num4 += 2;
                    }
                }

                if (flag)
                    ContextualLookups(Table, num4)
                        .ApplyContextualLookups(Font, TableTag, Table, Metrics, CharCount, Charmap, GlyphInfo, Advances,
                            Offsets, LookupFlags, FirstGlyph, AfterLastGlyph1, Parameter, nestingLevel, out NextGlyph);
                return flag;
            }

            private const Int32 sizeCount = 2;
            private const Int32 sizeGlyphId = 2;
            private readonly Int32 offset;
        }
    }
}
