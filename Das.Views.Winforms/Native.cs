using System;
using System.Runtime.InteropServices;

namespace Das.Views.Winforms
{
    public static class Native
    {
        public const string Gdi32 = "gdi32.dll";
        public const string User32 = "user32.dll";

        [DllImport(Gdi32)]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref BitMapInfo pbmi, uint iUsage,
            out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

        [DllImport(Gdi32)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiObj);

        [DllImport(Gdi32, SetLastError = true)]
        public static extern int ChoosePixelFormat(IntPtr hDC,
            [In, MarshalAs(UnmanagedType.LPStruct)] Pixelformatdescriptor ppfd);

        [DllImport(Gdi32, SetLastError = true)]
        public static extern int SetPixelFormat(IntPtr hDC, int iPixelFormat,
            [In, MarshalAs(UnmanagedType.LPStruct)] Pixelformatdescriptor ppfd);

        [DllImport(Gdi32)]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport(User32, SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport(Gdi32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, 
            int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        [DllImport(User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, 
            int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport(User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U2)]
        public static extern short RegisterClassEx([In] ref WNDCLASSEX lpwcx);

        [DllImport(Gdi32, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport(Gdi32, SetLastError = true)]
        public static extern IntPtr CreateFont(int nHeight, int nWidth, int nEscapement,
            int nOrientation, uint fnWeight, uint fdwItalic, uint fdwUnderline, uint
                fdwStrikeOut, uint fdwCharSet, uint fdwOutputPrecision, uint
                fdwClipPrecision, uint fdwQuality, uint fdwPitchAndFamily, string lpszFace);

        [DllImport(Gdi32, CharSet = CharSet.Unicode)]

        public static extern bool GetTextExtentPoint32(IntPtr hdc, string lpString,
            int cbString, out SIZE lpSize);

        public const uint SRCCOPY = 0x00CC0020;

        public const uint DEFAULT_CHARSET = 1;
        public const uint OUT_OUTLINE_PRECIS = 8;
        public const uint CLIP_DEFAULT_PRECIS = 0;
        public const uint VARIABLE_PITCH = 2;
        public const uint CLEARTYPE_QUALITY = 5;
    }
}
