using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;
using Das.Views.Styles;

namespace Das.Views.Controls
{
    // ReSharper disable once UnusedType.Global
    public class Button<T> : ButtonBase<T>
    {
        public override ISize Measure(IRenderSize availableSpace,
                                      IMeasureContext measureContext)
        {
            if (!(Content is {} content))
                return Size.Empty;

            var contentCanHave = GetPaddedSpace(measureContext, 
                availableSpace, out var padding);

            var contentWants = measureContext.MeasureElement(content, contentCanHave);
            var ambition = contentWants + padding;
            if (ambition.Width > availableSpace.Width ||
                ambition.Height > availableSpace.Height)
                return availableSpace;
            return contentWants + padding;
        }

        public override void Arrange(IRenderSize availableSpace,
                                     IRenderContext renderContext)
        {
            if (!(Content is {} content))
                return;

            var contentCanHave = GetPaddedSpace(renderContext, availableSpace, out var padding);
            var contentRect = new ValueRenderRectangle(padding.Left, padding.Top, contentCanHave,
                Point2D.Empty);

            renderContext.DrawElement(content, contentRect);
        }


        private IRenderSize GetPaddedSpace(IStyleProvider styleContext,
                                           IRenderSize availableSpace,
                                           out Thickness padding)
        {
            padding = styleContext.GetStyleSetter<Thickness>(StyleSetter.Padding,
                CurrentStyleSelector, this);

            return padding.IsEmpty
                ? availableSpace
                : availableSpace.Reduce(padding);
        }

       
    }
}