using System;
using System.Security;
using System.Threading.Tasks;
using Das.Views.Layout;
using Das.Views.Validation;

namespace Das.Views.Text.Fonts.Open
{
    /// <SecurityNote>
    ///     Critical - Everything in this struct is considered critical
    ///     because they either operate on raw font table bits or unsafe pointers.
    /// </SecurityNote>
    [SecurityCritical(SecurityCriticalScope.Everything)]
    public struct MarkToBasePositioningSubtable
    {
        private const Int32 offsetFormat = 0;
        private const Int32 offsetCoverage = 2;
        private const Int32 offsetBaseCoverage = 4;
        private const Int32 offsetClassCount = 6;
        private const Int32 offsetMarkArray = 8;
        private const Int32 offsetBaseArray = 10;

        private UInt16 Format(FontTable Table)
        {
            return Table.GetUShort(offset + offsetFormat);
        }

        private CoverageTable MarkCoverage(FontTable Table)
        {
            return new CoverageTable(offset + Table.GetUShort(offset + offsetCoverage));
        }

        private CoverageTable BaseCoverage(FontTable Table)
        {
            return new CoverageTable(offset + Table.GetUShort(offset +
                                                              offsetBaseCoverage));
        }

        private UInt16 ClassCount(FontTable Table)
        {
            return Table.GetUShort(offset + offsetClassCount);
        }

        private MarkArray Marks(FontTable Table)
        {
            return new MarkArray(offset + Table.GetUShort(offset + offsetMarkArray));
        }

        private BaseArray Bases(FontTable Table)
        {
            return new BaseArray(offset + Table.GetUShort(offset + offsetBaseArray));
        }

        #region Mark to base positioning child structures

        /// <SecurityNote>
        ///     Critical - Everything in this struct is considered critical
        ///     because they either operate on raw font table bits or unsafe pointers.
        /// </SecurityNote>
        [SecurityCritical(SecurityCriticalScope.Everything)]
        private struct BaseArray
        {
            private const Int32 offsetAnchorArray = 2;
            private const Int32 sizeAnchorOffset = 2;

            public AnchorTable BaseAnchor(FontTable Table,
                                          UInt16 BaseIndex,
                                          UInt16 MarkClassCount,
                                          UInt16 MarkClass)
            {
                Int32 anchorTableOffset = Table.GetUShort(offset + offsetAnchorArray +
                                                          (BaseIndex * MarkClassCount + MarkClass) *
                                                          sizeAnchorOffset
                );
                if (anchorTableOffset == 0)
                {
                    return new AnchorTable(Table, 0);
                }

                return new AnchorTable(Table, offset + anchorTableOffset);
            }

            public BaseArray(Int32 Offset)
            {
                offset = Offset;
            }

            private readonly Int32 offset;
        }

        #endregion

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

            NextGlyph = FirstGlyph + 1; //Always move to the next glyph, whether matched or not

            if (Format(Table) != 1) return false; //unknown format

            var glyphCount = GlyphInfo.Length;

            var markGlyph = FirstGlyph;

            //Lookup works with marks only
            if ((GlyphInfo.GlyphFlags[markGlyph] & (UInt16)GlyphFlags.GlyphTypeMask) !=
                (UInt16)GlyphFlags.Mark) return false;

            var markCoverageIndex = MarkCoverage(Table).GetGlyphIndex(Table, GlyphInfo.Glyphs[markGlyph]);
            if (markCoverageIndex == -1) return false;

            //Find preceeding base (precisely, not mark ). Uses special lookup flag
            var baseGlyph = LayoutEngine.GetNextGlyphInLookup(Font,
                GlyphInfo,
                FirstGlyph - 1,
                LayoutEngine.LookupFlagFindBase,
                LayoutEngine.LookBackward);
            if (baseGlyph < 0) return false;

            var baseCoverageIndex = BaseCoverage(Table).GetGlyphIndex(Table, GlyphInfo.Glyphs[baseGlyph]);
            if (baseCoverageIndex == -1) return false;

            var classCount = ClassCount(Table);
            var marks = Marks(Table);

            var markClass = marks.Class(Table, (UInt16)markCoverageIndex);
            if (markClass >= classCount) return false; //Invalid mark class

            var markAnchor = marks.MarkAnchor(Table, (UInt16)markCoverageIndex);
            if (markAnchor.IsNull())
            {
                return false;
            }

            var baseAnchor = Bases(Table).BaseAnchor(Table, (UInt16)baseCoverageIndex, classCount, markClass);
            if (baseAnchor.IsNull())
            {
                return false;
            }

            Positioning.AlignAnchors(Font, Table, Metrics, GlyphInfo, Advances, Offsets,
                baseGlyph, markGlyph, baseAnchor, markAnchor, false);
            return true;
        }

        public static Boolean IsLookupCovered(FontTable table,
                                              UInt32[] glyphBits,
                                              UInt16 minGlyphId,
                                              UInt16 maxGlyphId)
        {
            // 
            return false;
        }

        public CoverageTable GetPrimaryCoverage(FontTable table)
        {
            return MarkCoverage(table);
        }

        public MarkToBasePositioningSubtable(Int32 Offset)
        {
            offset = Offset;
        }

        private readonly Int32 offset;
    }
}
