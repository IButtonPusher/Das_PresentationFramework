using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Das.Views.Core.Writing;
using Das.Views.Localization;
using Das.Views.Text.Fonts;
using Das.Views.Text.Fonts.Cache;
using Das.Views.Text.Fonts.Open;

namespace Das.Views.Text
{
    public sealed class FontFaceLayoutInfo
    {
        internal FontFaceLayoutInfo(Font font)
        {
            _fontTechnologyInitialized = false;
            _typographyAvailabilitiesInitialized = false;
            _gsubInitialized = false;
            _gposInitialized = false;
            _gdefInitialized = false;
            _embeddingRightsInitialized = false;
            _gsubCache = null;
            _gposCache = null;
            _gsub = null;
            _gpos = null;
            _gdef = null;
            _font = font;
            _cmap = new IntMap(_font);
            _cmap.TryGetValue(32, out _blankGlyphIndex);
        }

        internal Byte[] Gsub()
        {
            if (!_gsubInitialized)
            {
                _gsub = GetFontTable(OpenTypeTableTag.TTO_GSUB);
                _gsubInitialized = true;
            }

            return _gsub;
        }

        internal Byte[] Gpos()
        {
            if (!_gposInitialized)
            {
                _gpos = GetFontTable(OpenTypeTableTag.TTO_GPOS);
                _gposInitialized = true;
            }

            return _gpos;
        }

        internal Byte[] Gdef()
        {
            if (!_gdefInitialized)
            {
                _gdef = GetFontTable(OpenTypeTableTag.TTO_GDEF);
                _gdefInitialized = true;
            }

            return _gdef;
        }

        internal Byte[] GetTableCache(OpenTypeTags tableTag)
        {
            if (tableTag != OpenTypeTags.GPOS)
            {
                if (tableTag != OpenTypeTags.GSUB)
                    throw new NotSupportedException();
                if (Gsub() != null)
                    return _gsubCache;
            }
            else if (Gpos() != null)
                return _gposCache;

            return null;
        }

        internal Byte[] AllocateTableCache(OpenTypeTags tableTag,
                                           Int32 size)
        {
            if (tableTag != OpenTypeTags.GPOS)
            {
                if (tableTag != OpenTypeTags.GSUB)
                    throw new NotSupportedException();
                _gsubCache = new Byte[size];
                return _gsubCache;
            }

            _gposCache = new Byte[size];
            return _gposCache;
        }

        private Byte[] GetFontTable(OpenTypeTableTag openTypeTableTag)
        {
            var fontFace = _font.GetFontFace();
            Byte[] tableData;
            try
            {
                if (!fontFace.TryGetFontTable(openTypeTableTag, out tableData))
                    tableData = null;
            }
            finally
            {
                fontFace.Release();
            }

            return tableData;
        }

        private void ComputeFontTechnology()
        {
            var fontFace = _font.GetFontFace();
            try
            {
                if (fontFace.Type == FontFaceType.TrueTypeCollection)
                    _fontTechnology = FontTechnology.TrueTypeCollection;
                else if (fontFace.Type == FontFaceType.CFF)
                    _fontTechnology = FontTechnology.PostscriptOpenType;
                else
                    _fontTechnology = FontTechnology.TrueType;
            }
            finally
            {
                fontFace.Release();
            }
        }

