using System;
using System.Threading.Tasks;
using Das.Views.Images;

namespace Das.Views.Controls
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once UnusedType.Global
    public class PictureFrame : ImageViewBase
    {
        public PictureFrame(IVisualBootstrapper visualBootstrapper) 
            : base(visualBootstrapper)
        {
        }

        //public override void Arrange<TRenderSize>(TRenderSize availableSpace,
        //                                          IRenderContext renderContext)
        //{
        //    var img = Image;
        //    if (img == null)
        //        return;

        //    ValueRectangle rect;

        //    if ((!availableSpace.Width.AreEqualEnough(img.Width) ||
        //        !availableSpace.Height.AreEqualEnough(availableSpace.Height)) &&
        //        availableSpace.Height < img.Height)
        //        {
        //            var scale = availableSpace.Height / img.Height;
        //            rect = new ValueRectangle(ValuePoint2D.Empty,
        //                new ValueRenderSize(img.Width * scale, availableSpace.Height,
        //                    availableSpace.Offset));
        //            //availableSpace = new ValueRenderSize(img.Width * scale, availableSpace.Height,
        //            //    availableSpace.Offset);
        //        }
        //        else
        //            rect = new ValueRectangle(ValuePoint2D.Empty, availableSpace);

        //    renderContext.DrawImage(img, rect);
        //}

        public override void Dispose()
        {
            base.Dispose();

            if (!(Image is { } img)) return;
            img.Dispose();
            Image = null;
        }

        //public override ValueSize Measure<TRenderSize>(TRenderSize availableSpace,
        //                                              IMeasureContext measureContext)
        //{
        //    if (!(Image is {} currentImage))
        //        return ValueSize.Empty;
        //    var size = measureContext.MeasureImage(currentImage);

        //    var width = Width ?? Double.NaN; 
        //    var height = Height ?? Double.NaN;

        //    if (!Double.IsNaN(width) && !Double.IsNaN(height))
        //        return new ValueSize(width, height);

        //    if (!Double.IsNaN(height))
        //    {
        //        var scale = height / size.Height;
        //        return new ValueSize(size.Width * scale, height);
        //    }

        //    if (!Double.IsNaN(width))
        //    {
        //        var scale = width / size.Height;
        //        return new ValueSize(height, size.Width * scale);
        //    }

        //    return size;
        //}

        protected override Boolean TryGetImage(Int32 width,
                                               Int32 height,
                                               out IImage image)
        {
            if (Image is { } img)
            {
                image = img;
                return true;
            }

            image = default!;
            return false;
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