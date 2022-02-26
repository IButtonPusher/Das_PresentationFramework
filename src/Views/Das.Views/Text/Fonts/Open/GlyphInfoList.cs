using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts.Open
{
    public class GlyphInfoList
    {
        public GlyphInfoList(Int32 capacity,
                             Int32 leap,
                             Boolean justify)
        {
            _glyphs = new UshortList(capacity, leap);
            _glyphFlags = new UshortList(capacity, leap);
            _firstChars = new UshortList(capacity, leap);
            _ligatureCounts = new UshortList(capacity, leap);
        }

        public void SetRange(Int32 index,
                             Int32 length)
        {
            _glyphs.SetRange(index, length);
            _glyphFlags.SetRange(index, length);
            _firstChars.SetRange(index, length);
            _ligatureCounts.SetRange(index, length);
        }

        public void SetLength(Int32 length)
        {
            _glyphs.Length = length;
            _glyphFlags.Length = length;
            _firstChars.Length = length;
            _ligatureCounts.Length = length;
        }

        public void Insert(Int32 index,
                           Int32 Count)
        {
            _glyphs.Insert(index, Count);
            _glyphFlags.Insert(index, Count);
            _firstChars.Insert(index, Count);
            _ligatureCounts.Insert(index, Count);
        }

        public void Remove(Int32 index,
                           Int32 Count)
        {
            _glyphs.Remove(index, Count);
            _glyphFlags.Remove(index, Count);
            _firstChars.Remove(index, Count);
            _ligatureCounts.Remove(index, Count);
        }

        [Conditional("DEBUG")]
        internal void ValidateLength(Int32 cch)
        {
        }

        public Int32 Length => _glyphs.Length;

        internal Int32 Offset => _glyphs.Offset;

        public UshortList Glyphs => _glyphs;

        public UshortList GlyphFlags => _glyphFlags;

        public UshortList FirstChars => _firstChars;

        public UshortList LigatureCounts => _ligatureCounts;

        private readonly UshortList _firstChars;
        private readonly UshortList _glyphFlags;
        private readonly UshortList _glyphs;
        private readonly UshortList _ligatureCounts;
    }
}
