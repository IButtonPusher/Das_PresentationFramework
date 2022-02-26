using System;
using System.Threading.Tasks;
using Das.Views.Layout;

namespace Das.Views.Text.Fonts.Open
{
    internal struct ContextSubtable
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
            switch (Format(Table))
            {
                case 1:
                    return new GlyphContextSubtable(offset).Apply(Font, TableTag, Table, Metrics, CharCount, Charmap,
                        GlyphInfo, Advances, Offsets, LookupFlags, FirstGlyph, AfterLastGlyph, Parameter, nestingLevel,
                        out NextGlyph);
                case 2:
                    return new ClassContextSubtable(offset).Apply(Font, TableTag, Table, Metrics, CharCount, Charmap,
                        GlyphInfo, Advances, Offsets, LookupFlags, FirstGlyph, AfterLastGlyph, Parameter, nestingLevel,
                        out NextGlyph);
                case 3:
                    return new CoverageContextSubtable(offset).Apply(Font, TableTag, Table, Metrics, CharCount, Charmap,
                        GlyphInfo, Advances, Offsets, LookupFlags, FirstGlyph, AfterLastGlyph, Parameter, nestingLevel,
                        out NextGlyph);
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
            switch (Format(table))
            {
                case 1:
                    return GlyphContextSubtable.IsLookupCovered(table, glyphBits, minGlyphId, maxGlyphId);
                case 2:
                    return ClassContextSubtable.IsLookupCovered(table, glyphBits, minGlyphId, maxGlyphId);
                case 3:
                    return CoverageContextSubtable.IsLookupCovered(table, glyphBits, minGlyphId,
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
                    return new GlyphContextSubtable(offset).GetPrimaryCoverage(table);
                case 2:
                    return new ClassContextSubtable(offset).GetPrimaryCoverage(table);
                case 3:
                    return new CoverageContextSubtable(offset).GetPrimaryCoverage(table);
                default:
                    return CoverageTable.InvalidCoverage;
            }
        }

        public ContextSubtable(Int32 Offset)
        {
            offset = Offset;
        }
    }
}
