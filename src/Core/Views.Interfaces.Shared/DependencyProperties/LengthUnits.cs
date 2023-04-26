using System;
using System.ComponentModel;

namespace Das.Views;

public enum LengthUnits
{
   Invalid,
   None,
        
   [Description("Centimeters")]
   Cm,

   [Description("Millimeters")]
   Mm,
        
   /// <summary>
   /// Inches
   /// </summary>
   [Description("Inches")]
   In,
        
   /// <summary>
   /// Pixels
   /// </summary>
   [Description("Pixels")]
   Px,
        
   /// <summary>
   /// Points
   /// </summary>
   [Description("Points")]
   Pt,
        
   /// <summary>
   /// Picas
   /// </summary>
   [Description("Picas")]
   Pc,
        
   /// <summary>
   /// Percent
   /// </summary>
   [Description("Percent")]
   Percent
        
}