using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts.Open
{
    public class OpenTypeLayoutWorkspace
    {
        internal OpenTypeLayoutWorkspace()
        {
            _bytesPerLookup = 0;
            _lookupUsageFlags = null;
            _cachePointers = null;
        }

        public void InitLookupUsageFlags(Int32 lookupCount,
                                         Int32 featureCount)
        {
            _bytesPerLookup = (featureCount + 2 + 7) >> 3;
            var length = lookupCount * _bytesPerLookup;
            if (_lookupUsageFlags == null || _lookupUsageFlags.Length < length)
                _lookupUsageFlags = new Byte[length];
            Array.Clear(_lookupUsageFlags, 0, length);
        }

        public Boolean IsAggregatedFlagSet(Int32 lookupIndex)
        {
            return (_lookupUsageFlags[lookupIndex * _bytesPerLookup] & 1U) > 0U;
        }

        public Boolean IsFeatureFlagSet(Int32 lookupIndex,
                                        Int32 featureIndex)
        {
            var num = featureIndex + 2;
            return (_lookupUsageFlags[lookupIndex * _bytesPerLookup + (num >> 3)] & (UInt32)(Byte)(1 << (num % 8))) >
                   0U;
        }

        public Boolean IsRequiredFeatureFlagSet(Int32 lookupIndex)
        {
            return (_lookupUsageFlags[lookupIndex * _bytesPerLookup] & 2U) > 0U;
        }

        public void SetFeatureFlag(Int32 lookupIndex,
                                   Int32 featureIndex)
        {
            var index1 = lookupIndex * _bytesPerLookup;
            var num1 = featureIndex + 2;
            var index2 = index1 + (num1 >> 3);
            var num2 = (Byte)(1 << (num1 % 8));
            if (index2 >= _lookupUsageFlags.Length)
                throw new FormatException();
            _lookupUsageFlags[index2] |= num2;
            _lookupUsageFlags[index1] |= 1;
        }

        public void SetRequiredFeatureFlag(Int32 lookupIndex)
        {
            var index = lookupIndex * _bytesPerLookup;
            if (index >= _lookupUsageFlags.Length)
                throw new FormatException();
            _lookupUsageFlags[index] |= 3;
        }

        public void AllocateCachePointers(Int32 glyphRunLength)
        {
            if (_cachePointers != null && _cachePointers.Length >= glyphRunLength)
                return;
            _cachePointers = new UInt16[glyphRunLength];
        }

        public void UpdateCachePointers(Int32 oldLength,
                                        Int32 newLength,
                                        Int32 firstGlyphChanged,
                                        Int32 afterLastGlyphChanged)
        {
            if (oldLength == newLength)
                return;
            var sourceIndex = afterLastGlyphChanged - (newLength - oldLength);
            if (_cachePointers.Length < newLength)
            {
                var destinationArray = new UInt16[newLength];
                Array.Copy(_cachePointers, destinationArray, firstGlyphChanged);
                Array.Copy(_cachePointers, sourceIndex, destinationArray, afterLastGlyphChanged,
                    oldLength - sourceIndex);
                _cachePointers = destinationArray;
            }
            else
                Array.Copy(_cachePointers, sourceIndex, _cachePointers, afterLastGlyphChanged, oldLength - sourceIndex);
        }

        internal OpenTypeLayoutResult Init(IOpenTypeFont font,
                                           OpenTypeTags tableTag,
                                           UInt32 scriptTag,
                                           UInt32 langSysTag)
        {
            return OpenTypeLayoutResult.Success;
        }

        public UInt16[] CachePointers => _cachePointers;

        public Byte[] TableCacheData
        {
            get => _tableCache;
            set => _tableCache = value;
        }

        private const Byte AggregatedFlagMask = 1;
        private const Byte RequiredFeatureFlagMask = 2;
        private const Int32 FeatureFlagsStartBit = 2;
        private Int32 _bytesPerLookup;
        private UInt16[] _cachePointers;
        private Byte[] _lookupUsageFlags;
        private Byte[] _tableCache;
    }
}
