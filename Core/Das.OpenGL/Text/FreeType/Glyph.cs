using Das.Views.Core.Geometry;
using System;

namespace Das.OpenGL.Text.FreeType
{
    public class Glyph
    {
        private readonly GlyphSlotRec _slotRect;
        public Char Char { get; }

        public Glyph(GlyphSlotRec slotRec, Char c)
        {
            _slotRect = slotRec;
            Char = c;
            Advance = new Vector
            {
                x = slotRec.advance.X.ToInt32(),
                y = slotRec.advance.Y.ToInt32()
            };
        }

        public Vector Advance { get; }

        public float HorizontalBearingX => GetFloat(_slotRect.metrics.horiBearingX);

        public float HorizontalBearingY => GetFloat(_slotRect.metrics.horiBearingY);

        public float Height => GetFloat(_slotRect.metrics.height);

        public float Width => GetFloat(_slotRect.metrics.width);
     
        private static float GetFloat(IntPtr ptr) =>
            Fixed26Dot6.FromRawValue((Int32)ptr).Value / 64f;
    }
}