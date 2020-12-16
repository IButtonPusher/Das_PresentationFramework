using System;
using System.Runtime.InteropServices;

namespace Das.OpenGL.Text.FreeType
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct OutlineRec
    {
        internal short n_contours;
        internal short n_points;

        internal IntPtr points;
        internal IntPtr tags;
        internal IntPtr contours;

        internal OutlineFlags flags;
    }
}
