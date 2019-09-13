using System;

namespace Das.OpenGL
{
    public enum FontModes
    {
        /// <summary>
        /// This is the default render mode; it corresponds to 8-bit anti-aliased bitmaps.
        /// </summary>
        Normal = 0,
        Light,
        Mono,
        Lcd,
        VerticalLcd,
    }
}
