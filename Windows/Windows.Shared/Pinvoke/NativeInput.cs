using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;

namespace Das.Views.Windows
{
    [SuppressUnmanagedCodeSecurity]
    public static partial class Native
    {
        [DllImport(User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean GetCursorPos(out POINT lpPoint);

        [DllImport(User32, CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern Int16 GetKeyState(Int32 keyCode);

        [DllImport(User32)]
        public static extern Int16 GetAsyncKeyState(UInt16 virtualKeyCode);
        
        [DllImport(User32)]
        public static extern IntPtr SetCapture(IntPtr hWnd);
        
        [DllImport(User32)]
        public static extern IntPtr GetCapture();
        
        [DllImport(User32)]
        public static extern Boolean ReleaseCapture();

        public const UInt16 VK_LBUTTON = 0x01;
        public const UInt16 VK_RBUTTON = 0x02;
    }
}