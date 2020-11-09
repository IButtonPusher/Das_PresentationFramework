﻿using System;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views.Controls
{
    // ReSharper disable once UnusedMember.Global
    public class PictureFrame : BindableElement<IImage>
    {
        public PictureFrame()
        {
        }

        public PictureFrame(IDataBinding<IImage> value) : base(value)
        {
        }

        public override void Arrange(IRenderSize availableSpace,
                                     IRenderContext renderContext)
        {
            var img = _currentImage;
            if (img == null)
                return;

            if (!availableSpace.Width.AreEqualEnough(img.Width) ||
                !availableSpace.Height.AreEqualEnough(availableSpace.Height))
                if (availableSpace.Height < img.Height)
                {
                    var scale = availableSpace.Height / img.Height;
                    availableSpace = new ValueRenderSize(img.Width * scale, availableSpace.Height);
                }

            var rect = new Rectangle(Point2D.Empty, availableSpace);
            var zoom = renderContext.ViewState?.ZoomLevel ?? 1;

            renderContext.DrawImage(img, (rect * zoom)!);
        }

        public override void Dispose()
        {
            _currentImage?.Dispose();
        }

        public override ISize Measure(IRenderSize availableSpace,
                                      IMeasureContext measureContext)
        {
            var zoom = measureContext.ViewState?.ZoomLevel ?? 1;

            //if (!(DataContext is {} dc))
            //    return Size.Empty;

            _currentImage = GetBoundValue(DataContext!);
            var size = measureContext.MeasureImage(_currentImage);
            var forced = measureContext.GetStyleSetter<Size>(StyleSetter.Size, this)
                         * zoom;
            if (forced != null && !Size.Empty.Equals(forced))
                return size;

            var width = measureContext.GetStyleSetter<Double>(StyleSetter.Width, this) * zoom;
            var height = measureContext.GetStyleSetter<Double>(StyleSetter.Height, this) * zoom;

            if (!Double.IsNaN(width) && !Double.IsNaN(height))
                return new Size(width, height);

            if (!Double.IsNaN(height))
            {
                var scale = height / size.Height;
                return new Size(size.Width * scale, height);
            }

            if (!Double.IsNaN(width))
            {
                var scale = width / size.Height;
                return new Size(height, size.Width * scale);
            }

            return size;
        }

        private IImage? _currentImage;
    }
}