using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Das.Views;
using Das.Views.Core;
using Das.Views.Core.Drawing;
using Das.Views.Images;

namespace OpenGLTests
{
   class TestImageProvider: IImageProvider
    {
        public IImage? GetImage(Stream stream)
        {
            var img = Image.FromStream(stream);
            return new TestBitmap(img, stream);
        }

        public IImage? GetImage(Stream stream,
                                Boolean isPreserveStream)
        {
            return GetImage(stream);
        }

        public IImage? GetDeviceScaledImage(Stream stream,
                                            Double maximumWidthPct,
                                            Boolean isPreserveStream)
        {
            return GetDeviceScaledImage(stream, maximumWidthPct);
        }

        public IImage GetScaledImage(IImage input,
                                     Double width,
                                     Double height)
        {
            var bmp = input.Unwrap<Bitmap>();

            var bmp2 = new Bitmap(Convert.ToInt32(width),
                Convert.ToInt32(height));

            using (var g = bmp2.GetSmoothGraphics())
            {
                g.DrawImage(bmp,
                    new Rectangle(0, 0, bmp2.Width, bmp2.Height),
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    GraphicsUnit.Pixel);
            }

            return new TestBitmap(bmp2, null);
        }

        public IImage? GetImage(Byte[] bytes)
        {
            var ms = new MemoryStream(bytes);
            return GetImage(ms);
        }

        public IImage? GetImage(Byte[] bytes,
                                Boolean isPreserveStream)
        {
            return GetImage(bytes);
        }

        public IImage? GetImage(FileInfo file)
        {
            var img = Image.FromFile(file.FullName);
            return new TestBitmap(img, null);
        }

        public IImage GetNullImage()
        {
            return _emptyImage ??= new TestBitmap(new Bitmap(1, 1), null);
        }

        public Double DeviceEffectiveDpi => 1.0;

        public IGraphicsPath GetNewGraphicsPath()
        {
            return new TestGraphicsPath();
        }

        public IImage? GetDeviceScaledImage(Stream stream,
                                            Double maximumWidthPct)
        {
            if (!(_window is { } window))
                return GetImage(stream);

            var bmp = new Bitmap(stream);

            var maximumWidth = window.AvailableSize.Width * maximumWidthPct;

            var ratio = bmp.Width / maximumWidth;

            if (ratio <= maximumWidthPct)
                return new TestBitmap(bmp, stream is MemoryStream ? stream : null);

            var scaleRatio = maximumWidth / bmp.Width;

            using (stream)
            using (bmp)
            {
                var scaledBmp = new Bitmap(Convert.ToInt32(bmp.Width * scaleRatio),
                    Convert.ToInt32(bmp.Height * scaleRatio));

                using (var g = scaledBmp.GetSmoothGraphics())
                {
                    g.DrawImage(bmp,
                        new Rectangle(0, 0, scaledBmp.Width, scaledBmp.Height),
                        new Rectangle(0, 0, bmp.Width, bmp.Height),
                        GraphicsUnit.Pixel);
                }


                return new TestBitmap(scaledBmp, null);
            }
        }

        public void SetVisualHost(IVisualHost visualHost)
        {
            _window = visualHost;
        }

        private IImage? _emptyImage;
        private IVisualHost? _window;
    }
}
