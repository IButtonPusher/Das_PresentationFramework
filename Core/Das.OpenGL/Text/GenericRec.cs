using System;
using System.Runtime.InteropServices;

namespace Das.OpenGL.Text
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct GenericRec
    {
        internal IntPtr data;
        internal IntPtr finalizer;
    }
}
