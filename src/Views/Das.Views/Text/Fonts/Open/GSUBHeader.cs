using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts.Open
{
    public struct GSUBHeader
    {
        private const Int32 offsetScriptList = 4;
        private const Int32 offsetFeatureList = 6;
        private const Int32 offsetLookupList = 8;
        private readonly Int32 offset;

        public ScriptList GetScriptList(FontTable Table)
        {
            return new ScriptList(offset + Table.GetOffset(offset + 4));
        }

        public FeatureList GetFeatureList(FontTable Table)
        {
            return new FeatureList(offset + Table.GetOffset(offset + 6));
        }

        public LookupList GetLookupList(FontTable Table)
        {
            return new LookupList(offset + Table.GetOffset(offset + 8));
        }

        public GSUBHeader(Int32 Offset)
        {
            offset = Offset;
        }
    }
}
