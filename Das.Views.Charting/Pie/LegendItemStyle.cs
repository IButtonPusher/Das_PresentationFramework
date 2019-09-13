using System;
using Das.Views.Controls;
using Das.Views.Styles;

namespace Das.Views.Charting.Pie
{
    public class LegendItemStyle: TypeStyle<Label>
    {
        public LegendItemStyle()
        {
            Setters.Add(StyleSetters.FontSize, 9);
        }
    }
}
