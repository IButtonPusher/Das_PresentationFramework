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

        public override void Arrange(IRenderSize availableSpace,
                                     IRenderContext renderContext)
        {
            if (!TryGetImage(out var img))
                return;

            if (!availableSpace.Width.AreEqualEnough(img.Width) ||
                !availableSpace.Height.AreEqualEnough(availableSpace.Height))
                if (availableSpace.Height < img.Height)
                {
                    var scale = availableSpace.Height / img.Height;
                    availableSpace = new ValueRenderSize(img.Width * scale, availableSpace.Height,
                        availableSpace.Offset);
                }

            var rect = new ValueRectangle(ValuePoint2D.Empty, availableSpace);

            renderContext.DrawImage(img, rect);
        }

        public override ValueSize Measure(IRenderSize availableSpace,
                                          IMeasureContext measureContext)
        {
            if (!TryGetImage(out var img))
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

        protected abstract Boolean TryGetImage(out IImage image);
    }
}
