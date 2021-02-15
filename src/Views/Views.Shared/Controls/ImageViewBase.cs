using System;
using Das.Extensions;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Images;
using Das.Views.Rendering;

namespace Das.Views.Controls
{
    public abstract class ImageViewBase : BindableElement
    {
        protected ImageViewBase(IVisualBootstrapper visualBootstrapper) 
            : base(visualBootstrapper)
        {
        }

        public override void Arrange<TRenderSize>(TRenderSize availableSpace,
                                                  IRenderContext renderContext)
        {
            if (!TryGetImage(availableSpace, out var img))
                return;

            ValueRectangle rect;

            if ((!availableSpace.Width.AreEqualEnough(img.Width) ||
                 !availableSpace.Height.AreEqualEnough(availableSpace.Height)) &&
                availableSpace.Height < img.Height)
            {
                var scale = availableSpace.Height / img.Height;
                rect = new ValueRectangle(ValuePoint2D.Empty,
                    new ValueRenderSize(img.Width * scale, availableSpace.Height,
                        availableSpace.Offset));
                //availableSpace = new ValueRenderSize(img.Width * scale, availableSpace.Height,
                //        availableSpace.Offset);
            }
            else
                rect = new ValueRectangle(ValuePoint2D.Empty, availableSpace);

            renderContext.DrawImage(img, rect);
        }

        public override ValueSize Measure<TRenderSize>(TRenderSize availableSpace,
                                                       IMeasureContext measureContext)
        {
            if (!TryGetImage(availableSpace, out var img))
                return ValueSize.Empty;

            var size = measureContext.MeasureImage(img);

            var width = Width ?? Double.NaN; 
            var height = Height ?? Double.NaN;

            if (!Double.IsNaN(width) && !Double.IsNaN(height))
                return new ValueSize(width, height);

            if (!Double.IsNaN(height))
            {
                var scale = height / size.Height;
                return new ValueSize(size.Width * scale, height);
            }

            if (!Double.IsNaN(width))
            {
                var scale = width / size.Height;
                return new ValueSize(height, size.Width * scale);
            }

            return size;
        }

        protected abstract Boolean TryGetImage<TRenderSize>(TRenderSize size,
                                                            out IImage image)
            where TRenderSize : IRenderSize;
    }
}
