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
            Setters.Add(StyleSetters.Background, new Brush(bg));

            Setters.Add(StyleSetters.Font, new Font(fontSize, "Segoe UI", fontStyle));
            Setters.Add(StyleSetters.FontSize, fontSize);
            Setters.Add(StyleSetters.Margin, new  Thickness(5));
        }
    }
}
