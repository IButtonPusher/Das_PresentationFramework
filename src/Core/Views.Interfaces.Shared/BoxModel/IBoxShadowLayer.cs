using System;
using Das.Views.Core.Drawing;

namespace Das.Views.BoxModel
{
    public interface IBoxShadowLayer
    {
        QuantifiedDouble OffsetX { get; }
        
        QuantifiedDouble OffsetY { get; }
        
        QuantifiedDouble BlurRadius {get;}

        QuantifiedDouble SpreadRadius { get; }

        IBrush Color { get; }

        Boolean IsInset { get; }
    }
}
