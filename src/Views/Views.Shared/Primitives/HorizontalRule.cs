using System;
using System.Threading.Tasks;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;

namespace Das.Views
{
   public sealed class HorizontalRule : VisualElement
   {
      public HorizontalRule(IVisualBootstrapper visualBootstrapper)
         : base(visualBootstrapper)
      {
         HorizontalAlignment = HorizontalAlignments.Stretch;

         InvalidateMeasure();
      }

      public override void Arrange<TRenderSize>(TRenderSize availableSpace,
                                                IRenderContext renderContext)
      {
         if (Background is not { } bg)
            return;

         var r = availableSpace.ToFullRectangle();

         renderContext.FillRectangle(r, bg);

         //var bg = Background ?? renderContext.ColorPalette.GetAlpha(ColorType.OnBackground, .2);
         //var r = availableSpace.ToFullRectangle();

         //renderContext.FillRectangle(r, bg);
      }

      public override ValueSize Measure<TRenderSize>(TRenderSize availableSpace,
                                                     IMeasureContext measureContext)
      {
         if (!(measureContext.ViewState is { }))
            return new ValueSize(availableSpace.Width, 1);

         var specificHeight = Height?.GetQuantity(availableSpace.Height) ?? Double.NaN;

         specificHeight = Double.IsNaN(specificHeight) ? availableSpace.Height : specificHeight;

         //            var specificWidth = measureContext.GetStyleSetter<Double>(StyleSetterType.Width, this);
         var specificWidth = Width?.GetQuantity(availableSpace.Width) ?? Double.NaN;

         specificWidth = Double.IsNaN(specificWidth) ? availableSpace.Width : specificWidth;

         return new ValueSize(specificWidth, specificHeight);
      }
   }
}
