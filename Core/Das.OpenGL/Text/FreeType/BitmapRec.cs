using System;
using System.Runtime.InteropServices;

namespace Das.OpenGL.Text.FreeType
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct BitmapRec
    {
        internal int rows;
        internal int width;
        internal int pitch;
        internal IntPtr buffer;
        internal short num_grays;
        internal FontModes pixel_mode;
        internal byte palette_mode;
        internal IntPtr palette;
    }
}
