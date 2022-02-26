using System;
using System.Threading.Tasks;
using Das.Views.Validation;

namespace Das.Views.Text.Fonts.Open
{
    public struct SingleSubstitutionSubtable
    {
        private const Int32 offsetFormat = 0;
        private const Int32 offsetCoverage = 2;
        private const Int32 offsetFormat1DeltaGlyphId = 4;
        private const Int32 offsetFormat2GlyphCount = 4;
        private const Int32 offsetFormat2SubstitutehArray = 6;
        private const Int32 sizeFormat2SubstituteSize = 2;
        private readonly Int32 offset;

        private UInt16 Format(FontTable Table)
        {
            return Table.GetUShort(offset);
        }

        private CoverageTable Coverage(FontTable Table)
        {
            return new CoverageTable(offset + Table.GetUShort(offset + 2));
        }

        private Int16 Format1DeltaGlyphId(FontTable Table)
        {
            Invariant.Assert(Format(Table) == 1);
            return Table.GetShort(offset + 4);
        }

        private UInt16 Format2SubstituteGlyphId(FontTable Table,
                                                UInt16 Index)
        {
            Invariant.Assert(Format(Table) == 2);
            return Table.GetUShort(offset + 6 + Index * 2);
        }

        public Boolean Apply(FontTable Table,
                             GlyphInfoList GlyphInfo,
                             Int32 FirstGlyph,
                             out Int32 NextGlyph)
        {
            Invariant.Assert(FirstGlyph >= 0);
            NextGlyph = FirstGlyph + 1;
            var glyph = GlyphInfo.Glyphs[FirstGlyph];
            var glyphIndex = Coverage(Table).GetGlyphIndex(Table, glyph);
            if (glyphIndex == -1)
                return false;
            switch (Format(Table))
            {
                case 1:
                    GlyphInfo.Glyphs[FirstGlyph] = (UInt16)(glyph + (UInt32)Format1DeltaGlyphId(Table));
                    GlyphInfo.GlyphFlags[FirstGlyph] = 23;
                    NextGlyph = FirstGlyph + 1;
                    return true;
                case 2:
                    GlyphInfo.Glyphs[FirstGlyph] = Format2SubstituteGlyphId(Table, (UInt16)glyphIndex);
                    GlyphInfo.GlyphFlags[FirstGlyph] = 23;
                    NextGlyph = FirstGlyph + 1;
                    return true;
                default:
                    NextGlyph = FirstGlyph + 1;
                    return false;
            }
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

        public SingleSubstitutionSubtable(Int32 Offset)
        {
            offset = Offset;
        }
    }
}
