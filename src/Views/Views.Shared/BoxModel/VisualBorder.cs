﻿using System;
using Das.Views.Core;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;

namespace Das.Views.BoxModel
{
    public readonly struct VisualBorder : IBoxValue<BorderSide>,
                                          IVisualBorder
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

        IThickness IVisualBorder.GetThickness<TSize>(TSize available) => GetThickness(available);

        Boolean IVisualBorder.IsEmpty => IsEmpty;

        public ValueThickness GetThickness<TSize>(TSize available)
            where TSize : ISize
        {
            return IsEmpty
            ? ValueThickness.Empty  
            : new(Left.GetQuantity(available.Width),
                Top.GetQuantity(available.Height),
                Right.GetQuantity(available.Width),
                Bottom.GetQuantity(available.Height));
        }

        public override String ToString()
        {
            return "Border - T: " + Top + " R: " + Right + " B: "
                   + Bottom + " L: " + Left;
        }

        public static readonly VisualBorder Empty = new(BorderSide.Empty);

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
