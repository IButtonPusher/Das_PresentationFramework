using System;
using System.Threading.Tasks;
using Das.Views.Layout;

namespace Das.Views.Text.Fonts.Open
{
    public struct ContextualLookupRecords
    {
        private const Int32 offsetSequenceIndex = 0;
        private const Int32 offsetLookupIndex = 2;
        private const Int32 sizeLookupRecord = 4;
        private const Int32 MaximumContextualLookupNestingLevel = 16;
        private readonly Int32 offset;
        private readonly UInt16 recordCount;

        private UInt16 SequenceIndex(FontTable Table,
                                     UInt16 Index)
        {
            return Table.GetUShort(offset + Index * 4);
        }

        private UInt16 LookupIndex(FontTable Table,
                                   UInt16 Index)
        {
            return Table.GetUShort(offset + Index * 4 + 2);
        }

        public unsafe void ApplyContextualLookups(IOpenTypeFont Font,
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
                                                  out Int32 nextGlyph)
        {
            if (nestingLevel >= 16)
            {
                nextGlyph = AfterLastGlyph;
            }
            else
            {
                var lookupList = TableTag != OpenTypeTags.GSUB
                    ? new GPOSHeader(0).GetLookupList(Table)
                    : new GSUBHeader(0).GetLookupList(Table);
                var num1 = -1;
                var num2 = -1;
                while (true)
                {
                    UInt16 Index1;
                    Int32 FirstGlyph1;
                    do
                    {
                        Index1 = UInt16.MaxValue;
                        var num3 = UInt16.MaxValue;
                        for (UInt16 Index2 = 0; Index2 < recordCount; ++Index2)
                        {
                            var num4 = LookupIndex(Table, Index2);
                            var num5 = SequenceIndex(Table, Index2);
                            if (num4 >= num1 && (num4 != num1 || num5 > num2) &&
                                (num4 < Index1 || num4 == Index1 && num5 < num3))
                            {
                                Index1 = num4;
                                num3 = num5;
                            }
                        }

                        if (Index1 != UInt16.MaxValue)
                        {
                            num1 = Index1;
                            num2 = num3;
                            FirstGlyph1 = FirstGlyph;
                            for (var index = 0; index < num3 && FirstGlyph1 < AfterLastGlyph; ++index)
                                FirstGlyph1 = LayoutEngine.GetNextGlyphInLookup(Font, GlyphInfo, FirstGlyph1 + 1,
                                    LookupFlags, 1);
                        }
                        else
                            goto label_14;
                    } while (FirstGlyph1 >= AfterLastGlyph);

                    var length = GlyphInfo.Length;
                    LayoutEngine.ApplyLookup(Font, TableTag, Table, Metrics, lookupList.Lookup(Table, Index1),
                        CharCount, Charmap, GlyphInfo, Advances, Offsets, FirstGlyph1, AfterLastGlyph, Parameter,
                        nestingLevel + 1, out var _);
                    AfterLastGlyph += GlyphInfo.Length - length;
                }

                label_14:
                nextGlyph = AfterLastGlyph;
            }
        }

        public ContextualLookupRecords(Int32 Offset,
                                       UInt16 RecordCount)
        {
            offset = Offset;
            recordCount = RecordCount;
        }
    }
}
