using System;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;

namespace Das.Views.BoxModel
{
    public class BoxShadowLayer : IBoxShadowLayer
    {
        public BoxShadowLayer(QuantifiedDouble offsetX, 
                              QuantifiedDouble offsetY, 
                              QuantifiedDouble blurRadius, 
                              QuantifiedDouble spreadRadius, 
                              IBrush color, 
                              Boolean isInset)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
            BlurRadius = blurRadius;
            SpreadRadius = spreadRadius;
            Color = color;
            IsInset = isInset;
        }

        public QuantifiedDouble OffsetX { get; }

        public QuantifiedDouble OffsetY { get; }

        public QuantifiedDouble BlurRadius { get; }

        public QuantifiedDouble SpreadRadius { get; }

        public IBrush Color { get; }

        public Boolean IsInset { get; }
    }
}