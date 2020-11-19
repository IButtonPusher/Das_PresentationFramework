using System;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Util;
using Das.Views.Core;
using Das.Views.Core.Drawing;

namespace Das.Xamarin.Android.Images
{
    public class AndroidImageProvider : IImageProvider
    {
        public AndroidImageProvider(DisplayMetrics displayMetrics)
        {
            _displayMetrics = displayMetrics;
        }

        public IImage? GetImage(Stream stream)
        {
            var bmp = BitmapFactory.DecodeStream(stream);
            return bmp == null ? default(IImage?) : new AndroidBitmap(bmp);
        }

        public IImage? GetImage(Stream stream,
                                Double maximumWidthPct)
        {
            var imgMaxWidth = _displayMetrics.WidthPixels * maximumWidthPct;

            if (!stream.CanSeek)
            {
                var img = BitmapFactory.DecodeStream(stream);
                return GetScaledImage(img, imgMaxWidth);
            }


            var options = new BitmapFactory.Options
            {
                InJustDecodeBounds = true
            };

            BitmapFactory.DecodeStream(stream, null, options);

            var scale = 1;
            while (options.OutWidth * (1 / Math.Pow(scale, 2)) >
                   imgMaxWidth)
                scale++;

            if (scale == 1)
                return GetImage(stream);

            options = new BitmapFactory.Options
            {
                InSampleSize = scale - 1
            };

            var bmp = BitmapFactory.DecodeStream(stream, null,
                options);

            return GetScaledImage(bmp, imgMaxWidth);
        }

        public IImage? GetImage(Byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                return GetImage(ms);
            }
        }

        IImage? IImageProvider.GetImage(FileInfo file)
        {
            throw new NotSupportedException();
        }

        public IImage GetNullImage()
        {
            var bmp = BitmapFactory.DecodeByteArray(Array.Empty<Byte>(), 0, 0);
            return new AndroidBitmap(bmp!);
        }

        private static IImage? GetScaledImage(Bitmap? img,
                                              Double imgMaxWidth)
        {
            if (img == null || img.Width < imgMaxWidth)
                return new AndroidBitmap(img);

            var scaleRatio = imgMaxWidth / img.Width;

            var scaledBitmap = Bitmap.CreateScaledBitmap(img,
                Convert.ToInt32(img.Width * scaleRatio),
                Convert.ToInt32(img.Height * scaleRatio), true);
            img.Dispose();
            return new AndroidBitmap(scaledBitmap);
        }

        private readonly DisplayMetrics _displayMetrics;
    }
}