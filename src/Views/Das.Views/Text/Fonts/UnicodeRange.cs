using System;
using System.Threading.Tasks;

namespace Das.Views.Text
{
    public struct UnicodeRange
    {
        public Int32 firstChar;
        public Int32 lastChar;

        public UnicodeRange(Int32 first,
                            Int32 last)
        {
            firstChar = first;
            lastChar = last;
        }

        public UInt32[] GetFullRange()
        {
            var num = Math.Min(lastChar, firstChar);
            var length = Math.Max(lastChar, firstChar) - num + 1;
            var fullRange = new UInt32[length];
            for (var index = 0; index < length; ++index)
                fullRange[index] = checked((UInt32)(num + index));
            return fullRange;
        }
    }
}
