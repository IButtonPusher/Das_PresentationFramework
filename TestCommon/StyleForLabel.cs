using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace TestCommon
{
    public class StyleForLabel : ElementStyle
    {
        public StyleForLabel(IVisualElement element) : 
            this(element, 18, FontStyle.Bold, Color.Orange)
        {
            
        }

        public StyleForLabel(IVisualElement element, double fontSize, FontStyle fontStyle,
            Color bg) 
            : base(element)
        {
            AddSetter(StyleSetter.Background, new SolidColorBrush(bg));

            AddSetter(StyleSetter.Font, new Font(fontSize, "Segoe UI", fontStyle));
            AddSetter(StyleSetter.FontSize, fontSize);
            AddSetter(StyleSetter.Margin, new  Thickness(5));
        }
    }
}
