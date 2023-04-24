using System;
using System.Runtime.InteropServices;
// ReSharper disable UnassignedField.Global
#pragma warning disable 8618

namespace Das.OpenGL.Text.FreeType;

[StructLayout(LayoutKind.Sequential)]
public class FtBitmapGlyph
{
   // ReSharper disable once UnusedMember.Global
   public GlyphRec root;
   public Int32 left;
   public Int32 top;
   public FtBitmap bitmap;
}