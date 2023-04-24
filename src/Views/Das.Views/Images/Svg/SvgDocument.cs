using System;

namespace Das.Views.Images.Svg;

// ReSharper disable once ClassNeverInstantiated.Global
public class SvgDocument
{
   public Int32 Width { get;set; }

   public Int32 Height { get;set; }

   public String? ViewBox { get;set; }

   public SvgPath? Path { get; set; }

   public String? Style { get; set; }
}