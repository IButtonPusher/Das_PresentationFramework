using System;
using System.Threading.Tasks;
using Das.Views.Layout;
using Das.Views.Validation;

namespace Das.Views.Text.Fonts.Open
{
    public struct MarkToMarkPositioningSubtable
    {
        private const Int32 offsetFormat = 0;
        private const Int32 offsetCoverage = 2;
        private const Int32 offsetMark2Coverage = 4;
        private const Int32 offsetClassCount = 6;
        private const Int32 offsetMark1Array = 8;
        private const Int32 offsetMark2Array = 10;
        private readonly Int32 offset;

        private UInt16 Format(FontTable Table)
        {
            return Table.GetUShort(offset);
        }

        private CoverageTable Mark1Coverage(FontTable Table)
        {
            return new CoverageTable(offset + Table.GetUShort(offset + 2));
        }

        private CoverageTable Mark2Coverage(FontTable Table)
        {
            return new CoverageTable(offset + Table.GetUShort(offset + 4));
        }

        private UInt16 Mark1ClassCount(FontTable Table)
        {
            return Table.GetUShort(offset + 6);
        }

        private MarkArray Mark1Array(FontTable Table)
        {
            return new MarkArray(offset + Table.GetUShort(offset + 8));
        }

        private Mark2Array Marks2(FontTable Table)
        {
            return new Mark2Array(offset + Table.GetUShort(offset + 10));
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
            if (Format(Table) != 1)
                return false;
            var length = GlyphInfo.Length;
            var num = FirstGlyph;
            if ((GlyphInfo.GlyphFlags[num] & 7) != 3)
                return false;
            var glyphIndex1 = Mark1Coverage(Table).GetGlyphIndex(Table, GlyphInfo.Glyphs[num]);
            if (glyphIndex1 == -1)
                return false;
            var nextGlyphInLookup =
                LayoutEngine.GetNextGlyphInLookup(Font, GlyphInfo, FirstGlyph - 1, (UInt16)(LookupFlags & 65280U), -1);
            if (nextGlyphInLookup < 0)
                return false;
            var glyphIndex2 = Mark2Coverage(Table).GetGlyphIndex(Table, GlyphInfo.Glyphs[nextGlyphInLookup]);
            if (glyphIndex2 == -1)
                return false;
            var Mark1ClassCount = this.Mark1ClassCount(Table);
            var markArray = Mark1Array(Table);
            var Mark1Class = markArray.Class(Table, (UInt16)glyphIndex1);
            if (Mark1Class >= Mark1ClassCount)
                return false;
            var MobileAnchor = markArray.MarkAnchor(Table, (UInt16)glyphIndex1);
            if (MobileAnchor.IsNull())
                return false;
            var StaticAnchor = Marks2(Table).Anchor(Table, (UInt16)glyphIndex2, Mark1ClassCount, Mark1Class);
            if (StaticAnchor.IsNull())
                return false;
            Positioning.AlignAnchors(Font, Table, Metrics, GlyphInfo, Advances, Offsets, nextGlyphInLookup, num,
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
            return Mark1Coverage(table);
        }

        public MarkToMarkPositioningSubtable(Int32 Offset)
        {
            offset = Offset;
        }

        private struct Mark2Array
        {
            private const Int32 offsetCount = 0;
            private const Int32 offsetAnchors = 2;
            private const Int32 sizeAnchorOffset = 2;
            private readonly Int32 offset;

            public AnchorTable Anchor(FontTable Table,
                                      UInt16 Mark2Index,
                                      UInt16 Mark1ClassCount,
                                      UInt16 Mark1Class)
            {
                Int32 num = Table.GetUShort(offset + 2 + (Mark2Index * Mark1ClassCount + Mark1Class) * 2);
                return num == 0 ? new AnchorTable(Table, 0) : new AnchorTable(Table, offset + num);
            }

            public Mark2Array(Int32 Offset)
            {
                offset = Offset;
            }
        }
    }
}
