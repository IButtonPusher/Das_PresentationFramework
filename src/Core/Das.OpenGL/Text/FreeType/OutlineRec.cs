using System;
using System.Runtime.InteropServices;

namespace Das.OpenGL.Text.FreeType;

[StructLayout(LayoutKind.Sequential)]
internal struct OutlineRec
{
   internal Int16 n_contours;
   internal Int16 n_points;

   internal IntPtr points;
   internal IntPtr tags;
   internal IntPtr contours;

   internal OutlineFlags flags;
}