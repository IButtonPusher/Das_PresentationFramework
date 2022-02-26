using System;
using System.Threading.Tasks;
using Das.Views.Validation;

namespace Das.Views.Text.Fonts.Open
{
    public struct AlternateSubstitutionSubtable
    {
        private const Int32 offsetFormat = 0;
        private const Int32 offsetCoverage = 2;
        private const Int32 offsetAlternateSetCount = 4;
        private const Int32 offsetAlternateSets = 6;
        private const Int32 sizeAlternateSetOffset = 2;
        private const UInt16 InvalidAlternateGlyph = 65535;
        private readonly Int32 offset;

        public UInt16 Format(FontTable Table)
        {
            return Table.GetUShort(offset);
        }

        private CoverageTable Coverage(FontTable Table)
        {
            return new CoverageTable(offset + Table.GetUShort(offset + 2));
        }

        private AlternateSetTable AlternateSet(FontTable Table,
                                               Int32 index)
        {
            return new AlternateSetTable(offset + Table.GetUShort(offset + 6 + index * 2));
        }

        public Boolean Apply(FontTable Table,
                             GlyphInfoList GlyphInfo,
                             UInt32 FeatureParam,
                             Int32 FirstGlyph,
                             out Int32 NextGlyph)
        {
            NextGlyph = FirstGlyph + 1;
            if (Format(Table) != 1)
                return false;
            var length = GlyphInfo.Length;
            var glyphIndex = Coverage(Table).GetGlyphIndex(Table, GlyphInfo.Glyphs[FirstGlyph]);
            if (glyphIndex == -1)
                return false;
            var num = AlternateSet(Table, glyphIndex).Alternate(Table, FeatureParam);
            if (num == UInt16.MaxValue)
                return false;
            GlyphInfo.Glyphs[FirstGlyph] = num;
            GlyphInfo.GlyphFlags[FirstGlyph] = 23;
            return true;
        }

        public Boolean IsLookupCovered(FontTable table,
                                       UInt32[] glyphBits,
                                       UInt16 minGlyphId,
                                       UInt16 maxGlyphId)
        {
            return Coverage(table).IsAnyGlyphCovered(table, glyphBits, minGlyphId, maxGlyphId);
        }

        public CoverageTable GetPrimaryCoverage(FontTable table)
        {
            return Coverage(table);
        }

        public AlternateSubstitutionSubtable(Int32 Offset)
        {
            offset = Offset;
        }

        private struct AlternateSetTable
        {
            private const Int32 offsetGlyphCount = 0;
            private const Int32 offsetGlyphs = 2;
            private const Int32 sizeGlyph = 2;
            private readonly Int32 offset;

            public UInt16 GlyphCount(FontTable Table)
            {
                return Table.GetUShort(offset);
            }

            public UInt16 Alternate(FontTable Table,
                                    UInt32 FeatureParam)
            {
                Invariant.Assert(FeatureParam > 0U);
                var num = FeatureParam - 1U;
                return num >= GlyphCount(Table) ? UInt16.MaxValue : Table.GetUShort(offset + 2 + (UInt16)num * 2);
            }

            public AlternateSetTable(Int32 Offset)
            {
                offset = Offset;
            }
        }
    }
}
