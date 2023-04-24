﻿using System;

namespace Das.OpenGL.Text.FreeType;

[Flags]
public enum OutlineFlags
{
   /// <summary>
   /// Value 0 is reserved.
   /// </summary>
   None = 0x0000,

   /// <summary>
   /// If set, this flag indicates that the outline's field arrays (i.e., ‘points’, ‘flags’, and ‘contours’) are
   /// ‘owned’ by the outline object, and should thus be freed when it is destroyed.
   /// </summary>
   Owner = 0x0001,

   /// <summary>
   /// By default, outlines are filled using the non-zero winding rule. If set to 1, the outline will be filled
   /// using the even-odd fill rule (only works with the smooth rasterizer).
   /// </summary>
   EvenOddFill = 0x0002,

   /// <summary>
   /// By default, outside contours of an outline are oriented in clock-wise direction, as defined in the TrueType
   /// specification. This flag is set if the outline uses the opposite direction (typically for Type 1 fonts).
   /// This flag is ignored by the scan converter.
   /// </summary>
   ReverseFill = 0x0004,

   /// <summary>
   /// By default, the scan converter will try to detect drop-outs in an outline and correct the glyph bitmap to
   /// ensure consistent shape continuity. If set, this flag hints the scan-line converter to ignore such cases.
   /// See below for more information.
   /// </summary>
   IgnoreDropouts = 0x0008,

   /// <summary>
   /// Select smart dropout control. If unset, use simple dropout control. Ignored if
   /// <see cref="OutlineFlags.IgnoreDropouts"/> is set. See below for more information.
   /// </summary>
   SmartDropouts = 0x0010,

   /// <summary>
   /// If set, turn pixels on for ‘stubs’, otherwise exclude them. Ignored if
   /// <see cref="OutlineFlags.IgnoreDropouts"/> is set. See below for more information.
   /// </summary>
   IncludeStubs = 0x0020,

   /// <summary>
   /// This flag indicates that the scan-line converter should try to convert this outline to bitmaps with the
   /// highest possible quality. It is typically set for small character sizes. Note that this is only a hint that
   /// might be completely ignored by a given scan-converter.
   /// </summary>
   HighPrecision = 0x0100,

   /// <summary>
   /// This flag is set to force a given scan-converter to only use a single pass over the outline to render a
   /// bitmap glyph image. Normally, it is set for very large character sizes. It is only a hint that might be
   /// completely ignored by a given scan-converter.
   /// </summary>
   SinglePass = 0x0200
}