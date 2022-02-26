using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts.Open
{
    public struct MultipleSubstitutionSubtable
    {
        private const Int32 offsetFormat = 0;
        private const Int32 offsetCoverage = 2;
        private const Int32 offsetSequenceCount = 4;
        private const Int32 offsetSequenceArray = 6;
        private const Int32 sizeSequenceOffset = 2;
        private readonly Int32 offset;

        private UInt16 Format(FontTable Table)
        {
            return Table.GetUShort(offset);
        }

        private CoverageTable Coverage(FontTable Table)
        {
            return new CoverageTable(offset + Table.GetUShort(offset + 2));
        }

        private MultipleSubstitutionSequenceTable Sequence(FontTable Table,
                                                           Int32 Index)
        {
            return new MultipleSubstitutionSequenceTable(offset + Table.GetUShort(offset + 6 + Index * 2));
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
            NextGlyph = FirstGlyph + 1;
            if (Format(Table) != 1)
                return false;
            var length = GlyphInfo.Length;
            var glyph = GlyphInfo.Glyphs[FirstGlyph];
            var glyphIndex = Coverage(Table).GetGlyphIndex(Table, glyph);
            if (glyphIndex == -1)
                return false;
            var substitutionSequenceTable = Sequence(Table, glyphIndex);
            var num = substitutionSequenceTable.GlyphCount(Table);
            var Count = num - 1;
            if (num == 0)
            {
                GlyphInfo.Remove(FirstGlyph, 1);
            }
            else
            {
                var firstChar = GlyphInfo.FirstChars[FirstGlyph];
                var ligatureCount = GlyphInfo.LigatureCounts[FirstGlyph];
                if (Count > 0)
                    GlyphInfo.Insert(FirstGlyph, Count);
                for (UInt16 index = 0; index < num; ++index)
                {
                    GlyphInfo.Glyphs[FirstGlyph + index] = substitutionSequenceTable.Glyph(Table, index);
                    GlyphInfo.GlyphFlags[FirstGlyph + index] = 23;
                    GlyphInfo.FirstChars[FirstGlyph + index] = firstChar;
                    GlyphInfo.LigatureCounts[FirstGlyph + index] = ligatureCount;
                }
            }

            for (var index = 0; index < CharCount; ++index)
            {
                if (Charmap[index] > FirstGlyph)
                    Charmap[index] += (UInt16)Count;
            }

            NextGlyph = FirstGlyph + Count + 1;
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

        public MultipleSubstitutionSubtable(Int32 Offset)
        {
            offset = Offset;
        }
    }
}
