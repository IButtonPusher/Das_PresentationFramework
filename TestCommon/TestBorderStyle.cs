using System;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace TestCommon
{
    public class TestBorderStyle : ElementStyle
    {
        public TestBorderStyle(IVisualElement element) : base(element)
        {
            Setters.Add(StyleSetters.BorderThickness, new Thickness(1,2,3,4));
            Setters.Add(StyleSetters.BorderBrush, Brush.Red);
        }
    }
}