        private unsafe void ComputeTypographyAvailabilities()
        {
            var length = (GlyphCount + 31) >> 5;
            var uints = BufferCache.GetUInts(length);
            Array.Clear(uints, 0, length);
            var minGlyphId = UInt16.MaxValue;
            UInt16 maxGlyphId = 0;
            var typographyAvailabilities1 = TypographyAvailabilities.None;
            var Font = new GsubGposTables(this);
            for (var index1 = 0; index1 < fastTextRanges.Length; ++index1)
            {
                var fullRange = fastTextRanges[index1].GetFullRange();
                var ushorts = BufferCache.GetUShorts(fullRange.Length);
                fixed (UInt32* pKeys = &fullRange[0])
                fixed (UInt16* pIndices = &ushorts[0])
                    CharacterMap.TryGetValues(pKeys, checked((UInt32)fullRange.Length), pIndices);
                for (var index2 = 0; index2 < fullRange.Length; ++index2)
                {
                    var num = ushorts[index2];
                    if (num != 0)
                    {
                        uints[num >> 5] |= (UInt32)(1 << (num % 32));
                        if (num > maxGlyphId)
                            maxGlyphId = num;
                        if (num < minGlyphId)
                            minGlyphId = num;
                    }
                }

                BufferCache.ReleaseUShorts(ushorts);
            }

            WritingSystem[] complexLanguages;
            if (OpenTypeLayout.GetComplexLanguageList(Font, LoclFeature, uints, minGlyphId, maxGlyphId,
                    out complexLanguages) != OpenTypeLayoutResult.Success)
            {
                _typographyAvailabilities = TypographyAvailabilities.None;
            }
            else
            {
                if (complexLanguages != null)
                {
                    var typographyAvailabilities2 =
                        TypographyAvailabilities.FastTextMajorLanguageLocalizedFormAvailable |
                        TypographyAvailabilities.FastTextExtraLanguageLocalizedFormAvailable;
                    for (var index = 0;
                         index < complexLanguages.Length && typographyAvailabilities1 != typographyAvailabilities2;
                         ++index)
                    {
                        if (MajorLanguages.Contains((ScriptTags)complexLanguages[index].scriptTag,
                                (LanguageTags)complexLanguages[index].langSysTag))
                            typographyAvailabilities1 |=
                                TypographyAvailabilities.FastTextMajorLanguageLocalizedFormAvailable;
                        else
                            typographyAvailabilities1 |=
                                TypographyAvailabilities.FastTextExtraLanguageLocalizedFormAvailable;
                    }
                }

                if (OpenTypeLayout.GetComplexLanguageList(Font, RequiredTypographyFeatures, uints, minGlyphId,
                        maxGlyphId, out complexLanguages) != OpenTypeLayoutResult.Success)
                {
                    _typographyAvailabilities = TypographyAvailabilities.None;
                }
                else
                {
                    if (complexLanguages != null)
                        typographyAvailabilities1 |= TypographyAvailabilities.FastTextTypographyAvailable;
                    for (var index = 0; index < length; ++index)
                        uints[index] = UInt32.MaxValue;
                    if (OpenTypeLayout.GetComplexLanguageList(Font, RequiredFeatures, uints, minGlyphId, maxGlyphId,
                            out complexLanguages) != OpenTypeLayoutResult.Success)
                    {
                        _typographyAvailabilities = TypographyAvailabilities.None;
                    }
                    else
                    {
                        if (complexLanguages != null)
                        {
                            for (var index = 0; index < complexLanguages.Length; ++index)
                            {
                                if (complexLanguages[index].scriptTag == 1751215721U)
                                    typographyAvailabilities1 |= TypographyAvailabilities.IdeoTypographyAvailable;
                                else
                                    typographyAvailabilities1 |= TypographyAvailabilities.Available;
                            }
                        }

                        if (typographyAvailabilities1 != TypographyAvailabilities.None)
                            typographyAvailabilities1 |= TypographyAvailabilities.Available;
                        _typographyAvailabilities = typographyAvailabilities1;
                        BufferCache.ReleaseUInts(uints);
                    }
                }
            }
        }

        internal IntMap CharacterMap => _cmap;

        internal UInt16 BlankGlyph => _blankGlyphIndex;

        internal UInt16 DesignEmHeight => _font.Metrics.DesignUnitsPerEm;

