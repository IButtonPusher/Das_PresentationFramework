using System;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;

namespace Das.Views.Styles
{
    public enum StyleSetters
    {
        [StyleType(typeof(Thickness))] Margin,

        [StyleType(typeof(Thickness))] Padding,

        [StyleType(typeof(Thickness))] BorderThickness,

        [StyleType(typeof(ISize))] Size,

        [StyleType(typeof(Double))] Height,

        [StyleType(typeof(Double))] Width,

        [StyleType(typeof(Font))] Font,
        [StyleType(typeof(String))] FontName,

        [StyleType(typeof(IConvertible))] FontSize,

        [StyleType(typeof(FontStyle))] FontWeight,

        [StyleType(typeof(Brush))] Foreground,

        [StyleType(typeof(Brush))] Background,

        [StyleType(typeof(Brush))] BorderBrush,

        [StyleType(typeof(VerticalAlignments))]
        VerticalAlignment,

        [StyleType(typeof(HorizontalAlignments))]
        HorizontalAlignment
    }
}