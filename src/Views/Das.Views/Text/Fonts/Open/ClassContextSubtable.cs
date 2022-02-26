using System;
using System.Threading.Tasks;
using Das.Views.Layout;
using Das.Views.Validation;

namespace Das.Views.Text.Fonts.Open
{
    public struct ClassContextSubtable
    {
        private const Int32 offsetFormat = 0;
        private const Int32 offsetCoverage = 2;
        private const Int32 offsetClassDef = 4;
        private const Int32 offsetSubClassSetCount = 6;
        private const Int32 offsetSubClassSetArray = 8;
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

        private ClassDefTable ClassDef(FontTable Table)
        {
            return new ClassDefTable(offset + Table.GetUShort(offset + 4));
        }

        private UInt16 ClassSetCount(FontTable Table)
        {
            return Table.GetUShort(offset + 6);
        }

        private SubClassSet ClassSet(FontTable Table,
                                     UInt16 Index)
        {
            Int32 num = Table.GetUShort(offset + 8 + Index * 2);
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
            var ClassDef = this.ClassDef(Table);
            var Index1 = ClassDef.GetClass(Table, glyph);
            if (Index1 >= ClassSetCount(Table))
                return false;
            var subClassSet = ClassSet(Table, Index1);
            if (subClassSet.IsNull)
                return false;
            var num = subClassSet.RuleCount(Table);
            var flag = false;
            for (UInt16 Index2 = 0; !flag && Index2 < num; ++Index2)
                flag = subClassSet.Rule(Table, Index2)
                                  .Apply(Font, TableTag, Table, Metrics, ClassDef, CharCount, Charmap, GlyphInfo,
                                      Advances, Offsets, LookupFlags, FirstGlyph, AfterLastGlyph, Parameter,
                                      nestingLevel, out NextGlyph);
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

        public ClassContextSubtable(Int32 Offset)
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

            public UInt16 GlyphCount(FontTable Table)
            {
                return Table.GetUShort(offset);
            }

            public UInt16 ClassId(FontTable Table,
                                  Int32 Index)
            {
                return Table.GetUShort(offset + 4 + (Index - 1) * 2);
            }

            public UInt16 SubstCount(FontTable Table)
            {
                return Table.GetUShort(offset + 2);
            }

            public ContextualLookupRecords ContextualLookups(FontTable Table)
            {
                return new ContextualLookupRecords(offset + 4 + (GlyphCount(Table) - 1) * 2, SubstCount(Table));
            }

            public unsafe Boolean Apply(IOpenTypeFont Font,
                                        OpenTypeTags TableTag,
                                        FontTable Table,
                                        LayoutMetrics Metrics,
                                        ClassDefTable ClassDef,
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
                NextGlyph = FirstGlyph + 1;
                var flag = true;
                var index = FirstGlyph;
                Int32 num1 = GlyphCount(Table);
                for (UInt16 Index = 1; (Index < num1) & flag; ++Index)
                {
                    index = LayoutEngine.GetNextGlyphInLookup(Font, GlyphInfo, index + 1, LookupFlags, 1);
                    if (index >= AfterLastGlyph)
                    {
                        flag = false;
                    }
                    else
                    {
                        var num2 = ClassId(Table, Index);
                        flag = ClassDef.GetClass(Table, GlyphInfo.Glyphs[index]) == num2;
                    }
                }

                if (flag)
                    ContextualLookups(Table)
                        .ApplyContextualLookups(Font, TableTag, Table, Metrics, CharCount, Charmap, GlyphInfo, Advances,
                            Offsets, LookupFlags, FirstGlyph, index + 1, Parameter, nestingLevel, out NextGlyph);
                return flag;
            }

            private const Int32 offsetGlyphCount = 0;
            private const Int32 offsetSubstCount = 2;
            private const Int32 offsetInputSequence = 4;
            private const Int32 sizeCount = 2;
            private const Int32 sizeClassId = 2;
            private readonly Int32 offset;
        }
    }
}
