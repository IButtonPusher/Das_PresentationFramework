using System;
using System.ComponentModel;

namespace Das.Views.Styles.Declarations
{
    public enum LengthUnits
    {
        Invalid,
        None,
        
        [Description("Centimeters")]
        Cm,
        
        [Description("Millimeters")]
        Mm,
        
        [Description("Inches")]
        In,
        
        [Description("Pixels")]
        Px,
        
        [Description("Points")]
        Pt,
        
        [Description("Picas")]
        Pc,
        
        [Description("Percent")]
        Percent
        
    }
}
