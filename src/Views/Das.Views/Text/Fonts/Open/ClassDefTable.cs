using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts.Open
{
    public struct ClassDefTable
    {
        private const Int32 offsetFormat = 0;
        private const Int32 offsetFormat1StartGlyph = 2;
        private const Int32 offsetFormat1GlyphCount = 4;
        private const Int32 offsetFormat1ClassValueArray = 6;
        private const Int32 sizeFormat1ClassValue = 2;
        private const Int32 offsetFormat2RangeCount = 2;
        private const Int32 offsetFormat2RangeRecordArray = 4;
        private const Int32 sizeFormat2RangeRecord = 6;
        private const Int32 offsetFormat2RangeRecordStart = 0;
        private const Int32 offsetFormat2RangeRecordEnd = 2;
        private const Int32 offsetFormat2RangeRecordClass = 4;
        private readonly Int32 offset;

        private UInt16 Format(FontTable Table)
        {
            return Table.GetUShort(offset);
        }

        private UInt16 Format1StartGlyph(FontTable Table)
        {
            return Table.GetUShort(offset + 2);
        }

        private UInt16 Format1GlyphCount(FontTable Table)
        {
            return Table.GetUShort(offset + 4);
        }

        private UInt16 Format1ClassValue(FontTable Table,
                                         UInt16 Index)
        {
            return Table.GetUShort(offset + 6 + Index * 2);
        }

        private UInt16 Format2RangeCount(FontTable Table)
        {
            return Table.GetUShort(offset + 2);
        }

        private UInt16 Format2RangeStartGlyph(FontTable Table,
                                              UInt16 Index)
        {
            return Table.GetUShort(offset + 4 + Index * 6);
        }

        private UInt16 Format2RangeEndGlyph(FontTable Table,
                                            UInt16 Index)
        {
            return Table.GetUShort(offset + 4 + Index * 6 + 2);
        }

        private UInt16 Format2RangeClassValue(FontTable Table,
                                              UInt16 Index)
        {
            return Table.GetUShort(offset + 4 + Index * 6 + 4);
        }

        public UInt16 GetClass(FontTable Table,
                               UInt16 glyph)
        {
            switch (Format(Table))
            {
                case 1:
                    var num1 = Format1StartGlyph(Table);
                    var num2 = Format1GlyphCount(Table);
                    return glyph >= num1 && glyph - num1 < num2
                        ? Format1ClassValue(Table, (UInt16)(glyph - (UInt32)num1))
                        : (UInt16)0;
                case 2:
                    UInt16 num3 = 0;
                    var num4 = Format2RangeCount(Table);
                    while (num3 < num4)
                    {
                        var Index = (UInt16)((num3 + num4) >> 1);
                        if (glyph < Format2RangeStartGlyph(Table, Index))
                        {
                            num4 = Index;
                        }
                        else
                        {
                            if (glyph <= Format2RangeEndGlyph(Table, Index))
                                return Format2RangeClassValue(Table, Index);
                            num3 = (UInt16)(Index + 1U);
                        }
                    }

                    return 0;
                default:
                    return 0;
            }
        }

        public static ClassDefTable InvalidClassDef => new ClassDefTable(-1);

        public Boolean IsInvalid => offset == -1;

        public ClassDefTable(Int32 Offset)
        {
            offset = Offset;
        }
    }
}
