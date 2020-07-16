using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Windows.Shared.Messages
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hwnd;
        public Int32 message;
        public IntPtr wParam;
        public IntPtr lParam;
        public Int32 time;

        // pt was a by-value POINT structure
        public Int32 pt_x;
        public Int32 pt_y;
    }
}