using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.OpenGL.Text.FreeType
{
    public class Glyph
    {
        public Glyph(GlyphSlotRec slotRec,
                     Char c)
        {
            _slotRect = slotRec;
            Char = c;
            Advance = new Vector
            {
                x = slotRec.advance.X.ToInt32(),
                y = slotRec.advance.Y.ToInt32()
            };
        }

        private static Single GetFloat(IntPtr ptr)
        {
            return Fixed26Dot6.FromRawValue((Int32)ptr).Value / 64f;
        }

        public Char Char { get; }

        public Vector Advance { get; }

        public Single HorizontalBearingX => GetFloat(_slotRect.metrics.horiBearingX);

        public Single HorizontalBearingY => GetFloat(_slotRect.metrics.horiBearingY);

        public Single Height => GetFloat(_slotRect.metrics.height);

        public Single Width => GetFloat(_slotRect.metrics.width);

        private readonly GlyphSlotRec _slotRect;
    }
}
