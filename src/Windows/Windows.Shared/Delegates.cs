﻿using System;
using System.Threading.Tasks;

namespace Das.Views.Windows
{
    public delegate IntPtr UWndProc(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

    public delegate IntPtr WndProc(IntPtr hWnd, Int32 msg, IntPtr wParam, IntPtr lParam);
}