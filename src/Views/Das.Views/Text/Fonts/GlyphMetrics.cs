using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts
{
    [StructLayout(LayoutKind.Explicit, Pack = 16)]
    public struct GlyphMetrics
    {
        [FieldOffset(0)]
        public Int32 LeftSideBearing;

        [FieldOffset(4)]
        public UInt32 AdvanceWidth;

        [FieldOffset(8)]
        public Int32 RightSideBearing;

        [FieldOffset(12)]
        public Int32 TopSideBearing;

        [FieldOffset(16)]
        public UInt32 AdvanceHeight;

        [FieldOffset(20)]
        public Int32 BottomSideBearing;

        [FieldOffset(24)]
        public Int32 VerticalOriginY;
    }
}
