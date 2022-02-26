using System;
using System.Threading.Tasks;
using Das.Views.Controls;
using Das.Views.Core;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Rendering;

namespace Das.Views.Styles
{
    public enum StyleSetterType
    {
        [StyleType(typeof(Thickness), false)]
        Margin,

        [StyleType(typeof(Thickness), false)]
        Padding,

        [StyleType(typeof(Thickness), false)]
        BorderThickness,

        [StyleType(typeof(Int32), false)]
        BorderRadius,

        [StyleType(typeof(Double), false)]
        Height,

        [StyleType(typeof(Double), false)]
        Width,

        [StyleType(typeof(Font), true)]
        Font,

        [StyleType(typeof(String), true)]
        FontName,

        [StyleType(typeof(IConvertible), true)]
        FontSize,

        [StyleType(typeof(FontStyle), true)]
        FontWeight,

        [StyleType(typeof(IBrush), false)]
        Foreground,

        [StyleType(typeof(IBrush), false)]
        Background,

        [StyleType(typeof(IBrush), false)]
        BorderBrush,

        [StyleType(typeof(VerticalAlignments), false)]
        VerticalAlignment,

        [StyleType(typeof(HorizontalAlignments), false)]
        HorizontalAlignment,

        [StyleType(typeof(Transition[]), false)]
        Transition,

        [StyleType(typeof(Visibility), false)]
        Visibility,
        
        [StyleType(typeof(IVisualTemplate), false)]
        Template,

        [StyleType(typeof(Int32), false)]
        ZIndex

    }
}