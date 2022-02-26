using System;
using System.Threading.Tasks;
using Das.Views.Layout;

namespace Das.Views.Text.Fonts.Open
{
    public struct ReverseChainingSubtable
    {
        private const Int32 offsetFormat = 0;
        private const Int32 offsetCoverage = 2;
        private const Int32 offsetBacktrackGlyphCount = 4;
        private const Int32 sizeCount = 2;
        private const Int32 sizeOffset = 2;
        private const Int32 sizeGlyphId = 2;
        private readonly Int32 offset;

        private UInt16 Format(FontTable Table)
        {
            return Table.GetUShort(offset);
        }

        private CoverageTable InputCoverage(FontTable Table)
        {
            return new CoverageTable(offset + Table.GetUShort(offset + 2));
        }

        private CoverageTable Coverage(FontTable Table,
                                       Int32 Offset)
        {
            return new CoverageTable(offset + Table.GetUShort(Offset));
        }

        private UInt16 GlyphCount(FontTable Table,
                                  Int32 Offset)
        {
            return Table.GetUShort(Offset);
        }

        private static UInt16 Glyph(FontTable Table,
                                    Int32 Offset)
        {
            return Table.GetUShort(Offset);
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
                                    out Int32 NextGlyph)
        {
            NextGlyph = AfterLastGlyph - 1;
            if (Format(Table) != 1)
                return false;
            var flag = true;
            var index1 = AfterLastGlyph - 1;
            var glyphIndex = InputCoverage(Table).GetGlyphIndex(Table, GlyphInfo.Glyphs[index1]);
            if (glyphIndex < 0)
                return false;
            var Offset1 = offset + 4;
            var num1 = GlyphCount(Table, Offset1);
            var Offset2 = Offset1 + 2;
            var index2 = index1;
            for (UInt16 index3 = 0; (index3 < num1) & flag; ++index3)
            {
                index2 = LayoutEngine.GetNextGlyphInLookup(Font, GlyphInfo, index2 - 1, LookupFlags, -1);
                if (index2 < 0)
                {
                    flag = false;
                }
                else
                {
                    flag = Coverage(Table, Offset2).GetGlyphIndex(Table, GlyphInfo.Glyphs[index2]) >= 0;
                    Offset2 += 2;
                }
            }

            var num2 = GlyphCount(Table, Offset2);
            var Offset3 = Offset2 + 2;
            var index4 = index1;
            for (UInt16 index5 = 0; (index5 < num2) & flag; ++index5)
            {
                index4 = LayoutEngine.GetNextGlyphInLookup(Font, GlyphInfo, index4 + 1, LookupFlags, 1);
                if (index4 >= GlyphInfo.Length)
                {
                    flag = false;
                }
                else
                {
                    flag = Coverage(Table, Offset3).GetGlyphIndex(Table, GlyphInfo.Glyphs[index4]) >= 0;
                    Offset3 += 2;
                }
            }

            if (flag)
            {
                var Offset4 = Offset3 + 2 + 2 * glyphIndex;
                GlyphInfo.Glyphs[index1] = Glyph(Table, Offset4);
                GlyphInfo.GlyphFlags[index1] = 23;
            }

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
            return InputCoverage(table);
        }

        public ReverseChainingSubtable(Int32 Offset)
        {
            offset = Offset;
        }
    }
}
