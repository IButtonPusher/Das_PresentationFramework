using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts
{
    [StructLayout(LayoutKind.Explicit, Pack = 16)]
    public sealed class FontMetrics
    {
        public Double Baseline => (LineGap * 0.5 + Ascent) / DesignUnitsPerEm;

        public Double LineSpacing => (LineGap + Descent + Ascent) / (Double)DesignUnitsPerEm;

        [FieldOffset(2)]
        public UInt16 Ascent;

        [FieldOffset(10)]
        public UInt16 CapHeight;

        [FieldOffset(4)]
        public UInt16 Descent;

        [FieldOffset(0)]
        public UInt16 DesignUnitsPerEm;

        [FieldOffset(8)]
        public Int16 LineGap;

        [FieldOffset(18)]
        public Int16 StrikethroughPosition;

        [FieldOffset(20)]
        public UInt16 StrikethroughThickness;

        [FieldOffset(14)]
        public Int16 UnderlinePosition;

        [FieldOffset(16)]
        public UInt16 UnderlineThickness;

        [FieldOffset(12)]
        public UInt16 XHeight;
    }
}
