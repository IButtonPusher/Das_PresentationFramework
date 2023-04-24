using System;
using System.Runtime.InteropServices;

namespace Das.OpenGL.Text.FreeType;

[StructLayout(LayoutKind.Sequential)]
public class FtBitmap
{
   public Int32 rows;
   public Int32 width;
   public Int32 pitch;
   public IntPtr buffer;
   public Int16 num_grays;
   public SByte pixel_mode;
   public SByte palette_mode;
   public IntPtr palette;
}