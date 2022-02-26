using System;
using System.Threading.Tasks;
using Das.Views.Validation;

namespace Das.Views.Text.Fonts.Open
{
    public struct GDEFHeader
    {
        private const Int32 offsetGlyphClassDef = 4;
        private const Int32 offsetGlyphAttachList = 6;
        private const Int32 offsetLigaCaretList = 8;
        private const Int32 offsetMarkAttachClassDef = 10;
        private readonly Int32 offset;

        public ClassDefTable GetGlyphClassDef(FontTable Table)
        {
            Invariant.Assert(Table.IsPresent);
            return new ClassDefTable(offset + Table.GetOffset(offset + 4));
        }

        public ClassDefTable GetMarkAttachClassDef(FontTable Table)
        {
            Invariant.Assert(Table.IsPresent);
            return new ClassDefTable(offset + Table.GetOffset(offset + 10));
        }

        public GDEFHeader(Int32 Offset)
        {
            offset = Offset;
        }
    }
}
