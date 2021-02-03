using System;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Images;
using Das.Views.Rendering;

namespace Das.Views.Controls
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once UnusedType.Global
    public class PictureFrame : BindableElement
    {
        public PictureFrame(IVisualBootstrapper visualBootstrapper) 
            : base(visualBootstrapper)
        {
        }

        public override void Arrange(IRenderSize availableSpace,
                                     IRenderContext renderContext)
        {
            var img = Image;
            if (img == null)
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

        public override void Dispose()
        {
            base.Dispose();

            if (!(Image is { } img)) return;
            img.Dispose();
            Image = null;
        }

        public override ValueSize Measure(IRenderSize availableSpace,
                                          IMeasureContext measureContext)
        {
            if (!(Image is {} currentImage))
                return ValueSize.Empty;
            var size = measureContext.MeasureImage(currentImage);

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


        public static readonly DependencyProperty<PictureFrame, IImage?> ImageProperty =
            DependencyProperty<PictureFrame, IImage?>.Register(nameof(Image),
                default);

        public IImage? Image
        {
            get => ImageProperty.GetValue(this);
            set => ImageProperty.SetValue(this, value);
        }
    }
}