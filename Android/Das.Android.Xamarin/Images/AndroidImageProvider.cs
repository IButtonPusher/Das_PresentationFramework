using System;
using System.IO;
using Android.Graphics;
using Das.Views.Core;
using Das.Views.Core.Drawing;

namespace Das.Xamarin.Android.Images
{
    public class AndroidImageProvider : IImageProvider
    {
        public IImage? GetImage(Stream stream)
        {
            var bmp = BitmapFactory.DecodeStream(stream);
            return bmp == null ? default(IImage?) : new AndroidBitmap(bmp);
        }
    }
}