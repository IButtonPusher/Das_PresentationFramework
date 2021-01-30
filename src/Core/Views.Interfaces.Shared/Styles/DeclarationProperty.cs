using System;
// ReSharper disable UnusedMember.Global

namespace Das.Views.Styles.Declarations
{
    /// <summary>
    /// Cascading style sheet properties (zb box-shadow, width, color)
    /// </summary>
    public enum DeclarationProperty
    {
        Invalid,
        
        Appearance,
        BackgroundColor,

        Border,
        BorderColor,
        BorderStyle,
        BorderWidth,
        
        BorderBottom,
        BorderBottomColor,
        BorderBottomStyle,
        BorderBottomWidth,

        BorderLeft,
        BorderLeftColor,
        BorderLeftStyle,
        BorderLeftWidth,

        BorderRight,
        BorderRightColor,
        BorderRightStyle,
        BorderRightWidth,

        BorderTop,
        BorderTopColor,
        BorderTopStyle,
        BorderTopWidth,
        
        BorderRadius,
        BorderRadiusBottom,
        BorderRadiusLeft,
        BorderRadiusRight,
        BorderRadiusTop,
        
        Bottom,
        BoxShadow,
        Color,
        Content,
        Cursor,
        Display,
        
        Float,
        FontFamily,
        FontSize,
        Height,
        Left,
        LineHeight,
        
        Margin,
        MarginBottom,
        MarginLeft,
        MarginRight,
        MarginTop,

        Opacity,
        
        Outline,
        OutlineColor,
        OutlineOffset,
        OutlineStyle,
        OutlineWidth,

        Padding,
        PaddingBottom,
        PaddingLeft,
        PaddingRight,
        PaddingTop,
        
        PointerEvents,
        Position,
        Right,
        Top,
        Transform,
        
        Transition,
        TransitionDelay,
        TransitionDuration,
        TransitionProperty,
        TransitionTimingFunction,
        
        VerticalAlign,
        
        Width,
        ZIndex
    }
}
