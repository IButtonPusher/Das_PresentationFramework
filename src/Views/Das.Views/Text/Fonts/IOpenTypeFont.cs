using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts
{
    public interface IOpenTypeFont
    {
        FontTable GetFontTable(OpenTypeTags TableTag);

        LayoutOffset GetGlyphPointCoord(UInt16 Glyph,
                                        UInt16 PointIndex);

        Byte[] GetTableCache(OpenTypeTags tableTag);

        Byte[] AllocateTableCache(OpenTypeTags tableTag,
                                  Int32 size);
    }
}
