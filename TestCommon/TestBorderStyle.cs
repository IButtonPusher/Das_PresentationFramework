using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace TestCommon
{
    public class TestBorderStyle : ElementStyle
    {
        public TestBorderStyle(IVisualElement element) : base(element,
            new BaseStyleContext(new DefaultStyle(), new DefaultColorPalette()))
        {
            AddSetter(StyleSetter.BorderThickness, new Thickness(1,2,3,4));
            AddSetter(StyleSetter.BorderBrush, SolidColorBrush.Red);
        }
    }
}
