using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts
{
    public sealed class GsubGposTables : IOpenTypeFont
    {
        internal GsubGposTables(FontFaceLayoutInfo layout)
        {
            _layout = layout;
            _gsubTable = new FontTable(_layout.Gsub());
            _gposTable = new FontTable(_layout.Gpos());
        }

        public FontTable GetFontTable(OpenTypeTags TableTag)
        {
            if (TableTag == OpenTypeTags.GPOS)
                return _gposTable;
            if (TableTag == OpenTypeTags.GSUB)
                return _gsubTable;
            throw new NotSupportedException();
        }

        public LayoutOffset GetGlyphPointCoord(UInt16 Glyph,
                                               UInt16 PointIndex)
        {
            throw new NotSupportedException();
        }

        public Byte[] GetTableCache(OpenTypeTags tableTag)
        {
            return _layout.GetTableCache(tableTag);
        }

        public Byte[] AllocateTableCache(OpenTypeTags tableTag,
                                         Int32 size)
        {
            return _layout.AllocateTableCache(tableTag, size);
        }

        private readonly FontTable _gposTable;
        private readonly FontTable _gsubTable;
        private readonly FontFaceLayoutInfo _layout;
    }
}
