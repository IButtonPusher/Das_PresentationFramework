using Das.Views;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace TestCommon
{
    public sealed class TestBorderStyle : ElementStyle
    {
        public TestBorderStyle(IVisualElement element) : base(element,
            new BaseStyleContext(DefaultStyle.Instance, new DefaultColorPalette()))
        {
            AddSetter(StyleSetterType.BorderThickness, new Thickness(1,2,3,4));
            AddSetter(StyleSetterType.BorderBrush, SolidColorBrush.Red);
        }
    }
}
