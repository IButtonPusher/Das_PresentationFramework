using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Das.Views.Windows
{
    public static partial class Native
    {
        [DllImport(User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean GetCursorPos(out POINT lpPoint);

        [DllImport(User32, CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern Int16 GetKeyState(Int32 keyCode);
    }
}
