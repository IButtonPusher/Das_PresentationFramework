﻿using Das.Views;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Styles;
// ReSharper disable VirtualMemberCallInConstructor

namespace TestCommon
{
    public class StyleForLabel : ElementStyle
    {
        public StyleForLabel(IVisualElement element, 
                             IStyleContext styleContext) : 
            this(element, 18, FontStyle.Bold, Color.Orange, styleContext)
        {
            
        }

        public StyleForLabel(IVisualElement element, 
                             double fontSize, 
                             FontStyle fontStyle,
            Color bg,
                             IStyleContext styleContext) 
            : base(element, styleContext)
        {
            AddSetter(StyleSetterType.Background, new SolidColorBrush(bg));

            AddSetter(StyleSetterType.Font, new Font(fontSize, "Segoe UI", fontStyle));
            AddSetter(StyleSetterType.FontSize, fontSize);
            AddSetter(StyleSetterType.Margin, new  Thickness(5));
        }
    }
}
