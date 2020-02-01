using System;
using System.Runtime.InteropServices;

namespace Windows.Shared.Messages
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hwnd;
        public int message;
        public IntPtr wParam;
        public IntPtr lParam;
        public int time;

        // pt was a by-value POINT structure
        public int pt_x;
        public int pt_y;
    }
}
