using System;
using System.Threading.Tasks;
using Das.Views.Layout;
using Das.Views.Validation;

namespace Das.Views.Text.Fonts.Open
{
    public struct LigatureSubstitutionSubtable
    {
        private const Int32 offsetFormat = 0;
        private const Int32 offsetCoverage = 2;
        private const Int32 offsetLigatureSetCount = 4;
        private const Int32 offsetLigatureSetArray = 6;
        private const Int32 sizeLigatureSet = 2;
        private readonly Int32 offset;

        private UInt16 Format(FontTable Table)
        {
            return Table.GetUShort(offset);
        }

        private CoverageTable Coverage(FontTable Table)
        {
            return new CoverageTable(offset + Table.GetUShort(offset + 2));
        }

        private UInt16 LigatureSetCount(FontTable Table)
        {
            return Table.GetUShort(offset + 4);
        }

        private LigatureSetTable LigatureSet(FontTable Table,
                                             UInt16 Index)
        {
            return new LigatureSetTable(offset + Table.GetUShort(offset + 6 + Index * 2));
        }

        public Boolean Apply(IOpenTypeFont Font,
                             FontTable Table,
                             Int32 CharCount,
                             UshortList Charmap,
                             GlyphInfoList GlyphInfo,
                             UInt16 LookupFlags,
                             Int32 FirstGlyph,
                             Int32 AfterLastGlyph,
                             out Int32 NextGlyph)
        {
            Invariant.Assert(FirstGlyph >= 0);
            Invariant.Assert(AfterLastGlyph <= GlyphInfo.Length);
            NextGlyph = FirstGlyph + 1;
            if (Format(Table) != 1)
                return false;
            var length = GlyphInfo.Length;
            var glyph = GlyphInfo.Glyphs[FirstGlyph];
            var glyphIndex = Coverage(Table).GetGlyphIndex(Table, glyph);
            if (glyphIndex == -1)
                return false;
            UInt16 num1 = 0;
            var flag = false;
            UInt16 num2 = 0;
            var ligatureSetTable = LigatureSet(Table, (UInt16)glyphIndex);
            var num3 = ligatureSetTable.LigatureCount(Table);
            for (UInt16 Index1 = 0; Index1 < num3; ++Index1)
            {
                var ligatureTable = ligatureSetTable.Ligature(Table, Index1);
                num2 = ligatureTable.ComponentCount(Table);
                if (num2 == 0)
                    throw new FormatException();
                var index = FirstGlyph;
                UInt16 Index2;
                for (Index2 = 1; Index2 < num2; ++Index2)
                {
                    index = LayoutEngine.GetNextGlyphInLookup(Font, GlyphInfo, index + 1, LookupFlags, 1);
                    if (index >= AfterLastGlyph || GlyphInfo.Glyphs[index] != ligatureTable.Component(Table, Index2))
                        break;
                }

                if (Index2 == num2)
                {
                    flag = true;
                    num1 = ligatureTable.LigatureGlyph(Table);
                    break;
                }
            }

            if (flag)
            {
                var num4 = 0;
                var num5 = Int32.MaxValue;
                var index1 = FirstGlyph;
                for (UInt16 index2 = 0; index2 < num2; ++index2)
                {
                    Invariant.Assert(index1 < AfterLastGlyph);
                    Int32 firstChar = GlyphInfo.FirstChars[index1];
                    Int32 ligatureCount = GlyphInfo.LigatureCounts[index1];
                    num4 += ligatureCount;
                    if (firstChar < num5)
                        num5 = firstChar;
                    index1 = LayoutEngine.GetNextGlyphInLookup(Font, GlyphInfo, index1 + 1, LookupFlags, 1);
                }

                var num6 = FirstGlyph;
                var num7 = FirstGlyph;
                UInt16 num8 = 0;
                for (UInt16 index3 = 1; index3 <= num2; ++index3)
                {
                    num7 = num6;
                    num6 = index3 >= num2
                        ? GlyphInfo.Length
                        : LayoutEngine.GetNextGlyphInLookup(Font, GlyphInfo, num6 + 1, LookupFlags, 1);
                    for (var index4 = 0; index4 < CharCount; ++index4)
                    {
                        if (Charmap[index4] == num7)
                            Charmap[index4] = (UInt16)FirstGlyph;
                    }

                    if (num8 > 0)
                    {
                        for (var index5 = num7 + 1; index5 < num6; ++index5)
                        {
                            GlyphInfo.Glyphs[index5 - num8] = GlyphInfo.Glyphs[index5];
                            GlyphInfo.GlyphFlags[index5 - num8] = GlyphInfo.GlyphFlags[index5];
                            GlyphInfo.FirstChars[index5 - num8] = GlyphInfo.FirstChars[index5];
                            GlyphInfo.LigatureCounts[index5 - num8] = GlyphInfo.LigatureCounts[index5];
                        }

                        if (num6 - num7 > 1)
                        {
                            for (var index6 = 0; index6 < CharCount; ++index6)
                            {
                                var num9 = Charmap[index6];
                                if (num9 > num7 && num9 < num6)
                                    Charmap[index6] -= num8;
                            }
                        }
                    }

                    ++num8;
                }

                GlyphInfo.Glyphs[FirstGlyph] = num1;
                GlyphInfo.GlyphFlags[FirstGlyph] = 23;
                GlyphInfo.FirstChars[FirstGlyph] = (UInt16)num5;
                GlyphInfo.LigatureCounts[FirstGlyph] = (UInt16)num4;
                if (num2 > 1)
                    GlyphInfo.Remove(GlyphInfo.Length - num2 + 1, num2 - 1);
                NextGlyph = num7 - (num2 - 1) + 1;
            }

            return flag;
        }

