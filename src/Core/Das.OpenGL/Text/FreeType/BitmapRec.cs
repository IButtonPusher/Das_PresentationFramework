using System;
using System.Runtime.InteropServices;

namespace Das.OpenGL.Text.FreeType
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct BitmapRec
    {
        internal Int32 rows;
        internal Int32 width;
        internal Int32 pitch;
        internal IntPtr buffer;
        internal Int16 num_grays;
        internal FontModes pixel_mode;
        internal Byte palette_mode;
        internal IntPtr palette;
    }
}
