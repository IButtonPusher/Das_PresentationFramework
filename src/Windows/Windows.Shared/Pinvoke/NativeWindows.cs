﻿using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Das.Views.Windows
{
    public static partial class Native
    {
        [DllImport(User32, SetLastError = true)]
        public static extern IntPtr CreateWindowEx(
            WindowStylesEx dwExStyle,
            String lpClassName,
            String lpWindowName,
            WindowStyles dwStyle,
            Int32 x,
            Int32 y,
            Int32 nWidth,
            Int32 nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam);

        [DllImport(User32, SetLastError = true, EntryPoint = "DestroyWindow",
            CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 DestroyWindow(IntPtr handle);

        [DllImport(User32, EntryPoint = "GetWindowLong")]
        public static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, Int32 nIndex);

        [DllImport(User32, EntryPoint = "GetWindowLongPtr")]
        public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, Int32 nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern Int32 SetWindowLong32(HandleRef hWnd, Int32 nIndex, Int32 dwNewLong);

        public static IntPtr SetWindowLongPtr(HandleRef hWnd, Int32 nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }

        [DllImport(User32, EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, Int32 nIndex, IntPtr dwNewLong);

        [DllImport(User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
                                                  Int32 X, Int32 Y, Int32 cx, Int32 cy, SetWindowPosFlags uFlags);
    }
}