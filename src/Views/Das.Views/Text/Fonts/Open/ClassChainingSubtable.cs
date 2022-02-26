using System;
using System.Threading.Tasks;
using Das.Views.Layout;
using Das.Views.Validation;

namespace Das.Views.Text.Fonts.Open
{
    public struct ClassChainingSubtable
    {
        private const Int32 offsetFormat = 0;
        private const Int32 offsetCoverage = 2;
        private const Int32 offsetBacktrackClassDef = 4;
        private const Int32 offsetInputClassDef = 6;
        private const Int32 offsetLookaheadClassDef = 8;
        private const Int32 offsetSubClassSetCount = 10;
        private const Int32 offsetSubClassSetArray = 12;
        private const Int32 sizeClassSetOffset = 2;
        private readonly Int32 offset;

        public UInt16 Format(FontTable Table)
        {
            return Table.GetUShort(offset);
        }

        private CoverageTable Coverage(FontTable Table)
        {
            return new CoverageTable(offset + Table.GetUShort(offset + 2));
        }

        private ClassDefTable BacktrackClassDef(FontTable Table)
        {
            return new ClassDefTable(offset + Table.GetUShort(offset + 4));
        }

        private ClassDefTable InputClassDef(FontTable Table)
        {
            return new ClassDefTable(offset + Table.GetUShort(offset + 6));
        }

        private ClassDefTable LookaheadClassDef(FontTable Table)
        {
            return new ClassDefTable(offset + Table.GetUShort(offset + 8));
        }

        private UInt16 ClassSetCount(FontTable Table)
        {
            return Table.GetUShort(offset + 10);
        }

        private SubClassSet ClassSet(FontTable Table,
                                     UInt16 Index)
        {
            Int32 num = Table.GetUShort(offset + 12 + Index * 2);
            return num == 0 ? new SubClassSet(Int32.MaxValue) : new SubClassSet(offset + num);
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
            Invariant.Assert(Format(Table) == 2);
            NextGlyph = FirstGlyph + 1;
            var length = GlyphInfo.Length;
            var index = FirstGlyph;
            var glyph = GlyphInfo.Glyphs[index];
            if (Coverage(Table).GetGlyphIndex(Table, glyph) < 0)
                return false;
            var inputClassDef = InputClassDef(Table);
            var backtrackClassDef = BacktrackClassDef(Table);
            var lookaheadClassDef = LookaheadClassDef(Table);
            var Index1 = inputClassDef.GetClass(Table, glyph);
            if (Index1 >= ClassSetCount(Table))
                return false;
            var subClassSet = ClassSet(Table, Index1);
            if (subClassSet.IsNull)
                return false;
            var num = subClassSet.RuleCount(Table);
            var flag = false;
            for (UInt16 Index2 = 0; !flag && Index2 < num; ++Index2)
                flag = subClassSet.Rule(Table, Index2)
                                  .Apply(Font, TableTag, Table, Metrics, inputClassDef, backtrackClassDef,
                                      lookaheadClassDef, CharCount, Charmap, GlyphInfo, Advances, Offsets, LookupFlags,
                                      FirstGlyph, AfterLastGlyph, Parameter, nestingLevel, out NextGlyph);
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

        public ClassChainingSubtable(Int32 Offset)
        {
            offset = Offset;
        }

        private class SubClassSet
        {
            public SubClassSet(Int32 Offset)
            {
                offset = Offset;
            }

            public UInt16 RuleCount(FontTable Table)
            {
                return Table.GetUShort(offset);
            }

            public SubClassRule Rule(FontTable Table,
                                     UInt16 Index)
            {
                return new SubClassRule(offset + Table.GetUShort(offset + 2 + Index * 2));
            }

            public Boolean IsNull => offset == Int32.MaxValue;

            private const Int32 offsetRuleCount = 0;
            private const Int32 offsetRuleArray = 2;
            private const Int32 sizeRuleOffset = 2;
            private readonly Int32 offset;
        }

        private class SubClassRule
        {
            public SubClassRule(Int32 Offset)
            {
                offset = Offset;
            }

            public static UInt16 GlyphCount(FontTable Table,
                                            Int32 Offset)
            {
                return Table.GetUShort(Offset);
            }

            public static UInt16 ClassId(FontTable Table,
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
                                        ClassDefTable inputClassDef,
                                        ClassDefTable backtrackClassDef,
                                        ClassDefTable lookaheadClassDef,
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
                        var num2 = ClassId(Table, Offset1);
                        Offset1 += 2;
                        flag = backtrackClassDef.GetClass(Table, GlyphInfo.Glyphs[index1]) == num2;
                    }
                }

                if (!flag)
                    return false;
                Int32 num3 = GlyphCount(Table, Offset1);
                var Offset2 = Offset1 + 2;
                var index3 = FirstGlyph;
                for (UInt16 index4 = 1; (index4 < num3) & flag; ++index4)
                {
                    index3 = LayoutEngine.GetNextGlyphInLookup(Font, GlyphInfo, index3 + 1, LookupFlags, 1);
                    if (index3 >= AfterLastGlyph)
                    {
                        flag = false;
                    }
                    else
                    {
                        var num4 = ClassId(Table, Offset2);
                        Offset2 += 2;
                        flag = inputClassDef.GetClass(Table, GlyphInfo.Glyphs[index3]) == num4;
                    }
                }

                if (!flag)
                    return false;
                var AfterLastGlyph1 = index3 + 1;
                Int32 num5 = GlyphCount(Table, Offset2);
                var num6 = Offset2 + 2;
                for (UInt16 index5 = 0; (index5 < num5) & flag; ++index5)
                {
                    index3 = LayoutEngine.GetNextGlyphInLookup(Font, GlyphInfo, index3 + 1, LookupFlags, 1);
                    if (index3 >= GlyphInfo.Length)
                    {
                        flag = false;
                    }
                    else
                    {
                        var num7 = ClassId(Table, num6);
                        num6 += 2;
                        flag = lookaheadClassDef.GetClass(Table, GlyphInfo.Glyphs[index3]) == num7;
                    }
                }

                if (flag)
                    ContextualLookups(Table, num6)
                        .ApplyContextualLookups(Font, TableTag, Table, Metrics, CharCount, Charmap, GlyphInfo, Advances,
                            Offsets, LookupFlags, FirstGlyph, AfterLastGlyph1, Parameter, nestingLevel, out NextGlyph);
                return flag;
            }

            private const Int32 sizeCount = 2;
            private const Int32 sizeClassId = 2;
            private readonly Int32 offset;
        }
    }
}
