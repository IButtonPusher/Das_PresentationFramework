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
            _nullBitmapLock = new Object();
        }

        public IImage? GetImage(Stream stream) => GetImage(stream, false);

        public IImage? GetImage(Stream stream,
                                Boolean isPreserveStream)
        {
            var bmp = BitmapFactory.DecodeStream(stream);
            if (bmp == null)
                return null;

            if (isPreserveStream)
                return new AndroidBitmap(bmp, stream);

            return new AndroidBitmap(bmp, null);
        }

        public IImage? GetDeviceScaledImage(Stream stream,
                                            Double maximumWidthPct,
                                            Boolean isPreserveStream)
        {
            var imgMaxWidth = _displayMetrics.WidthPixels * maximumWidthPct;

            if (!stream.CanSeek)
            {
                var img = BitmapFactory.DecodeStream(stream);
                return GetScaledImage(img, null, imgMaxWidth, false);
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
                return GetImage(stream, isPreserveStream);

            options = new BitmapFactory.Options
            {
                InSampleSize = scale - 1
            };

            var bmp = BitmapFactory.DecodeStream(stream, null,
                options);

            return GetScaledImage(bmp, stream, imgMaxWidth, isPreserveStream);
        }

        public IImage GetScaledImage(IImage input,
                                     Double width,
                                     Double height)
        {
            var bmp = input.Unwrap<Bitmap>();
            return GetScaledImage(bmp, null, width, false);
        }

        public IImage? GetImage(Byte[] bytes) => GetImage(bytes, false);

        public IImage? GetImage(Byte[] bytes,
                                Boolean isPreserveStream)
        {
            using (var ms = new MemoryStream(bytes))
            {
                return GetImage(ms, isPreserveStream);
            }
        }

        IImage? IImageProvider.GetImage(FileInfo file)
        {
            throw new NotSupportedException();
        }

        public IImage GetNullImage()
        {
            lock (_nullBitmapLock)
            {
                if (_nullBitmap != null)
                    return _nullBitmap;

                var resultBitmap = Bitmap.CreateBitmap(1, 1, Bitmap.Config.Argb8888);
                _nullBitmap = new AndroidBitmap(resultBitmap!, null);
                return _nullBitmap;
            }
        }

        public Double DeviceEffectiveDpi => 1.0;

        private IImage GetScaledImage(Bitmap? img,
                                             Stream? stream,
                                              Double imgDesiredWidth,
                                              Boolean isPreserveStream)
        {
            if (img == null)
            {
                return GetNullImage();

            }

            if (imgDesiredWidth.AreEqualEnough(img.Width))
            {
                return isPreserveStream
                    ? new AndroidBitmap(img, stream)
                    : new AndroidBitmap(img, null);
            }

            var scaleRatio = imgDesiredWidth / img.Width;

            var width = Convert.ToInt32(img.Width * scaleRatio);
            var height = Convert.ToInt32(img.Height * scaleRatio);

            var scaledBitmap = Bitmap.CreateScaledBitmap(img,
                width, height, true);
            img.Dispose();
            return new AndroidBitmap(scaledBitmap!, null);
        }

        private readonly DisplayMetrics _displayMetrics;
        private readonly Object _nullBitmapLock;

        private AndroidBitmap? _nullBitmap;
    }
}