﻿using System;
using System.Threading.Tasks;
using Das.Extensions;
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

        protected override Boolean TryGetImage<TRenderSize>(TRenderSize size,
                                                            out IImage image)
        {
            var width = !Double.IsInfinity(size.Width) && !Double.IsNaN(size.Width)
                ? size.Width
                : _image?.Width ?? Width ?? 0;

            var height = !Double.IsInfinity(size.Height) && !Double.IsNaN(size.Height)
                ? size.Height
                : _image?.Height ?? Height ?? 0;


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
                image = source.ToStaticImage(Convert.ToInt32(width),
                    Convert.ToInt32(height), Stroke, Fill)!;

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
