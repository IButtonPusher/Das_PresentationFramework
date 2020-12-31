using System;
using Das.Views.Core.Geometry;

namespace Das.Views.Converters
{
    public class QuantifiedThicknessConverter : BaseConverter<String, QuantifiedThickness>
    {
        public static QuantifiedThicknessConverter Instance { get; } = new QuantifiedThicknessConverter();

        public override QuantifiedThickness Convert(String input)
        {
            return QuantifiedThickness.Parse(input);
        }
    }
}
