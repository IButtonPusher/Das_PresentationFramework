using System;
using System.Runtime.InteropServices;
using Das.Views.Core.Geometry;

namespace Das.Gdi.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public static implicit operator Point(POINT point) => new Point(point.X, point.Y);
    }
}