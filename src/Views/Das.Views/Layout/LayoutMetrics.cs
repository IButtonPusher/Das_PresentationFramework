using System;
using System.Threading.Tasks;
using Das.Views.Text;

namespace Das.Views.Layout
{
    public struct LayoutMetrics
    {
        public TextFlowDirection Direction;
        public UInt16 DesignEmHeight;
        public UInt16 PixelsEmWidth;
        public UInt16 PixelsEmHeight;

        public LayoutMetrics(TextFlowDirection Direction,
                             UInt16 DesignEmHeight,
                             UInt16 PixelsEmWidth,
                             UInt16 PixelsEmHeight)
        {
            this.Direction = Direction;
            this.DesignEmHeight = DesignEmHeight;
            this.PixelsEmWidth = PixelsEmWidth;
            this.PixelsEmHeight = PixelsEmHeight;
        }
    }
}
