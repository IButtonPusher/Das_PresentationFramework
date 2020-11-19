using System;
using System.Drawing;
using System.IO;
using Das.Views;
using Das.Views.Core;
using Das.Views.Core.Drawing;

namespace Gdi.Shared
{
    public class GdiImageProvider : IImageProvider
    {
        public IImage? GetImage(Stream stream)
        {
            var img = Image.FromStream(stream);

            //var bmp = new Bitmap(stream,);
            return new GdiBitmap(img, stream);
        }

        public IImage? GetImage(Stream stream, 
                                Double maximumWidthPct)
        {
            if (!(_window is {} window))
                return GetImage(stream);

            var bmp = new Bitmap(stream);

            var maximumWidth = window.AvailableSize.Width * maximumWidthPct;

            var ratio = bmp.Width / maximumWidth;

            if (ratio <= maximumWidthPct)
                return new GdiBitmap(bmp, stream);

            var scaleRatio = maximumWidth / bmp.Width; //maximumWidthPct * maximumWidth;

            using (stream)
            using (bmp)
            {
                var scaledBmp = new Bitmap(bmp, Convert.ToInt32(bmp.Width * scaleRatio),
                    Convert.ToInt32(bmp.Height * scaleRatio));
                return new GdiBitmap(scaledBmp, null);
            }
        }

        public IImage? GetImage(Byte[] bytes)
        {
            var ms = new MemoryStream(bytes);
            return GetImage(ms);
        }

        public IImage? GetImage(FileInfo file)
        {
            var img = Image.FromFile(file.FullName);
            return new GdiBitmap(img, null);
        }

        public IImage GetNullImage()
        {
            return _emptyImage ??= new GdiBitmap(new Bitmap(1, 1), null);
        }

        public void SetVisualHost(IVisualHost visualHost)
        {
            _window = visualHost;
        }

        private IImage? _emptyImage;
        private IVisualHost? _window;
    }
}
