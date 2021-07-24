using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Shared;
using Windows.Shared.Messages;

// ReSharper disable UnusedMember.Global

namespace Das.Views.Windows
{
    public static partial class Native
    {
        [DllImport(Gdi32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean BitBlt(IntPtr hdc, Int32 nXDest, Int32 nYDest, Int32 nWidth,
                                            Int32 nHeight, IntPtr hdcSrc, Int32 nXSrc, Int32 nYSrc, UInt32 dwRop);

        [DllImport(Gdi32, SetLastError = true)]
        public static extern Int32 ChoosePixelFormat(IntPtr hDC,
                                                     [In] [MarshalAs(UnmanagedType.LPStruct)]
                                                     Pixelformatdescriptor ppfd);

        [DllImport(Gdi32, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport(Gdi32, EntryPoint = "CreateDC", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateDC(
            String lpszDriver,
            String lpszDeviceName,
            String lpszOutput,
            HandleRef devMode);

        [DllImport(Gdi32)]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref BitMapInfo pbmi, UInt32 iUsage,
                                                     out IntPtr ppvBits, IntPtr hSection, UInt32 dwOffset);

        [DllImport(Gdi32, SetLastError = true)]
        public static extern IntPtr CreateFont(Int32 nHeight, Int32 nWidth, Int32 nEscapement,
                                               Int32 nOrientation, UInt32 fnWeight, UInt32 fdwItalic,
                                               UInt32 fdwUnderline, UInt32
                                                   fdwStrikeOut, UInt32 fdwCharSet, UInt32 fdwOutputPrecision, UInt32
                                                   fdwClipPrecision, UInt32 fdwQuality, UInt32 fdwPitchAndFamily,
                                               String lpszFace);

        [DllImport(User32, SetLastError = true)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, UInt32 uMsg, IntPtr wParam, IntPtr lParam);


        [DllImport(User32, SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);


        [DllImport(User32, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern Int32 GetMessage([In] [Out] ref MSG msg, IntPtr hWnd,
                                                Int32 uMsgFilterMin, Int32 uMsgFilterMax);

        [DllImport(User32, CharSet = CharSet.Auto)]
        public static extern Boolean GetMonitorInfo(IntPtr hMonitor, ref MonitorInfoEx lpmi);

        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //public static extern Boolean GetMonitorInfo(HandleRef hmonitor, [In] [Out] MONITORINFOEX info);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 GetSystemMetrics(Int32 nIndex);

        [DllImport(Gdi32, CharSet = CharSet.Unicode)]
        public static extern Boolean GetTextExtentPoint32(IntPtr hdc, String lpString,
                                                          Int32 cbString, out SIZE lpSize);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(HandleRef handle, Int32 flags);

        [DllImport(User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U2)]
        public static extern Int16 RegisterClassEx([In] ref WNDCLASSEX lpwcx);

        [DllImport(Gdi32)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiObj);

        [DllImport(Gdi32, SetLastError = true)]
        public static extern Int32 SetPixelFormat(IntPtr hDC, Int32 iPixelFormat,
                                                  [In] [MarshalAs(UnmanagedType.LPStruct)]
                                                  Pixelformatdescriptor ppfd);


        public const String Gdi32 = "gdi32.dll";
        public const String User32 = "user32.dll";

        public const String Core = "coredll.dll";

        public const UInt32 SRCCOPY = 0x00CC0020;

        public const UInt32 DEFAULT_CHARSET = 1;
        public const UInt32 OUT_OUTLINE_PRECIS = 8;
        public const UInt32 CLIP_DEFAULT_PRECIS = 0;
        public const UInt32 VARIABLE_PITCH = 2;
        public const UInt32 CLEARTYPE_QUALITY = 5;


        #region Window Handling

        #endregion

        #region General Definitions

        /// <summary>
        ///     Places (posts) a message in the message queue associated with the thread that created
        ///     the specified window and returns without waiting for the thread to process the message.
        /// </summary>
        /// <param name="windowHandle">
        ///     Handle to the window whose window procedure will receive the message.
        ///     If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system,
        ///     including disabled or invisible unowned windows, overlapped windows, and pop-up windows;
        ///     but the message is not sent to child windows.
        /// </param>
        /// <param name="message">Specifies the message to be sent.</param>
        /// <param name="wparam">Specifies additional message-specific information.</param>
        /// <param name="lparam">Specifies additional message-specific information.</param>
        /// <returns>A return code specific to the message being sent.</returns>
        [DllImport(User32, CharSet = CharSet.Auto, PreserveSig = false, SetLastError = true)]
        public static extern void PostMessage(
            IntPtr windowHandle,
            MessageTypes message,
            IntPtr wparam,
            IntPtr lparam
        );

        /// <summary>
        ///     Sends the specified message to a window or windows. The SendMessage function calls
        ///     the window procedure for the specified window and does not return until the window
        ///     procedure has processed the message.
        /// </summary>
        /// <param name="windowHandle">
        ///     Handle to the window whose window procedure will receive the message.
        ///     If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system,
        ///     including disabled or invisible unowned windows, overlapped windows, and pop-up windows;
        ///     but the message is not sent to child windows.
        /// </param>
        /// <param name="message">Specifies the message to be sent.</param>
        /// <param name="wparam">Specifies additional message-specific information.</param>
        /// <param name="lparam">Specifies additional message-specific information.</param>
        /// <returns>A return code specific to the message being sent.</returns>
        [DllImport(User32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(
            IntPtr windowHandle,
            MessageTypes message,
            IntPtr wparam,
            IntPtr lparam
        );

        /// <summary>
        ///     Sends the specified message to a window or windows. The SendMessage function calls
        ///     the window procedure for the specified window and does not return until the window
        ///     procedure has processed the message.
        /// </summary>
        /// <param name="windowHandle">
        ///     Handle to the window whose window procedure will receive the message.
        ///     If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system,
        ///     including disabled or invisible unowned windows, overlapped windows, and pop-up windows;
        ///     but the message is not sent to child windows.
        /// </param>
        /// <param name="message">Specifies the message to be sent.</param>
        /// <param name="wparam">Specifies additional message-specific information.</param>
        /// <param name="lparam">Specifies additional message-specific information.</param>
        /// <returns>A return code specific to the message being sent.</returns>
        [DllImport(User32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(
            IntPtr windowHandle,
            UInt32 message,
            IntPtr wparam,
            IntPtr lparam
        );

        /// <summary>
        ///     Sends the specified message to a window or windows. The SendMessage function calls
        ///     the window procedure for the specified window and does not return until the window
        ///     procedure has processed the message.
        /// </summary>
        /// <param name="windowHandle">
        ///     Handle to the window whose window procedure will receive the message.
        ///     If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system,
        ///     including disabled or invisible unowned windows, overlapped windows, and pop-up windows;
        ///     but the message is not sent to child windows.
        /// </param>
        /// <param name="message">Specifies the message to be sent.</param>
        /// <param name="wparam">Specifies additional message-specific information.</param>
        /// <param name="lparam">Specifies additional message-specific information.</param>
        /// <returns>A return code specific to the message being sent.</returns>
        [DllImport(User32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(
            IntPtr windowHandle,
            UInt32 message,
            IntPtr wparam,
            [MarshalAs(UnmanagedType.LPWStr)] String lparam);

        /// <summary>
        ///     Sends the specified message to a window or windows. The SendMessage function calls
        ///     the window procedure for the specified window and does not return until the window
        ///     procedure has processed the message.
        /// </summary>
        /// <param name="windowHandle">
        ///     Handle to the window whose window procedure will receive the message.
        ///     If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system,
        ///     including disabled or invisible unowned windows, overlapped windows, and pop-up windows;
        ///     but the message is not sent to child windows.
        /// </param>
        /// <param name="message">Specifies the message to be sent.</param>
        /// <param name="wparam">Specifies additional message-specific information.</param>
        /// <param name="lparam">Specifies additional message-specific information.</param>
        /// <returns>A return code specific to the message being sent.</returns>
        public static IntPtr SendMessage(
            IntPtr windowHandle,
            UInt32 message,
            Int32 wparam,
            String lparam)
        {
            return SendMessage(windowHandle, message, (IntPtr) wparam, lparam);
        }

        /// <summary>
        ///     Sends the specified message to a window or windows. The SendMessage function calls
        ///     the window procedure for the specified window and does not return until the window
        ///     procedure has processed the message.
        /// </summary>
        /// <param name="windowHandle">
        ///     Handle to the window whose window procedure will receive the message.
        ///     If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system,
        ///     including disabled or invisible unowned windows, overlapped windows, and pop-up windows;
        ///     but the message is not sent to child windows.
        /// </param>
        /// <param name="message">Specifies the message to be sent.</param>
        /// <param name="wparam">Specifies additional message-specific information.</param>
        /// <param name="lparam">Specifies additional message-specific information.</param>
        /// <returns>A return code specific to the message being sent.</returns>
        [DllImport(User32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(
            IntPtr windowHandle,
            UInt32 message,
            ref Int32 wparam,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lparam);

        // Various helpers for forcing binding to proper 
        // version of Comctl32 (v6).
        [DllImport("kernel32.dll", SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
        public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] String fileName);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean DeleteObject(IntPtr graphicsObjectHandle);

        [DllImport(User32, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern Int32 LoadString(
            IntPtr instanceHandle,
            Int32 id,
            StringBuilder buffer,
            Int32 bufferSize);

        [DllImport("Kernel32.dll", EntryPoint = "LocalFree")]
        public static extern IntPtr LocalFree(ref Guid guid);

        /// <summary>
        ///     Destroys an icon and frees any memory the icon occupied.
        /// </summary>
        /// <param name="hIcon">Handle to the icon to be destroyed. The icon must not be in use. </param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. To get
        ///     extended error information, call GetLastError.
        /// </returns>
        [DllImport(User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean DestroyIcon(IntPtr hIcon);

        #endregion

        #region General Declarations

        // Various important window messages
        public const Int32 UserMessage = 0x0400;
        public const Int32 EnterIdleMessage = 0x0121;

        // FormatMessage constants and structs.
        public const Int32 FormatMessageFromSystem = 0x00001000;

        // App recovery and restart return codes
        public const UInt32 ResultFailed = 0x80004005;
        public const UInt32 ResultInvalidArgument = 0x80070057;
        public const UInt32 ResultFalse = 1;
        public const UInt32 ResultNotFound = 0x80070490;

        /// <summary>
        ///     Gets the HiWord
        /// </summary>
        /// <param name="value">The value to get the hi word from.</param>
        /// <param name="size">Size</param>
        /// <returns>The upper half of the dword.</returns>
        public static Int32 GetHiWord(Int64 value, Int32 size)
        {
            return (Int16) (value >> size);
        }

        /// <summary>
        ///     Gets the LoWord
        /// </summary>
        /// <param name="value">The value to get the low word from.</param>
        /// <returns>The lower half of the dword.</returns>
        public static Int32 GetLoWord(Int64 value)
        {
            return (Int16) (value & 0xFFFF);
        }

        #endregion

        #region GDI and DWM Declarations

        /// <summary>
        ///     A Wrapper for a SIZE struct
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Size
        {
            /// <summary>
            ///     Width
            /// </summary>
            public Int32 Width { get; set; }

            /// <summary>
            ///     Height
            /// </summary>
            public Int32 Height { get; set; }
        }

        // Enable/disable non-client rendering based on window style.
        public const Int32 DWMNCRP_USEWINDOWSTYLE = 0;

        // Disabled non-client rendering; window style is ignored.
        public const Int32 DWMNCRP_DISABLED = 1;

        // Enabled non-client rendering; window style is ignored.
        public const Int32 DWMNCRP_ENABLED = 2;

        // Enable/disable non-client rendering Use DWMNCRP_* values.
        public const Int32 DWMWA_NCRENDERING_ENABLED = 1;

        // Non-client rendering policy.
        public const Int32 DWMWA_NCRENDERING_POLICY = 2;

        // Potentially enable/forcibly disable transitions 0 or 1.
        public const Int32 DWMWA_TRANSITIONS_FORCEDISABLED = 3;

        #endregion

        #region Windows OS structs and consts

        public const UInt32 StatusAccessDenied = 0xC0000022;


        public delegate Int32 WNDPROC(IntPtr hWnd,
                                      UInt32 uMessage,
                                      IntPtr wParam,
                                      IntPtr lParam);

        #endregion
    }
}