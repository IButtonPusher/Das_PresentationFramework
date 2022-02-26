using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts.Open
{
    public enum OpenTypeLayoutResult
    {
        Success,
        InvalidParameter,
        TableNotFound,
        ScriptNotFound,
        LangSysNotFound,
        BadFontTable,
        UnderConstruction,
    }
}
