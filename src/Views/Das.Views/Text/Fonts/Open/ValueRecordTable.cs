using System;
using System.Threading.Tasks;
using Das.Views.Layout;

namespace Das.Views.Text.Fonts.Open
{
    public struct ValueRecordTable
    {
        private const UInt16 XPlacmentFlag = 1;
        private const UInt16 YPlacmentFlag = 2;
        private const UInt16 XAdvanceFlag = 4;
        private const UInt16 YAdvanceFlag = 8;
        private const UInt16 XPlacementDeviceFlag = 16;
        private const UInt16 YPlacementDeviceFlag = 32;
        private const UInt16 XAdvanceDeviceFlag = 64;
        private const UInt16 YAdvanceDeviceFlag = 128;

        private static readonly UInt16[] BitCount = new UInt16[16]
        {
            0,
            2,
            2,
            4,
            2,
            4,
            4,
            6,
            2,
            4,
            4,
            6,
            4,
            6,
            6,
            8
        };

        private readonly UInt16 format;
        private readonly Int32 baseTableOffset;
        private readonly Int32 offset;

        public static UInt16 Size(UInt16 Format)
        {
            return (UInt16)(BitCount[Format & 15] + (UInt32)BitCount[(Format >> 4) & 15]);
        }

        public void AdjustPos(FontTable Table,
                              LayoutMetrics Metrics,
                              ref LayoutOffset GlyphOffset,
                              ref Int32 GlyphAdvance)
        {
            var offset1 = offset;
            if ((format & 1) != 0)
            {
                GlyphOffset.dx += Positioning.DesignToPixels(Metrics.DesignEmHeight, Metrics.PixelsEmWidth,
                    Table.GetShort(offset1));
                offset1 += 2;
            }

            if ((format & 2) != 0)
            {
                GlyphOffset.dy += Positioning.DesignToPixels(Metrics.DesignEmHeight, Metrics.PixelsEmHeight,
                    Table.GetShort(offset1));
                offset1 += 2;
            }

            if ((format & 4) != 0)
            {
                GlyphAdvance += Positioning.DesignToPixels(Metrics.DesignEmHeight, Metrics.PixelsEmWidth,
                    Table.GetShort(offset1));
                offset1 += 2;
            }

            if ((format & 8) != 0)
            {
                GlyphAdvance += Positioning.DesignToPixels(Metrics.DesignEmHeight, Metrics.PixelsEmHeight,
                    Table.GetShort(offset1));
                offset1 += 2;
            }

            if ((format & 16) != 0)
            {
                Int32 offset2 = Table.GetOffset(offset1);
                if (offset2 != 0)
                {
                    var deviceTable = new DeviceTable(baseTableOffset + offset2);
                    GlyphOffset.dx += deviceTable.Value(Table, Metrics.PixelsEmWidth);
                }

                offset1 += 2;
            }

            if ((format & 32) != 0)
            {
                Int32 offset3 = Table.GetOffset(offset1);
                if (offset3 != 0)
                {
                    var deviceTable = new DeviceTable(baseTableOffset + offset3);
                    GlyphOffset.dy += deviceTable.Value(Table, Metrics.PixelsEmHeight);
                }

                offset1 += 2;
            }

            if ((format & 64) != 0)
            {
                if (Metrics.Direction == TextFlowDirection.LTR || Metrics.Direction == TextFlowDirection.RTL)
                {
                    Int32 offset4 = Table.GetOffset(offset1);
                    if (offset4 != 0)
                    {
                        var deviceTable = new DeviceTable(baseTableOffset + offset4);
                        GlyphAdvance += deviceTable.Value(Table, Metrics.PixelsEmWidth);
                    }
                }

                offset1 += 2;
            }

            if ((format & 128) == 0)
                return;
            if (Metrics.Direction == TextFlowDirection.TTB || Metrics.Direction == TextFlowDirection.BTT)
            {
                Int32 offset5 = Table.GetOffset(offset1);
                if (offset5 != 0)
                {
                    var deviceTable = new DeviceTable(baseTableOffset + offset5);
                    GlyphAdvance += deviceTable.Value(Table, Metrics.PixelsEmHeight);
                }
            }

            var num = offset1 + 2;
        }

        public ValueRecordTable(Int32 Offset,
                                Int32 BaseTableOffset,
                                UInt16 Format)
        {
            offset = Offset;
            baseTableOffset = BaseTableOffset;
            format = Format;
        }
    }
}
