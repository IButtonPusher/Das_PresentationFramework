using System;

namespace Das.OpenGL.Text.FreeType;

[Flags]
public enum FaceFlags : long
{
   /// <summary>
   /// No style flags.
   /// </summary>
   None = 0x0000,

   Scalable = 0x0001,
   FixedSizes = 0x0002,

   FixedWidth = 0x0004,
   Sfnt = 0x0008,
   Horizontal = 0x0010,
   Vertical = 0x0020,
   Kerning = 0x0040,
   MultipleMasters = 0x0100,
   GlyphNames = 0x0200,
   ExternalStream = 0x0400,
   Hinter = 0x0800,
   CidKeyed = 0x1000,
   Tricky = 0x2000,
   Color = 0x4000,
}