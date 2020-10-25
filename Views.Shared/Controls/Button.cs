using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views.Controls
{
    // ReSharper disable once UnusedType.Global
    public class Button<T> : ButtonBase<T>
    {
        public override ISize Measure(ISize availableSpace,
                                      IMeasureContext measureContext)
        {
            if (!(Content is {} content))
                return Size.Empty;

            var contentCanHave = GetPaddedSpace(measureContext, availableSpace, out var padding);

            var contentWants = measureContext.MeasureElement(content, contentCanHave);
            return contentWants + padding;
        }

        public override void Arrange(ISize availableSpace,
                                     IRenderContext renderContext)
        {
            if (!(Content is {} content))
                return;

            var contentCanHave = GetPaddedSpace(renderContext, availableSpace, out var padding);
            var contentRect = new ValueRectangle(padding.Left, padding.Top, contentCanHave);

            renderContext.DrawElement(content, contentRect);
        }


        private ISize GetPaddedSpace(IStyleProvider styleContext,
                                     ISize availableSpace,
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