        public Boolean IsLookupCovered(FontTable table,
                                       UInt32[] glyphBits,
                                       UInt16 minGlyphId,
                                       UInt16 maxGlyphId)
        {
            if (!Coverage(table).IsAnyGlyphCovered(table, glyphBits, minGlyphId, maxGlyphId))
                return false;
            var num1 = LigatureSetCount(table);
            for (UInt16 Index1 = 0; Index1 < num1; ++Index1)
            {
                var ligatureSetTable = LigatureSet(table, Index1);
                var num2 = ligatureSetTable.LigatureCount(table);
                for (UInt16 Index2 = 0; Index2 < num2; ++Index2)
                {
                    var ligatureTable = ligatureSetTable.Ligature(table, Index2);
                    var num3 = ligatureTable.ComponentCount(table);
                    var flag = true;
                    for (UInt16 Index3 = 1; Index3 < num3; ++Index3)
                    {
                        var num4 = ligatureTable.Component(table, Index3);
                        if (num4 > maxGlyphId || num4 < minGlyphId || (glyphBits[num4 >> 5] & 1 << (num4 % 32)) == 0L)
                        {
                            flag = false;
                            break;
                        }
                    }

                    if (flag)
                        return true;
                }
            }

            return false;
        }

        public CoverageTable GetPrimaryCoverage(FontTable table)
        {
            return Coverage(table);
        }

        public LigatureSubstitutionSubtable(Int32 Offset)
        {
            offset = Offset;
        }

        private struct LigatureSetTable
        {
            private const Int32 offsetLigatureCount = 0;
            private const Int32 offsetLigatureArray = 2;
            private const Int32 sizeLigatureOffset = 2;
            private readonly Int32 offset;

            public UInt16 LigatureCount(FontTable Table)
            {
                return Table.GetUShort(offset);
            }

            public LigatureTable Ligature(FontTable Table,
                                          UInt16 Index)
            {
                return new LigatureTable(offset + Table.GetUShort(offset + 2 + Index * 2));
            }

            public LigatureSetTable(Int32 Offset)
            {
                offset = Offset;
            }
        }

        private struct LigatureTable
        {
            private const Int32 offsetLigatureGlyph = 0;
            private const Int32 offsetComponentCount = 2;
            private const Int32 offsetComponentArray = 4;
            private const Int32 sizeComponent = 2;
            private readonly Int32 offset;

            public UInt16 LigatureGlyph(FontTable Table)
            {
                return Table.GetUShort(offset);
            }

            public UInt16 ComponentCount(FontTable Table)
            {
                return Table.GetUShort(offset + 2);
            }

            public UInt16 Component(FontTable Table,
                                    UInt16 Index)
            {
                return Table.GetUShort(offset + 4 + (Index - 1) * 2);
            }

            public LigatureTable(Int32 Offset)
            {
                offset = Offset;
            }
        }
    }
}
