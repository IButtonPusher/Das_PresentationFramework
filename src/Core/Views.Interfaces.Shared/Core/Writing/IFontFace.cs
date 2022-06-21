using System;
using System.Threading.Tasks;
using Das.Views.Text;

namespace Das.Views.Core.Writing
{
    public interface IFontFace
    {
        void Release();

        unsafe void GetArrayOfGlyphIndices(UInt32* pCodePoints,
                                           UInt32 glyphCount,
                                           UInt16* pGlyphIndices);

        Boolean ReadFontEmbeddingRights(out UInt16 fsType);

        Boolean TryGetFontTable(OpenTypeTableTag openTypeTableTag,
                                out Byte[] tableData);

        FontFaceType Type { get; }

        UInt16 GlyphCount { get; }
    }
}
