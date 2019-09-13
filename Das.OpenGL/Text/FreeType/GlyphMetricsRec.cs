﻿using System.Runtime.InteropServices;
using FT_Long = System.IntPtr;

namespace Das.OpenGL.Text.FreeType
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GlyphMetricsRec
    {
        public FT_Long width;
        public FT_Long height;
        
        public FT_Long horiBearingX;
        public FT_Long horiBearingY;
        public FT_Long horiAdvance;
        
        public FT_Long vertBearingX;
        public FT_Long vertBearingY;
        public FT_Long vertAdvance;
    }
}
