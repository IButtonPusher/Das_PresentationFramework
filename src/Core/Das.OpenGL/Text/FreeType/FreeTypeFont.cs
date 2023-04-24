using System;
using System.Runtime.InteropServices;
using FT_Long = System.IntPtr;
using FT_ULong = System.UIntPtr;
// ReSharper disable All

namespace Das.OpenGL.Text;

[StructLayout(LayoutKind.Sequential)]
public struct FreeTypeFont
{
   internal FT_Long num_faces;
   internal FT_Long face_index;

   internal FT_Long face_flags;
   internal FT_Long style_flags;

   internal FT_Long num_glyphs;

   internal IntPtr family_name;
   internal IntPtr style_name;

   internal Int32 num_fixed_sizes;
   internal IntPtr available_sizes;

   internal Int32 num_charmaps;
   internal IntPtr charmaps;

   internal GenericRec generic;

   internal BBox bbox;

   internal UInt16 units_per_EM;
   internal Int16 ascender;
   internal Int16 descender;
   internal Int16 height;

   internal Int16 max_advance_width;
   internal Int16 max_advance_height;

   internal Int16 underline_position;
   internal Int16 underline_thickness;

   internal IntPtr glyph;
   internal IntPtr size;
   internal IntPtr charmap;

   private IntPtr driver;
   private IntPtr memory;
   private IntPtr stream;

   private IntPtr sizes_list;
   private GenericRec autohint;
   private IntPtr extensions;

   private IntPtr @internal;

   internal static Int32 SizeInBytes => Marshal.SizeOf(typeof(FreeTypeFont));
}