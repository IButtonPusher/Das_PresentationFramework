using System;
using Das.Views.Core;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;

namespace Das.Views.BoxModel
{
    public readonly struct VisualBorder : IBoxValue<BorderSide>,
                                          IBoxValue<IBrush>
    {
        public VisualBorder(BorderSide all)
        : this(all, all, all, all)
        {
            
        }

        public VisualBorder(BorderSide left,
                            BorderSide right,
                            BorderSide top,
                            BorderSide bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;

            IsEmpty = Left.IsEmpty && Right.IsEmpty && Top.IsEmpty && Bottom.IsEmpty;
            
            IsColorUniform = Left.Color.Equals(Right.Color) &&
                             Left.Color.Equals(Top.Color) &&
                             Left.Color.Equals(Bottom.Color);
        }

        public ValueThickness GetThickness<TSize>(TSize available)
            where TSize : ISize
        {
            return new ValueThickness(Left.GetQuantity(available.Width),
                Top.GetQuantity(available.Height),
                Right.GetQuantity(available.Width),
                Bottom.GetQuantity(available.Height));
        }

        public static readonly VisualBorder Empty = new VisualBorder(BorderSide.Empty);

        public BorderSide Left { get; }

        IBrush IBoxValue<IBrush>.Right => Right.Color;

        IBrush IBoxValue<IBrush>.Top => Top.Color;

        IBrush IBoxValue<IBrush>.Bottom => Bottom.Color;

        IBrush IBoxValue<IBrush>.Left => Left.Color;

        public BorderSide Right { get; }

        public BorderSide Top { get; }

        public BorderSide Bottom { get; }

        public readonly Boolean IsEmpty;

        public readonly Boolean IsColorUniform;

    }
}
