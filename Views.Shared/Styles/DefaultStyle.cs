using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;

namespace Das.Views.Styles
{
    public class DefaultStyle : Style
    {
        public DefaultStyle()
        {
            var font = new Font(10, FontName, FontStyle.Regular);

            Setters = new Dictionary<StyleSetters, Object?>
            {
                {StyleSetters.Margin, new Thickness(0)},
                {StyleSetters.Padding, new Thickness(0)},
                {StyleSetters.BorderThickness, new Thickness(0)},

                {StyleSetters.BorderBrush, new SolidColorBrush(Color.Transparent)},
                {StyleSetters.Foreground, new SolidColorBrush(Color.Black)},
                {StyleSetters.Background, new SolidColorBrush(Color.Transparent)},

                {StyleSetters.FontName, font.FamilyName},
                {StyleSetters.FontSize, font.Size},
                {StyleSetters.FontWeight, font.FontStyle},
                {StyleSetters.Font, font},
                {StyleSetters.HorizontalAlignment, HorizontalAlignments.Center},
                {StyleSetters.VerticalAlignment, VerticalAlignments.Center},
                {StyleSetters.Size, null},
                {StyleSetters.Height, Double.NaN},
                {StyleSetters.Width, Double.NaN}
            };
        }

        public const String FontName = "Segoe UI";
    }
}