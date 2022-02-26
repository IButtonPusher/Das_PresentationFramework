using System;
using Das.Extensions;
using Das.Views.Core;
using Das.Views.Core.Drawing;

namespace Das.Views.BoxModel
{
    public readonly struct BorderSide : IQuantifiedValue<Double, LengthUnits>
    {
        private readonly Double _quantity;

        public BorderSide(Double quantity,
                          LengthUnits units,
                          OutlineStyle style,
                          IBrush color)
        {
            Units = units;
            Style = style;
            Color = color;
            _quantity = quantity;

            IsEmpty = _quantity.IsZero();
        }

        public static readonly BorderSide Empty = new(0, LengthUnits.None, OutlineStyle.None,
            SolidColorBrush.Tranparent);

        public Double GetQuantity(Double available)
        {
            switch (Units)
            {
                case LengthUnits.Percent:
                    return available * (_quantity / 100);

                case LengthUnits.Px:
                case LengthUnits.None:
                    return _quantity;

                default:
                    throw new NotImplementedException();
            }
        }

        public override String ToString()
        {
            return _quantity + " " + Units + " " + Color;
        }

        public LengthUnits Units { get; }

        public OutlineStyle Style { get; }

        public IBrush Color { get; }

        public readonly Boolean IsEmpty;
    }
}
