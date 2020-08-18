using System;
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
    public class Image : BindableElement<IImage>
    {
        public Image()
        {
        }

        public Image(IDataBinding<IImage> value) : base(value)
        {
        }

        public override void Arrange(ISize availableSpace, IRenderContext renderContext)
        {
            var img = _currentImage;
            if (img == null)
                return;

            if (!availableSpace.Width.AreEqualEnough(_currentImage.Width) ||
                !availableSpace.Height.AreEqualEnough(availableSpace.Height))
                if (availableSpace.Height < _currentImage.Height)
                {
                    var scale = availableSpace.Height / _currentImage.Height;
                    availableSpace = new Size(img.Width * scale, availableSpace.Height);
                }

            var rect = new Rectangle(Point.Empty, availableSpace);
            renderContext.DrawImage(_currentImage, rect * renderContext.ViewState.ZoomLevel);
        }

        public override void Dispose()
        {
            _currentImage?.Dispose();
        }

        public override ISize Measure(ISize availableSpace, IMeasureContext measureContext)
        {
            var zoom = measureContext.ViewState.ZoomLevel;

            if (!(DataContext is {} dc))
                return Size.Empty;

            _currentImage = GetBoundValue(dc);
            var size = measureContext.MeasureImage(_currentImage);
            var forced = measureContext.GetStyleSetter<Size>(StyleSetters.Size, this)
                         * zoom;
            if (forced != null && !Size.Empty.Equals(forced))
                return size;

            var width = measureContext.GetStyleSetter<Double>(StyleSetters.Width, this) * zoom;
            var height = measureContext.GetStyleSetter<Double>(StyleSetters.Height, this) * zoom;

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

        private IImage _currentImage;
    }
}