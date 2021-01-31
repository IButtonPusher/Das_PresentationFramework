using System;
using System.Runtime.InteropServices;
using FT_Long = System.IntPtr;
// ReSharper disable All

namespace Das.OpenGL.Text.FreeType
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GlyphSlotRec
    {
        internal IntPtr library;
        internal IntPtr face;
        internal IntPtr next;
        internal UInt32 reserved;
        internal GenericRec generic;

        internal GlyphMetricsRec metrics;
        internal FT_Long linearHoriAdvance;
        internal FT_Long linearVertAdvance;
        internal FTVector26Dot6 advance;

        internal GlyphFormat format;

        internal BitmapRec bitmap;
        internal Int32 bitmap_left;
        internal Int32 bitmap_top;

        internal OutlineRec outline;

        internal UInt32 num_subglyphs;
        internal IntPtr subglyphs;

        internal IntPtr control_data;
        internal FT_Long control_len;

        internal FT_Long lsb_delta;
        internal FT_Long rsb_delta;

        internal IntPtr other;

        private IntPtr @internal;
    }

}
