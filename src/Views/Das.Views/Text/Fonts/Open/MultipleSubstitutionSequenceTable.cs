using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts.Open
{
    public struct MultipleSubstitutionSequenceTable
    {
        private const Int32 offsetGlyphCount = 0;
        private const Int32 offsetGlyphArray = 2;
        private const Int32 sizeGlyphId = 2;
        private readonly Int32 offset;

        public UInt16 GlyphCount(FontTable Table)
        {
            return Table.GetUShort(offset);
        }

        public UInt16 Glyph(FontTable Table,
                            UInt16 index)
        {
            return Table.GetUShort(offset + 2 + index * 2);
        }

        public MultipleSubstitutionSequenceTable(Int32 Offset)
        {
            offset = Offset;
        }
    }
}