        internal FontEmbeddingRight EmbeddingRights
        {
            get
            {
                if (!_embeddingRightsInitialized)
                {
                    var fontEmbeddingRight = FontEmbeddingRight.RestrictedLicense;
                    var fontFace = _font.GetFontFace();
                    UInt16 fsType;
                    Boolean flag;
                    try
                    {
                        flag = fontFace.ReadFontEmbeddingRights(out fsType);
                    }
                    finally
                    {
                        fontFace.Release();
                    }

                    if (flag)
                    {
                        if ((fsType & 14) == 0)
                        {
                            switch (fsType & 768)
                            {
                                case 0:
                                    fontEmbeddingRight = FontEmbeddingRight.Installable;
                                    break;
                                case 256:
                                    fontEmbeddingRight = FontEmbeddingRight.InstallableButNoSubsetting;
                                    break;
                                case 512:
                                    fontEmbeddingRight = FontEmbeddingRight.InstallableButWithBitmapsOnly;
                                    break;
                                case 768:
                                    fontEmbeddingRight =
                                        FontEmbeddingRight.InstallableButNoSubsettingAndWithBitmapsOnly;
                                    break;
                            }
                        }
                        else if ((fsType & 8) != 0)
                        {
                            switch (fsType & 768)
                            {
                                case 0:
                                    fontEmbeddingRight = FontEmbeddingRight.Editable;
                                    break;
                                case 256:
                                    fontEmbeddingRight = FontEmbeddingRight.EditableButNoSubsetting;
                                    break;
                                case 512:
                                    fontEmbeddingRight = FontEmbeddingRight.EditableButWithBitmapsOnly;
                                    break;
                                case 768:
                                    fontEmbeddingRight = FontEmbeddingRight.EditableButNoSubsettingAndWithBitmapsOnly;
                                    break;
                            }
                        }
                        else if ((fsType & 4) != 0)
                        {
                            switch (fsType & 768)
                            {
                                case 0:
                                    fontEmbeddingRight = FontEmbeddingRight.PreviewAndPrint;
                                    break;
                                case 256:
                                    fontEmbeddingRight = FontEmbeddingRight.PreviewAndPrintButNoSubsetting;
                                    break;
                                case 512:
                                    fontEmbeddingRight = FontEmbeddingRight.PreviewAndPrintButWithBitmapsOnly;
                                    break;
                                case 768:
                                    fontEmbeddingRight =
                                        FontEmbeddingRight.PreviewAndPrintButNoSubsettingAndWithBitmapsOnly;
                                    break;
                            }
                        }
                    }

                    _embeddingRights = fontEmbeddingRight;
                    _embeddingRightsInitialized = true;
                }

                return _embeddingRights;
            }
        }

        internal FontTechnology FontTechnology
        {
            get
            {
                if (!_fontTechnologyInitialized)
                {
                    ComputeFontTechnology();
                    _fontTechnologyInitialized = true;
                }

                return _fontTechnology;
            }
        }

        internal TypographyAvailabilities TypographyAvailabilities
        {
            get
            {
                if (!_typographyAvailabilitiesInitialized)
                {
                    ComputeTypographyAvailabilities();
                    _typographyAvailabilitiesInitialized = true;
                }

                return _typographyAvailabilities;
            }
        }

        internal UInt16 GlyphCount
        {
            get
            {
                var fontFace = _font.GetFontFace();
                try
                {
                    return fontFace.GlyphCount;
                }
                finally
                {
                    fontFace.Release();
                }
            }
        }

        private static readonly UInt32[] LoclFeature = new UInt32[1]
        {
            1819239276U
        };

        private static readonly UInt32[] RequiredTypographyFeatures = new UInt32[8]
        {
            1667460464U,
            1919707495U,
            1818847073U,
            1668049255U,
            1667329140U,
            1801810542U,
            1835102827U,
            1835756907U
        };

        private static readonly UInt32[] RequiredFeatures = new UInt32[9]
        {
            1819239276U,
            1667460464U,
            1919707495U,
            1818847073U,
            1668049255U,
            1667329140U,
            1801810542U,
            1835102827U,
            1835756907U
        };

        private static readonly UnicodeRange[] fastTextRanges = new UnicodeRange[8]
        {
            new(32, 126),
            new(161, Byte.MaxValue),
            new(256, 383),
            new(384, 591),
            new(7680, 7935),
            new(12352, 12440),
            new(12443, 12447),
            new(12448, 12543)
        };

        private readonly UInt16 _blankGlyphIndex;
        private readonly IntMap _cmap;
        private FontEmbeddingRight _embeddingRights;
        private Boolean _embeddingRightsInitialized;
        private readonly Font _font;
        private FontTechnology _fontTechnology;
        private Boolean _fontTechnologyInitialized;
        private Byte[] _gdef;
        private Boolean _gdefInitialized;
        private Byte[] _gpos;
        private Byte[] _gposCache;
        private Boolean _gposInitialized;
        private Byte[] _gsub;
        private Byte[] _gsubCache;
        private Boolean _gsubInitialized;
        private TypographyAvailabilities _typographyAvailabilities;
        private Boolean _typographyAvailabilitiesInitialized;

