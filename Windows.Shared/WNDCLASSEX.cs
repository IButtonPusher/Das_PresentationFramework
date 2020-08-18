using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Das.Views.Windows
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WNDCLASSEX
    {
        public UInt32 cbSize;
        public ClassStyles style;
        [MarshalAs(UnmanagedType.FunctionPtr)] public UWndProc lpfnWndProc;
        public Int32 cbClsExtra;
        public Int32 cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        public String lpszMenuName;
        public String lpszClassName;
        public IntPtr hIconSm;

        public void Init()
        {
            cbSize = (UInt32) Marshal.SizeOf(this);
        }
    }

    
}