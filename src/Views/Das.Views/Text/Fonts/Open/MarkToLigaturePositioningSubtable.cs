using System;
using System.Threading.Tasks;
using Das.Views.Layout;
using Das.Views.Validation;

namespace Das.Views.Text.Fonts.Open
{
    internal struct MarkToLigaturePositioningSubtable
    {
        private const Int32 offsetFormat = 0;
        private const Int32 offsetMarkCoverage = 2;
        private const Int32 offsetLigatureCoverage = 4;
        private const Int32 offsetClassCount = 6;
        private const Int32 offsetMarkArray = 8;
        private const Int32 offsetLigatureArray = 10;
        private const Int32 offsetLigatureAttachArray = 2;
        private const Int32 sizeOffset = 2;
        private readonly Int32 offset;

        private UInt16 Format(FontTable Table)
        {
            return Table.GetUShort(offset);
        }

        private CoverageTable MarkCoverage(FontTable Table)
        {
            return new CoverageTable(offset + Table.GetUShort(offset + 2));
        }

        private CoverageTable LigatureCoverage(FontTable Table)
        {
            return new CoverageTable(offset + Table.GetUShort(offset + 4));
        }

        private UInt16 ClassCount(FontTable Table)
        {
            return Table.GetUShort(offset + 6);
        }

        private MarkArray Marks(FontTable Table)
        {
            return new MarkArray(offset + Table.GetUShort(offset + 8));
        }

        private LigatureAttachTable Ligatures(FontTable Table,
                                              Int32 Index,
                                              UInt16 ClassCount)
        {
            var num = offset + Table.GetUShort(offset + 10);
            return new LigatureAttachTable(num + Table.GetUShort(num + 2 + Index * 2), ClassCount);
        }

        private void FindBaseLigature(Int32 CharCount,
                                      UshortList Charmap,
                                      GlyphInfoList GlyphInfo,
                                      Int32 markGlyph,
                                      out UInt16 component,
                                      out Int32 ligatureGlyph)
        {
            var num1 = 0;
            ligatureGlyph = -1;
            component = 0;
            var flag = false;
            for (Int32 firstChar = GlyphInfo.FirstChars[markGlyph]; firstChar >= 0 && !flag; --firstChar)
            {
                var index = Charmap[firstChar];
                if ((GlyphInfo.GlyphFlags[index] & 7) != 3)
                {
                    num1 = firstChar;
                    ligatureGlyph = index;
                    flag = true;
                }
            }

            if (!flag)
                return;
            UInt16 num2 = 0;
            for (var firstChar = GlyphInfo.FirstChars[ligatureGlyph];
                 firstChar < CharCount && firstChar != num1;
                 ++firstChar)
            {
                if (Charmap[firstChar] == ligatureGlyph)
                    ++num2;
            }

            component = num2;
        }

        public unsafe Boolean Apply(IOpenTypeFont Font,
                                    FontTable Table,
                                    LayoutMetrics Metrics,
                                    GlyphInfoList GlyphInfo,
                                    UInt16 LookupFlags,
                                    Int32 CharCount,
                                    UshortList Charmap,
                                    Int32* Advances,
                                    LayoutOffset* Offsets,
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
            var num = FirstGlyph;
            if ((GlyphInfo.GlyphFlags[num] & 7) != 3)
                return false;
            var glyphIndex1 = MarkCoverage(Table).GetGlyphIndex(Table, GlyphInfo.Glyphs[num]);
            if (glyphIndex1 == -1)
                return false;
            UInt16 component;
            Int32 ligatureGlyph;
            FindBaseLigature(CharCount, Charmap, GlyphInfo, num, out component, out ligatureGlyph);
            if (ligatureGlyph < 0)
                return false;
            var glyphIndex2 = LigatureCoverage(Table).GetGlyphIndex(Table, GlyphInfo.Glyphs[ligatureGlyph]);
            if (glyphIndex2 == -1)
                return false;
            var ClassCount = this.ClassCount(Table);
            var markArray = Marks(Table);
            var MarkClass = markArray.Class(Table, (UInt16)glyphIndex1);
            if (MarkClass >= ClassCount)
                return false;
            var StaticAnchor = Ligatures(Table, glyphIndex2, ClassCount).LigatureAnchor(Table, component, MarkClass);
            if (StaticAnchor.IsNull())
                return false;
            var MobileAnchor = markArray.MarkAnchor(Table, (UInt16)glyphIndex1);
            if (MobileAnchor.IsNull())
                return false;
            Positioning.AlignAnchors(Font, Table, Metrics, GlyphInfo, Advances, Offsets, ligatureGlyph, num,
                StaticAnchor, MobileAnchor, false);
            return true;
        }

        public static Boolean IsLookupCovered(FontTable table,
                                              UInt32[] glyphBits,
                                              UInt16 minGlyphId,
                                              UInt16 maxGlyphId)
        {
            return false;
        }

        public CoverageTable GetPrimaryCoverage(FontTable table)
        {
            return MarkCoverage(table);
        }

        public MarkToLigaturePositioningSubtable(Int32 Offset)
        {
            offset = Offset;
        }
    }
}
