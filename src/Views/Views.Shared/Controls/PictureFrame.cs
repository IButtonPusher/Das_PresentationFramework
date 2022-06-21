using System;
using System.Threading.Tasks;
using Das.Views.Images;
using Das.Views.Rendering;

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

        public override void Dispose()
        {
            base.Dispose();

            if (!(Image is { } img)) return;
            img.Dispose();
            Image = null;
        }

        //public override void InvalidateMeasure()
        //{
        //   base.InvalidateMeasure();
        //}

        protected override Boolean TryGetImage<TRenderSize>(TRenderSize size,
                                                            IVisualContext visualContext,
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
            DependencyProperty<PictureFrame, IImage?>.Register(nameof(Image), default);

        public IImage? Image
        {
            get => ImageProperty.GetValue(this);
            set => ImageProperty.SetValue(this, value);
        }
    }
}