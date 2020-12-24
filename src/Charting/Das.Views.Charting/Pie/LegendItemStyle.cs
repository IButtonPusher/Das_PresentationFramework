using Das.Views.Controls;
using Das.Views.Styles;

namespace Das.Views.Charting.Pie
{
    public class LegendItemStyle: TypeStyle<Label>
    {
        public LegendItemStyle()
        {
            AddSetter(StyleSetter.FontSize, 9);
        }
    }
}
