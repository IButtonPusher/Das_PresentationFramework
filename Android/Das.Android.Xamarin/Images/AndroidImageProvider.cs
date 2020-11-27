using System;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Util;
using Das.Extensions;
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

        public IImage? GetDeviceScaledImage(Stream stream,
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

        public IImage GetScaledImage(IImage input, 
                                     Double width, 
                                     Double height)
        {
            var bmp = input.Unwrap<Bitmap>();
            return GetScaledImage(bmp, width);
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

        public Double DeviceEffectiveDpi => 1.0;

        private static IImage? GetScaledImage(Bitmap? img,
                                              Double imgDesiredWidth)
        {
            if (img == null || imgDesiredWidth.AreEqualEnough(img.Width))
                return new AndroidBitmap(img);

            var scaleRatio = imgDesiredWidth / img.Width;

            var widthWas = img.Width;

            var width = Convert.ToInt32(img.Width * scaleRatio);
            var height = Convert.ToInt32(img.Height * scaleRatio);

            var scaledBitmap = Bitmap.CreateScaledBitmap(img,
                width, height, true);
            img.Dispose();
            return new AndroidBitmap(scaledBitmap);
        }

        private readonly DisplayMetrics _displayMetrics;
    }
}