using System;
using System.Security;
using System.Threading.Tasks;
using Das.Views.Layout;
using Das.Views.Validation;

namespace Das.Views.Text.Fonts.Open
{
    [SecurityCritical(SecurityCriticalScope.Everything)]
    public struct CursivePositioningSubtable
    {
        private const UInt16 offsetFormat = 0;
        private const UInt16 offsetCoverage = 2;
        private const UInt16 offsetEntryExitCount = 4;
        private const UInt16 offsetEntryExitArray = 6;
        private const UInt16 sizeEntryExitRecord = 4;
        private const UInt16 offsetEntryAnchor = 0;
        private const UInt16 offsetExitAnchor = 2;

        private UInt16 Format(FontTable Table)
        {
            return Table.GetUShort(offset + offsetFormat);
        }

        private CoverageTable Coverage(FontTable Table)
        {
            return new CoverageTable(offset + Table.GetUShort(offset + offsetCoverage));
        }

        // Not used. This value should be equal to glyph count in Coverage.
        // Keeping it for future reference
        //private ushort EntryExitCount(FontTable Table)
        //{
        //    return Table.GetUShort(offsetEntryExitCount);
        //}        

        private AnchorTable EntryAnchor(FontTable Table,
                                        Int32 Index)
        {
            Int32 anchorTableOffset = Table.GetUShort(offset +
                                                      offsetEntryExitArray +
                                                      sizeEntryExitRecord * Index +
                                                      offsetEntryAnchor);
            if (anchorTableOffset == 0) return new AnchorTable(Table, 0);

            return new AnchorTable(Table, offset + anchorTableOffset);
        }

        private AnchorTable ExitAnchor(FontTable Table,
                                       Int32 Index)
        {
            Int32 anchorTableOffset = Table.GetUShort(offset +
                                                      offsetEntryExitArray +
                                                      sizeEntryExitRecord * Index +
                                                      offsetExitAnchor);
            if (anchorTableOffset == 0) return new AnchorTable(Table, 0);

            return new AnchorTable(Table, offset + anchorTableOffset);
        }

        public unsafe Boolean Apply(IOpenTypeFont Font,
                                    FontTable Table,
                                    LayoutMetrics Metrics, // LayoutMetrics
                                    GlyphInfoList GlyphInfo, // List of GlyphInfo structs
                                    UInt16 LookupFlags, // Lookup flags for glyph lookups
                                    Int32* Advances, // Glyph adv.widths
                                    LayoutOffset* Offsets, // Glyph offsets
                                    Int32 FirstGlyph, // where to apply lookup
                                    Int32 AfterLastGlyph, // how long is a context we can use
                                    out Int32 NextGlyph // Next glyph to process
        )
        {
            Invariant.Assert(FirstGlyph >= 0);
            Invariant.Assert(AfterLastGlyph <= GlyphInfo.Length);

            NextGlyph = FirstGlyph + 1;

            if (Format(Table) != 1) return false; // Unknown format            

            var RTL = (LookupFlags & LayoutEngine.LookupFlagRightToLeft) != 0;
            var cursiveBit = (UInt16)GlyphFlags.CursiveConnected;

            // 


            Int32 glyphIndex,
                prevGlyphIndex,
                coverageIndex,
                prevCoverageIndex;

            glyphIndex = LayoutEngine.GetNextGlyphInLookup(Font, GlyphInfo,
                FirstGlyph, LookupFlags,
                LayoutEngine.LookForward
            );

            //clear "CursiveConected" bit,
            //we will set it only if there is a connection to previous glyph
            if (RTL)
            {
                GlyphInfo.GlyphFlags[glyphIndex] &= (UInt16)~cursiveBit;
            }

            if (glyphIndex >= AfterLastGlyph)
            {
                return false;
            }

            prevGlyphIndex = LayoutEngine.GetNextGlyphInLookup(Font, GlyphInfo,
                FirstGlyph - 1, LookupFlags,
                LayoutEngine.LookBackward
            );
            if (prevGlyphIndex < 0)
            {
                return false;
            }

            var coverage = Coverage(Table);

            coverageIndex = coverage.GetGlyphIndex(Table, GlyphInfo.Glyphs[glyphIndex]);
            if (coverageIndex == -1)
            {
                return false;
            }

            prevCoverageIndex = coverage.GetGlyphIndex(Table, GlyphInfo.Glyphs[prevGlyphIndex]);
            if (prevCoverageIndex == -1)
            {
                return false;
            }

            AnchorTable prevExitAnchor, entryAnchor;

            prevExitAnchor = ExitAnchor(Table, prevCoverageIndex);
            if (prevExitAnchor.IsNull())
            {
                return false;
            }

            entryAnchor = EntryAnchor(Table, coverageIndex);
            if (entryAnchor.IsNull())
            {
                return false;
            }

            Positioning.AlignAnchors(Font, Table, Metrics,
                GlyphInfo, Advances, Offsets,
                prevGlyphIndex, glyphIndex,
                prevExitAnchor, entryAnchor, true);

            if (RTL)
            {
                var glyphFlags = GlyphInfo.GlyphFlags;

                Int32 index;

                //set "cursive" bit for everything up to prevGlyphIndex
                for (index = glyphIndex; index > prevGlyphIndex; index--)
                {
                    glyphFlags[index] |= cursiveBit;
                }

                //fix cursive dependencies
                var yCorrection = Offsets[glyphIndex].dy;
                for (index = glyphIndex;
                     (glyphFlags[index] & cursiveBit) != 0;
                     index--
                    )
                {
                    Offsets[index].dy -= yCorrection;
                }

                Invariant.Assert(glyphIndex >= 0); //First glyph should not have bit set
                Offsets[index].dy -= yCorrection;
            }

            return true;
        }

        public static Boolean IsLookupCovered(FontTable table,
                                              UInt32[] glyphBits,
                                              UInt16 minGlyphId,
                                              UInt16 maxGlyphId)
        {
            // 
            return true;
        }

        public CoverageTable GetPrimaryCoverage(FontTable table)
        {
            return Coverage(table);
        }

        public CursivePositioningSubtable(Int32 Offset)
        {
            offset = Offset;
        }

        private readonly Int32 offset;
    }
}
