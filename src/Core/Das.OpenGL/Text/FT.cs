using System;
using System.Runtime.InteropServices;

namespace Das.OpenGL.Text;

public static class FT
{
   private const String FreetypeDll = "freetype.dll";
   private const CallingConvention CallConvention = CallingConvention.Cdecl;

   [DllImport(FreetypeDll, CallingConvention = CallConvention)]
   public static extern FtErrors FT_Init_FreeType(out IntPtr alibrary);

   [DllImport(FreetypeDll, CallingConvention = CallConvention, CharSet = CharSet.Ansi, 
      BestFitMapping = false, ThrowOnUnmappableChar = true)]
   public static extern FtErrors FT_New_Face(IntPtr library, String filepathname, 
                                             Int32 face_index, out IntPtr aface);

   [DllImport(FreetypeDll, CallingConvention = CallConvention)]
   public static extern FtErrors FT_Set_Char_Size(IntPtr face, IntPtr char_width, 
                                                  IntPtr char_height, UInt32 horz_resolution, UInt32 vert_resolution);

   [DllImport(FreetypeDll, CallingConvention = CallConvention)]
   public static extern FtErrors FT_Set_Pixel_Sizes(IntPtr face, UInt32 pixel_width, 
                                                    UInt32 pixel_height);

   [DllImport(FreetypeDll, CallingConvention = CallConvention)]
   public static extern FtErrors FT_Done_Face(IntPtr face);

   [DllImport(FreetypeDll, CallingConvention = CallConvention)]
   public static extern FtErrors FT_Done_Library(IntPtr library);

   [DllImport(FreetypeDll, CallingConvention = CallConvention)]
   public static extern FtErrors FT_Glyph_To_Bitmap(ref IntPtr the_glyph, 
                                                    FontModes render_mode, ref FTVector26Dot6 origin, 
                                                    [MarshalAs(UnmanagedType.U1)] Boolean destroy);

   [DllImport(FreetypeDll, CallingConvention = CallConvention)]
   public static extern void FT_Glyph_To_Bitmap(out IntPtr glyph,
                                                FontModes render_mode, Int32 origin, Int32 destroy);

   [DllImport(FreetypeDll, CallingConvention = CallConvention)]
   public static extern UInt32 FT_Get_Char_Index(IntPtr face, UInt32 charcode);

   [DllImport(FreetypeDll, CallingConvention = CallConvention)]
   public static extern FtErrors FT_Load_Glyph(IntPtr face, UInt32 glyph_index, 
                                               Int32 load_flags);

   [DllImport(FreetypeDll, CallingConvention = CallConvention)]
   public static extern FtErrors FT_Get_Glyph_Name(IntPtr face, UInt32 glyph_index, 
                                                   IntPtr buffer, UInt32 buffer_max);

   [DllImport(FreetypeDll, CallingConvention = CallConvention)]
   internal static extern FtErrors FT_Get_Glyph(IntPtr slot, out IntPtr aglyph);

   [DllImport(FreetypeDll, CallingConvention = CallConvention)]
   public static extern FtErrors FT_Get_Kerning(IntPtr face, UInt32 left_glyph, 
                                                UInt32 right_glyph, UInt32 kern_mode, out FTVector26Dot6 akerning);

      
}