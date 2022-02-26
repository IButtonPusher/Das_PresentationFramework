using System;
using System.Threading.Tasks;
using Das.Views.Layout;
using Das.Views.Validation;

namespace Das.Views.Text.Fonts.Open
{
    public struct AnchorTable
    {
        private const Int32 offsetFormat = 0;
        private const Int32 offsetXCoordinate = 2;
        private const Int32 offsetYCoordinate = 4;
        private const Int32 offsetFormat2AnchorPoint = 6;
        private const Int32 offsetFormat3XDeviceTable = 6;
        private const Int32 offsetFormat3YDeviceTable = 8;
        private readonly Int32 offset;
        private readonly UInt16 format;

        private Int16 XCoordinate(FontTable Table)
        {
            return Table.GetShort(offset + 2);
        }

        private Int16 YCoordinate(FontTable Table)
        {
            return Table.GetShort(offset + 4);
        }

        private UInt16 Format2AnchorPoint(FontTable Table)
        {
            Invariant.Assert(format == 2);
            return Table.GetUShort(offset + 6);
        }

        private DeviceTable Format3XDeviceTable(FontTable Table)
        {
            Invariant.Assert(format == 3);
            Int32 num = Table.GetUShort(offset + 6);
            return num != 0 ? new DeviceTable(offset + num) : new DeviceTable(0);
        }

        private DeviceTable Format3YDeviceTable(FontTable Table)
        {
            Invariant.Assert(format == 3);
            Int32 num = Table.GetUShort(offset + 8);
            return num != 0 ? new DeviceTable(offset + num) : new DeviceTable(0);
        }

        public Boolean NeedContourPoint(FontTable Table)
        {
            return format == 2;
        }

        public UInt16 ContourPointIndex(FontTable Table)
        {
            Invariant.Assert(NeedContourPoint(Table));
            return Format2AnchorPoint(Table);
        }

        public LayoutOffset AnchorCoordinates(FontTable Table,
                                              LayoutMetrics Metrics,
                                              LayoutOffset ContourPoint)
        {
            var layoutOffset = new LayoutOffset();
            switch (format)
            {
                case 1:
                    layoutOffset.dx = Positioning.DesignToPixels(Metrics.DesignEmHeight, Metrics.PixelsEmWidth,
                        XCoordinate(Table));
                    layoutOffset.dy = Positioning.DesignToPixels(Metrics.DesignEmHeight, Metrics.PixelsEmHeight,
                        YCoordinate(Table));
                    break;
                case 2:
                    if (ContourPoint.dx == Int32.MinValue)
                    {
                        layoutOffset.dx = Positioning.DesignToPixels(Metrics.DesignEmHeight, Metrics.PixelsEmWidth,
                            XCoordinate(Table));
                        layoutOffset.dy = Positioning.DesignToPixels(Metrics.DesignEmHeight, Metrics.PixelsEmHeight,
                            YCoordinate(Table));
                        break;
                    }

                    layoutOffset.dx =
                        Positioning.DesignToPixels(Metrics.DesignEmHeight, Metrics.PixelsEmWidth, ContourPoint.dx);
                    layoutOffset.dy =
                        Positioning.DesignToPixels(Metrics.DesignEmHeight, Metrics.PixelsEmWidth, ContourPoint.dy);
                    break;
                case 3:
                    layoutOffset.dx =
                        Positioning.DesignToPixels(Metrics.DesignEmHeight, Metrics.PixelsEmWidth, XCoordinate(Table)) +
                        Format3XDeviceTable(Table).Value(Table, Metrics.PixelsEmWidth);
                    layoutOffset.dy =
                        Positioning.DesignToPixels(Metrics.DesignEmHeight, Metrics.PixelsEmHeight, YCoordinate(Table)) +
                        Format3YDeviceTable(Table).Value(Table, Metrics.PixelsEmHeight);
                    break;
                default:
                    layoutOffset.dx = 0;
                    layoutOffset.dx = 0;
                    break;
            }

            return layoutOffset;
        }

        public AnchorTable(FontTable Table,
                           Int32 Offset)
        {
            offset = Offset;
            if (offset != 0)
                format = Table.GetUShort(offset);
            else
                format = 0;
        }

        public Boolean IsNull()
        {
            return offset == 0;
        }
    }
}
