using System;
using System.Threading.Tasks;
using Das.Views.Text;

namespace Das.Views.Core.Writing
{
    public class FontFace : IFontFace
    {
        public unsafe void GetArrayOfGlyphIndices(UInt32* pCodePoints,
                                                  UInt32 glyphCount,
                                                  UInt16* pGlyphIndices)
        {
            throw new NotImplementedException();
        }

        public Boolean ReadFontEmbeddingRights(out UInt16 fsType)
        {
            throw new NotImplementedException();
        }

        public Boolean TryGetFontTable(OpenTypeTableTag openTypeTableTag,
                                       out Byte[] tableData)
        {
            throw new NotImplementedException();
        }

        public FontFaceType Type => throw new NotImplementedException();

        public UInt16 GlyphCount => throw new NotImplementedException();

        public void Release()
        {
            throw new NotImplementedException();
        }
    }
}
