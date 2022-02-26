using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts.Open
{
    public struct ScriptList
    {
        private const Int32 offsetScriptCount = 0;
        private const Int32 offsetScriptRecordArray = 2;
        private const Int32 sizeScriptRecord = 6;
        private const Int32 offsetScriptRecordTag = 0;
        private const Int32 offsetScriptRecordOffset = 4;
        private readonly Int32 offset;

        public ScriptTable FindScript(FontTable Table,
                                      UInt32 Tag)
        {
            for (UInt16 Index = 0; Index < GetScriptCount(Table); ++Index)
            {
                if ((Int32)GetScriptTag(Table, Index) == (Int32)Tag)
                    return GetScriptTable(Table, Index);
            }

            return new ScriptTable(Int32.MaxValue);
        }

        public UInt16 GetScriptCount(FontTable Table)
        {
            return Table.GetUShort(offset);
        }

        public UInt32 GetScriptTag(FontTable Table,
                                   UInt16 Index)
        {
            return Table.GetUInt(offset + 2 + Index * 6);
        }

        public ScriptTable GetScriptTable(FontTable Table,
                                          UInt16 Index)
        {
            return new ScriptTable(offset + Table.GetOffset(offset + 2 + Index * 6 + 4));
        }

        public ScriptList(Int32 Offset)
        {
            offset = Offset;
        }
    }
}
