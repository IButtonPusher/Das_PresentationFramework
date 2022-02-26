using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts.Open
{
    public struct LigatureAttachTable
    {
        private const Int32 offsetAnchorArray = 2;
        private const Int32 sizeAnchorOffset = 2;
        private readonly Int32 offset;
        private readonly Int32 classCount;

        public AnchorTable LigatureAnchor(FontTable Table,
                                          UInt16 Component,
                                          UInt16 MarkClass)
        {
            Int32 num = Table.GetUShort(offset + 2 + (Component * classCount + MarkClass) * 2);
            return num == 0 ? new AnchorTable(Table, 0) : new AnchorTable(Table, offset + num);
        }

        public LigatureAttachTable(Int32 Offset,
                                   UInt16 ClassCount)
        {
            offset = Offset;
            classCount = ClassCount;
        }
    }
}
