using System;
using System.Runtime.InteropServices;

namespace Das.OpenGL.Text
{
    public static class FT
    {
        private const string FreetypeDll = "freetype.dll";
        private const CallingConvention CallConvention = CallingConvention.Cdecl;

        [DllImport(FreetypeDll, CallingConvention = CallConvention)]
        public static extern FtErrors FT_Init_FreeType(out IntPtr alibrary);

        [DllImport(FreetypeDll, CallingConvention = CallConvention, CharSet = CharSet.Ansi, 
            BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern FtErrors FT_New_Face(IntPtr library, string filepathname, 
            int face_index, out IntPtr aface);

        [DllImport(FreetypeDll, CallingConvention = CallConvention)]
        public static extern FtErrors FT_Set_Char_Size(IntPtr face, IntPtr char_width, 
            IntPtr char_height, uint horz_resolution, uint vert_resolution);

        [DllImport(FreetypeDll, CallingConvention = CallConvention)]
        public static extern FtErrors FT_Set_Pixel_Sizes(IntPtr face, uint pixel_width, 
            uint pixel_height);

        [DllImport(FreetypeDll, CallingConvention = CallConvention)]
        public static extern FtErrors FT_Done_Face(IntPtr face);

        [DllImport(FreetypeDll, CallingConvention = CallConvention)]
        public static extern FtErrors FT_Done_Library(IntPtr library);

        [DllImport(FreetypeDll, CallingConvention = CallConvention)]
        public static extern FtErrors FT_Glyph_To_Bitmap(ref IntPtr the_glyph, 
            FontModes render_mode, ref FTVector26Dot6 origin, 
            [MarshalAs(UnmanagedType.U1)] bool destroy);

        [DllImport(FreetypeDll, CallingConvention = CallConvention)]
        public static extern void FT_Glyph_To_Bitmap(out IntPtr glyph,
            FontModes render_mode, int origin, int destroy);

        [DllImport(FreetypeDll, CallingConvention = CallConvention)]
        public static extern uint FT_Get_Char_Index(IntPtr face, uint charcode);

        [DllImport(FreetypeDll, CallingConvention = CallConvention)]
        public static extern FtErrors FT_Load_Glyph(IntPtr face, uint glyph_index, 
            int load_flags);

        [DllImport(FreetypeDll, CallingConvention = CallConvention)]
        public static extern FtErrors FT_Get_Glyph_Name(IntPtr face, uint glyph_index, 
            IntPtr buffer, uint buffer_max);

        [DllImport(FreetypeDll, CallingConvention = CallConvention)]
        internal static extern FtErrors FT_Get_Glyph(IntPtr slot, out IntPtr aglyph);

        [DllImport(FreetypeDll, CallingConvention = CallConvention)]
        public static extern FtErrors FT_Get_Kerning(IntPtr face, uint left_glyph, 
            uint right_glyph, uint kern_mode, out FTVector26Dot6 akerning);

      
    }
}
