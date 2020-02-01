using System;
using System.Runtime.InteropServices;
using Windows.Shared.Messages;

namespace Das.Views.Winforms
{
    public static class Native
    {
        public const String Gdi32 = "gdi32.dll";
        public const String User32 = "user32.dll";

        [DllImport(Gdi32)]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref BitMapInfo pbmi, UInt32 iUsage,
            out IntPtr ppvBits, IntPtr hSection, UInt32 dwOffset);

        [DllImport(Gdi32)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiObj);

        [DllImport(Gdi32, SetLastError = true)]
        public static extern Int32 ChoosePixelFormat(IntPtr hDC,
            [In, MarshalAs(UnmanagedType.LPStruct)] Pixelformatdescriptor ppfd);

        [DllImport(Gdi32, SetLastError = true)]
        public static extern Int32 SetPixelFormat(IntPtr hDC, Int32 iPixelFormat,
            [In, MarshalAs(UnmanagedType.LPStruct)] Pixelformatdescriptor ppfd);

        [DllImport(Gdi32)]
        public static extern Boolean DeleteObject(IntPtr hObject);

        [DllImport(User32, SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport(Gdi32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean BitBlt(IntPtr hdc, Int32 nXDest, Int32 nYDest, Int32 nWidth, 
            Int32 nHeight, IntPtr hdcSrc, Int32 nXSrc, Int32 nYSrc, UInt32 dwRop);

        [DllImport(User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, 
            Int32 X, Int32 Y, Int32 cx, Int32 cy, SetWindowPosFlags uFlags);

        [DllImport(User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U2)]
        public static extern Int16 RegisterClassEx([In] ref WNDCLASSEX lpwcx);

        [DllImport(Gdi32, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport(Gdi32, SetLastError = true)]
        public static extern IntPtr CreateFont(Int32 nHeight, Int32 nWidth, Int32 nEscapement,
            Int32 nOrientation, UInt32 fnWeight, UInt32 fdwItalic, UInt32 fdwUnderline, UInt32
                fdwStrikeOut, UInt32 fdwCharSet, UInt32 fdwOutputPrecision, UInt32
                fdwClipPrecision, UInt32 fdwQuality, UInt32 fdwPitchAndFamily, String lpszFace);

        [DllImport(Gdi32, CharSet = CharSet.Unicode)]

        public static extern Boolean GetTextExtentPoint32(IntPtr hdc, String lpString,
            Int32 cbString, out SIZE lpSize);

       

        [DllImport(User32, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern Int32 GetMessage([In, Out] ref MSG msg, IntPtr hWnd, 
            Int32 uMsgFilterMin, Int32 uMsgFilterMax);

        public const UInt32 SRCCOPY = 0x00CC0020;

        public const UInt32 DEFAULT_CHARSET = 1;
        public const UInt32 OUT_OUTLINE_PRECIS = 8;
        public const UInt32 CLIP_DEFAULT_PRECIS = 0;
        public const UInt32 VARIABLE_PITCH = 2;
        public const UInt32 CLEARTYPE_QUALITY = 5;
    }
}
