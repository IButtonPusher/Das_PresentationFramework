using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts
{
    public struct ScriptTable
    {
        private const Int32 offsetDefaultLangSys = 0;
        private const Int32 offsetLangSysCount = 2;
        private const Int32 offsetLangSysRecordArray = 4;
        private const Int32 sizeLangSysRecord = 6;
        private const Int32 offsetLangSysRecordTag = 0;
        private const Int32 offsetLangSysRecordOffset = 4;
        private readonly Int32 offset;

        public LangSysTable FindLangSys(FontTable Table,
                                        UInt32 Tag)
        {
            if (IsNull)
                return new LangSysTable(Int32.MaxValue);
            if (Tag == 1684434036U)
                return IsDefaultLangSysExists(Table)
                    ? new LangSysTable(offset + Table.GetOffset(offset))
                    : new LangSysTable(Int32.MaxValue);
            for (UInt16 Index = 0; Index < GetLangSysCount(Table); ++Index)
            {
                if ((Int32)GetLangSysTag(Table, Index) == (Int32)Tag)
                    return GetLangSysTable(Table, Index);
            }

            return new LangSysTable(Int32.MaxValue);
        }

        public Boolean IsDefaultLangSysExists(FontTable Table)
        {
            return Table.GetOffset(offset) > 0;
        }

        public LangSysTable GetDefaultLangSysTable(FontTable Table)
        {
            return IsDefaultLangSysExists(Table)
                ? new LangSysTable(offset + Table.GetOffset(offset))
                : new LangSysTable(Int32.MaxValue);
        }

        public UInt16 GetLangSysCount(FontTable Table)
        {
            return Table.GetUShort(offset + 2);
        }

        public UInt32 GetLangSysTag(FontTable Table,
                                    UInt16 Index)
        {
            return Table.GetUInt(offset + 4 + Index * 6);
        }

        public LangSysTable GetLangSysTable(FontTable Table,
                                            UInt16 Index)
        {
            return new LangSysTable(offset + Table.GetOffset(offset + 4 + Index * 6 + 4));
        }

        public ScriptTable(Int32 Offset)
        {
            offset = Offset;
        }

        public Boolean IsNull => offset == Int32.MaxValue;
    }
}
