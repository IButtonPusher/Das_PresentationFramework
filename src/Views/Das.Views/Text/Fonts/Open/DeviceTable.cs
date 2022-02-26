using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts.Open
{
    public struct DeviceTable
    {
        private const Int32 offsetStartSize = 0;
        private const Int32 offsetEndSize = 2;
        private const Int32 offsetDeltaFormat = 4;
        private const Int32 offsetDeltaValueArray = 6;
        private const Int32 sizeDeltaValue = 2;
        private readonly Int32 offset;

        private UInt16 StartSize(FontTable Table)
        {
            return Table.GetUShort(offset);
        }

        private UInt16 EndSize(FontTable Table)
        {
            return Table.GetUShort(offset + 2);
        }

        private UInt16 DeltaFormat(FontTable Table)
        {
            return Table.GetUShort(offset + 4);
        }

        private UInt16 DeltaValue(FontTable Table,
                                  UInt16 Index)
        {
            return Table.GetUShort(offset + 6 + Index * 2);
        }

        public Int32 Value(FontTable Table,
                           UInt16 PixelsPerEm)
        {
            if (IsNull())
                return 0;
            var num1 = StartSize(Table);
            var num2 = EndSize(Table);
            if (PixelsPerEm < num1 || PixelsPerEm > num2)
                return 0;
            var num3 = (UInt16)(PixelsPerEm - (UInt32)num1);
            UInt16 Index;
            UInt16 num4;
            UInt16 num5;
            switch (DeltaFormat(Table))
            {
                case 1:
                    Index = (UInt16)((UInt32)num3 >> 3);
                    num4 = (UInt16)(16 + 2 * (num3 & 7));
                    num5 = 30;
                    break;
                case 2:
                    Index = (UInt16)((UInt32)num3 >> 2);
                    num4 = (UInt16)(16 + 4 * (num3 & 3));
                    num5 = 28;
                    break;
                case 3:
                    Index = (UInt16)((UInt32)num3 >> 1);
                    num4 = (UInt16)(16 + 8 * (num3 & 1));
                    num5 = 24;
                    break;
                default:
                    return 0;
            }

            return (DeltaValue(Table, Index) << num4) >> num5;
        }

        public DeviceTable(Int32 Offset)
        {
            offset = Offset;
        }

        private Boolean IsNull()
        {
            return offset == 0;
        }
    }
}
