using System;
using System.Threading.Tasks;
using Das.Views.Core;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Rendering;

namespace Das.Views.Styles
{
    public enum StyleSetter
    {
        [StyleType(typeof(Thickness))]
        Margin,

        [StyleType(typeof(Thickness))]
        Padding,

        [StyleType(typeof(Thickness))]
        BorderThickness,

        [StyleType(typeof(Int32))]
        BorderRadius,

        //[StyleType(typeof(ISize))]
        //Size,

        [StyleType(typeof(Double))]
        Height,

        [StyleType(typeof(Double))]
        Width,

        [StyleType(typeof(Font))]
        Font,

        [StyleType(typeof(String))]
        FontName,

        [StyleType(typeof(IConvertible))]
        FontSize,

        [StyleType(typeof(FontStyle))]
        FontWeight,

        [StyleType(typeof(SolidColorBrush))]
        Foreground,

        [StyleType(typeof(SolidColorBrush))]
        Background,

        [StyleType(typeof(SolidColorBrush))]
        BorderBrush,

        [StyleType(typeof(VerticalAlignments))]
        VerticalAlignment,

        [StyleType(typeof(HorizontalAlignments))]
        HorizontalAlignment,

        [StyleType(typeof(Transition[]))]
        Transition,

        [StyleType(typeof(Visibility))]
        Visibility


    }
}