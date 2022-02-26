using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts.Open
{
    public class Feature
    {
        public Feature(UInt16 startIndex,
                       UInt16 length,
                       UInt32 tag,
                       UInt32 parameter)
        {
            _startIndex = startIndex;
            _length = length;
            _tag = tag;
            _parameter = parameter;
        }

        public UInt32 Tag
        {
            get => _tag;
            set => _tag = value;
        }

        public UInt32 Parameter
        {
            get => _parameter;
            set => _parameter = value;
        }

        public UInt16 StartIndex
        {
            get => _startIndex;
            set => _startIndex = value;
        }

        public UInt16 Length
        {
            get => _length;
            set => _length = value;
        }

        private UInt16 _length;
        private UInt32 _parameter;
        private UInt16 _startIndex;
        private UInt32 _tag;
    }
}
