using System;
using System.Threading.Tasks;
using Das.Views.Core.Enums;
using Das.Views.Rendering;

namespace Das.Views.Measuring
{
    public static class MeasureHelper
    {
        public static DimensionResult MeasureVisual<TVisual, TRenderSize>(TVisual visual,
                                                                          TRenderSize availableSpace,
                                                                          out Double desiredWidth,
                                                                          out Double desiredHeight)
            where TVisual : IVisualElement
            where TRenderSize : IRenderSize
        {
            DimensionResult result;

            if (visual.Width is { } vWidth)
            {
                desiredWidth = vWidth.GetQuantity(availableSpace.Width);
                result = DimensionResult.Width;
            }
            else if (visual.HorizontalAlignment == HorizontalAlignments.Stretch)
            {
                desiredWidth = availableSpace.Width;
                result = DimensionResult.Width;
            }
            else
            {
                desiredWidth = Double.NaN;
                result = DimensionResult.None;
            }


            if (visual.Height is { } vHeight)
            {
                desiredHeight = vHeight.GetQuantity(availableSpace.Height);
                result |= DimensionResult.Height;
            }
            else if (visual.VerticalAlignment == VerticalAlignments.Stretch)
            {
                desiredHeight = availableSpace.Height;
                result |= DimensionResult.Height;
            }
            else
                desiredHeight = Double.NaN;

            return result;
        }
    }
}
