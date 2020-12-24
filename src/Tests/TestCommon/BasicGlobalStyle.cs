using System;
using System.Collections.Generic;
using Das.Views;
using Das.Views.Styles;

namespace TestCommon
{
    public class BasicGlobalStyle : IStyle
    {
        public BasicGlobalStyle()
        {
            var fontName = "Segoe UI";

            Setters = new Dictionary<StyleSetters, object>
            {
                { StyleSetters.Margin, new Thickness(0) },
                { StyleSetters.Padding, new Thickness(0) },
                { StyleSetters.BorderThickness, new Thickness(0) },

                { StyleSetters.BorderBrush, new Brush(Color.Transparent) },
                { StyleSetters.Foreground, new Brush(Color.Black) },
                { StyleSetters.Background, new Brush(Color.Transparent) },

                { StyleSetters.FontName, fontName},
                { StyleSetters.FontSize, 12.0},
                { StyleSetters.FontWeight, FontStyle.Regular},
                { StyleSetters.Font, new Font(12, fontName, FontStyle.Regular)},
                { StyleSetters.HorizontalAlignment, HorizontalAlignments.Center },
                { StyleSetters.VerticalAlignment, VerticalAlignments.Center }
            };
        }

        public IDictionary<StyleSetters, object> Setters { get; }
    }
}
