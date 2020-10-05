using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views
{
    [StructLayout(LayoutKind.Sequential)]
    // ReSharper disable once InconsistentNaming
    public struct POINT
    {
        public Int32 X;
        public Int32 Y;

        public static implicit operator Point2D(POINT point)
        {
            return new Point2D(point.X, point.Y);
        }
    }
}