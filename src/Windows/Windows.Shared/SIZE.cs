﻿using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Das.Views
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SIZE
    {
        public Int32 cx;
        public Int32 cy;

        public SIZE(Int32 cx, Int32 cy)
        {
            this.cx = cx;
            this.cy = cy;
        }
    }
}