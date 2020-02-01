using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace WpfTest
{
    public class Win32Host : HwndHost
    {
        private readonly IntPtr _child;
        private readonly WpfHost _wpf;
        

        public Win32Host(WpfHost wpf)
        {
            _child = wpf.Handle;
            _wpf = wpf;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var returning = base.ArrangeOverride(finalSize);
            //_sizor.Arrange(new Rect(new Point(0, 0), finalSize));

            GetWindowRect(Handle, out var rekt);

            var ww = rekt.Right - rekt.Left;
            var wh = rekt.Bottom - rekt.Top;
            
            Debug.WriteLine("'window' w is " + ww + " h " + wh + " actual ctrl size: " +
                            " "  + " arranging to " + finalSize);

            return returning;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            SetParent(_child, hwndParent.Handle);

            var hwnd = new HandleRef(this, _child);

            return hwnd;
        }
        private const string PresentationNativeDll = "PresentationNative_v0400.dll";
        
        

        public delegate IntPtr WndProc(IntPtr hWnd, Int32 msg, IntPtr wParam, IntPtr lParam);
        

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
    }

    public struct RECT
    {
        public int Left;        // x position of upper-left corner
        public int Top;         // y position of upper-left corner
        public int Right;       // x position of lower-right corner
        public int Bottom;      // y position of lower-right corner
    }
}
