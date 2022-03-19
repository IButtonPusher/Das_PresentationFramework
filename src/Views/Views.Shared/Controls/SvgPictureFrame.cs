using System;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Images;
using Das.Views.Measuring;
using Das.Views.Rendering;

namespace Das.Views.Controls
{
    public class SvgPictureFrame : ImageViewBase
    {
        public SvgPictureFrame(IVisualBootstrapper visualBootstrapper)
            : base(visualBootstrapper)
        {
        }

        public IBrush? Fill
        {
            get => FillProperty.GetValue(this);
            set => FillProperty.SetValue(this, value);
        }

        public ISvgImage? Source
        {
            get => SourceProperty.GetValue(this);
            set => SourceProperty.SetValue(this, value);
        }

        public IColor? Stroke
        {
            get => StrokeProperty.GetValue(this);
            set => StrokeProperty.SetValue(this, value);
        }

        public override ValueSize Measure<TRenderSize>(TRenderSize availableSpace,
                                                       IMeasureContext measureContext)
        {
            var ezMeasure = MeasureHelper.MeasureVisual(this, availableSpace,
                out var desiredWidth, out var desiredHeight);

            if (ezMeasure == DimensionResult.HeightAndWidth)
                return new ValueSize(desiredWidth, desiredHeight);

            if (Source is not { } source)
                return ValueSize.Empty;

            switch (ezMeasure)
            {
                case DimensionResult.Invalid:
                case DimensionResult.None:
                    desiredWidth = source.Width;
                    desiredHeight = source.Height;
                    break;

                case DimensionResult.Width:
                    desiredHeight = source.Height;
                    break;

                case DimensionResult.Height:
                    desiredWidth = source.Width;
                    break;


                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new ValueSize(desiredWidth, desiredHeight);
        }


        protected override Boolean TryGetImage<TRenderSize>(TRenderSize size,
                                                            IVisualContext visualContext,
                                                            out IImage image)
        {
           var width = Convert.ToInt32(!Double.IsInfinity(size.Width) && !Double.IsNaN(size.Width)
              ? size.Width * visualContext.ZoomLevel
              : _image?.Width ?? Width ?? 0);

           var height = Convert.ToInt32(!Double.IsInfinity(size.Height) && !Double.IsNaN(size.Height)
              ? size.Height * visualContext.ZoomLevel
              : _image?.Height ?? Height ?? 0);


            if (_image is { } img &&
                img.Width.AreEqualEnough(width) &&
                img.Height.AreEqualEnough(height))
            {
                image = img;
                return true;
            }

            if (Source is { } source &&
                (Stroke != null ||
                 Fill != null))
            {
                image = source.ToStaticImage(width, height, Stroke, Fill)!;

                if (_image is { } ripImg)
                    ripImg.Dispose();


                _image = image;
                return _image != null;
            }

            image = default!;
            return false;
        }

        public static readonly DependencyProperty<SvgPictureFrame, ISvgImage?> SourceProperty =
            DependencyProperty<SvgPictureFrame, ISvgImage?>.Register(
                nameof(Source), default);

        public static readonly DependencyProperty<SvgPictureFrame, IColor?> StrokeProperty =
            DependencyProperty<SvgPictureFrame, IColor?>.Register(
                nameof(Stroke),
                default);

        public static readonly DependencyProperty<SvgPictureFrame, IBrush?> FillProperty =
            DependencyProperty<SvgPictureFrame, IBrush?>.Register(
                nameof(Fill),
                default);

        private IImage? _image;
    }
}
