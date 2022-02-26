using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts
{
    public struct LayoutOffset
    {
        public Int32 dx;
        public Int32 dy;

        public LayoutOffset(Int32 dx,
                            Int32 dy)
        {
            this.dx = dx;
            this.dy = dy;
        }
    }
}
