using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Windows
{
    public struct RECT
    {
        public Int32 left;
        public Int32 top;
        public Int32 right;
        public Int32 bottom;

        public RECT(Int32 left, Int32 top, Int32 right, Int32 bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public RECT(Rectangle r)
        {
            left = Convert.ToInt32(r.Left);
            top = Convert.ToInt32(r.Top);
            right = Convert.ToInt32(r.Right);
            bottom = Convert.ToInt32(r.Bottom);
        }

        public static RECT FromXYWH(Int32 x, Int32 y, Int32 width, Int32 height)
        {
            return new RECT(x, y, x + width, y + height);
        }

        public SIZE Size => new SIZE(right - left, bottom - top);
    }
}