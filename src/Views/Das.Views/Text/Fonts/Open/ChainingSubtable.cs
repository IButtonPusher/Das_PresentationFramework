using System;
using System.Threading.Tasks;
using Das.Views.Layout;

namespace Das.Views.Text.Fonts.Open
{
    public struct ChainingSubtable
    {
        private const Int32 offsetFormat = 0;
        private readonly Int32 offset;

        private UInt16 Format(FontTable Table)
        {
            return Table.GetUShort(offset);
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
            NextGlyph = FirstGlyph + 1;
            switch (Format(Table))
            {
                case 1:
                    return new GlyphChainingSubtable(offset).Apply(Font, TableTag, Table, Metrics, CharCount, Charmap,
                        GlyphInfo, Advances, Offsets, LookupFlags, FirstGlyph, AfterLastGlyph, Parameter, nestingLevel,
                        out NextGlyph);
                case 2:
                    return new ClassChainingSubtable(offset).Apply(Font, TableTag, Table, Metrics, CharCount, Charmap,
                        GlyphInfo, Advances, Offsets, LookupFlags, FirstGlyph, AfterLastGlyph, Parameter, nestingLevel,
                        out NextGlyph);
                case 3:
                    return new CoverageChainingSubtable(Table, offset).Apply(Font, TableTag, Table, Metrics, CharCount,
                        Charmap, GlyphInfo, Advances, Offsets, LookupFlags, FirstGlyph, AfterLastGlyph, Parameter,
                        nestingLevel, out NextGlyph);
                default:
                    return false;
            }
        }

        public Boolean IsLookupCovered(FontTable table,
                                       UInt32[] glyphBits,
                                       UInt16 minGlyphId,
                                       UInt16 maxGlyphId)
        {
            switch (Format(table))
            {
                case 1:
                    return new GlyphChainingSubtable(offset).IsLookupCovered(table, glyphBits, minGlyphId, maxGlyphId);
                case 2:
                    return ClassChainingSubtable.IsLookupCovered(table, glyphBits, minGlyphId, maxGlyphId);
                case 3:
                    return new CoverageChainingSubtable(table, offset).IsLookupCovered(table, glyphBits, minGlyphId,
                        maxGlyphId);
                default:
                    return true;
            }
        }

        public CoverageTable GetPrimaryCoverage(FontTable table)
        {
            switch (Format(table))
            {
                case 1:
                    return new GlyphChainingSubtable(offset).GetPrimaryCoverage(table);
                case 2:
                    return new ClassChainingSubtable(offset).GetPrimaryCoverage(table);
                case 3:
                    return new CoverageChainingSubtable(table, offset).GetPrimaryCoverage(table);
                default:
                    return CoverageTable.InvalidCoverage;
            }
        }

        public ChainingSubtable(Int32 Offset)
        {
            offset = Offset;
        }
    }
}
