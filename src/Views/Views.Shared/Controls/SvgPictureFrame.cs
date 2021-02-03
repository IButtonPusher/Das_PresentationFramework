using System;
using Das.Views.Core.Drawing;
using Das.Views.Images;

namespace Das.Views.Controls
{
    public class SvgPictureFrame : ImageViewBase
    {
        public SvgPictureFrame(IVisualBootstrapper visualBootstrapper) 
            : base(visualBootstrapper)
        {
        }

        protected override Boolean TryGetImage(out IImage image)
        {
            if (_image is { } img)
            {
                image = img;
                return true;
            }

            if (Source is { } source && 
                (Stroke != null || 
                 Fill != null))
            {
                image = source.ToStaticImage(Stroke, Fill);
                _image = image;
                return true;
            }

            image = default!;
            return false;
        }

        public static readonly DependencyProperty<SvgPictureFrame, ISvgImage?> SourceProperty =
            DependencyProperty<SvgPictureFrame, ISvgImage?>.Register(
                nameof(Source), default);

        public ISvgImage? Source
        {
            get => SourceProperty.GetValue(this);
            set => SourceProperty.SetValue(this, value);
        }

        public static readonly DependencyProperty<SvgPictureFrame, IColor?> StrokeProperty =
            DependencyProperty<SvgPictureFrame, IColor?>.Register(
                nameof(Stroke),
                default);

        public IColor? Stroke
        {
            get => StrokeProperty.GetValue(this);
            set => StrokeProperty.SetValue(this, value);
        }

        public static readonly DependencyProperty<SvgPictureFrame, IBrush?> FillProperty =
            DependencyProperty<SvgPictureFrame, IBrush?>.Register(
                nameof(Fill),
                default);

        public IBrush? Fill
        {
            get => FillProperty.GetValue(this);
            set => FillProperty.SetValue(this, value);
        }

        private IImage? _image;
    }
}
