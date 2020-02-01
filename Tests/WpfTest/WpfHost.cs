using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace WpfTest
{
    /// <summary>
    /// Win32 window hosting a FrameworkElement
    /// </summary>
    public class WpfHost : HwndSource
    {
        public WpfHost(FrameworkElement element) : base(GetSourceParameters())
        {
            ConvertToChildWindow(Handle);
            CompositionTarget.BackgroundColor = Colors.Yellow;
            SizeToContent = SizeToContent.Manual;
            RootVisual = element;
        }

        public sealed override Visual RootVisual
        {
            get => base.RootVisual;
            set => base.RootVisual = value;
        }

        private static HwndSourceParameters GetSourceParameters()
        {
            var hwsp = new HwndSourceParameters("AddIn");
            hwsp.WindowStyle = 0; // no style, in particular no WM_CHILD
            hwsp.HwndSourceHook = CheeseBurgers;
            return hwsp;
        }

        private static IntPtr CheeseBurgers(IntPtr hwnd, Int32 msg, IntPtr wparam, IntPtr lparam, ref Boolean handled)
        {
            Debug.WriteLine("child: " + msg);
            return IntPtr.Zero;
        }

        public static void ConvertToChildWindow(IntPtr hwnd)
        {
            SetWindowLong(hwnd, GWL_STYLE, WS_CHILD | WS_CLIPCHILDREN);
            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_FRAMECHANGED | SWP_NOMOVE |
                                                        SWP_NOSIZE | SWP_NOZORDER);
        }

        public const Int32 GWL_STYLE = -16;
        public const UInt32 WS_CHILD = 0x40000000, WS_CLIPCHILDREN = 0x02000000, SWP_FRAMECHANGED = 0x0020,
            SWP_NOSIZE = 0x0001,
            SWP_NOMOVE = 0x0002,
            SWP_NOZORDER = 0x0004;

        [DllImport("user32.dll")]
        public static extern Boolean SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, Int32 X, Int32 Y, Int32 cx, Int32 cy, UInt32 uFlags);

        [DllImport("user32.dll")]
        public static extern Int32 SetWindowLong(IntPtr hWnd, Int32 nIndex, UInt32 dwNewLong);
    }
}