        private static class Os2EmbeddingFlags
        {
            public const UInt16 RestrictedLicense = 2;
            public const UInt16 PreviewAndPrint = 4;
            public const UInt16 Editable = 8;
            public const UInt16 InstallableMask = 14;
            public const UInt16 NoSubsetting = 256;
            public const UInt16 BitmapOnly = 512;
        }

        internal sealed class IntMap :
            IDictionary<Int32, UInt16>,
            ICollection<KeyValuePair<Int32, UInt16>>,
            IEnumerable<KeyValuePair<Int32, UInt16>>,
            IEnumerable
        {
            internal IntMap(Font font)
            {
                _font = font;
                _cmap = null;
            }

            public void Add(Int32 key,
                            UInt16 value)
            {
                throw new NotSupportedException();
            }

            public Boolean ContainsKey(Int32 key)
            {
                return _font.HasCharacter(checked((UInt32)key));
            }

            public ICollection<Int32> Keys => CMap.Keys;

            public Boolean Remove(Int32 key)
            {
                throw new NotSupportedException();
            }

            /// <SecurityNote>
            ///     Critical: This code calls into an unsafe block and calls critical GetArrayOfGlyphIndices.
            ///     TreatAsSafe: Creates its own known-sized buffers to pass into critical code and
            ///     does not modify or return any critical data.
            /// </SecurityNote>
            [SecurityCritical]
            [SecurityTreatAsSafe]
            public Boolean TryGetValue(Int32 key,
                                       out UInt16 value)
            {
                UInt16 localValue;
                unsafe
                {
                    var uKey = checked((UInt32)key);
                    var pKey = &uKey;

                    var fontFace = _font.GetFontFace();
                    try
                    {
                        fontFace.GetArrayOfGlyphIndices(pKey, 1, &localValue);
                    }
                    finally
                    {
                        fontFace.Release();
                    }

                    value = localValue;
                }

                // if a glyph is not present, index 0 is returned
                return value != 0;
            }

            public ICollection<UInt16> Values => CMap.Values;

            UInt16 IDictionary<Int32, UInt16>.this[Int32 i]
            {
                get
                {
                    UInt16 num;
                    if (!TryGetValue(i, out num))
                        throw new KeyNotFoundException();
                    return num;
                }
                set => throw new NotSupportedException();
            }

            public void Add(KeyValuePair<Int32, UInt16> item)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public Boolean Contains(KeyValuePair<Int32, UInt16> item)
            {
                return ContainsKey(item.Key);
            }

            public void CopyTo(KeyValuePair<Int32, UInt16>[] array,
                               Int32 arrayIndex)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (array.Rank != 1)
                    throw new ArgumentException(SR.Get("Collection_BadRank"));
                if (arrayIndex < 0 || arrayIndex >= array.Length || arrayIndex + Count > array.Length)
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex));
                foreach (var keyValuePair in this)
                    array[arrayIndex++] = keyValuePair;
            }

            public Int32 Count => CMap.Count;

            public Boolean IsReadOnly => true;

            public Boolean Remove(KeyValuePair<Int32, UInt16> item)
            {
                throw new NotSupportedException();
            }

            public IEnumerator<KeyValuePair<Int32, UInt16>> GetEnumerator()
            {
                return CMap.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            internal unsafe void TryGetValues(UInt32* pKeys,
                                              UInt32 characterCount,
                                              UInt16* pIndices)
            {
                var fontFace = _font.GetFontFace();
                try
                {
                    fontFace.GetArrayOfGlyphIndices(pKeys, characterCount, pIndices);
                }
                finally
                {
                    fontFace.Release();
                }
            }

            private Dictionary<Int32, UInt16> CMap
            {
                get
                {
                    if (_cmap == null)
                    {
                        var intMap = this;
                        var lockTaken = false;
                        try
                        {
                            Monitor.Enter(intMap, ref lockTaken);
                            if (_cmap == null)
                            {
                                _cmap = new Dictionary<Int32, UInt16>();
                                for (var key = 0; key <= 1114111; ++key)
                                {
                                    UInt16 num;
                                    if (TryGetValue(key, out num))
                                        _cmap.Add(key, num);
                                }
                            }
                        }
                        finally
                        {
                            if (lockTaken)
                                Monitor.Exit(intMap);
                        }
                    }

                    return _cmap;
                }
            }

            private Dictionary<Int32, UInt16> _cmap;
            private readonly Font _font;
        }
    }
}
