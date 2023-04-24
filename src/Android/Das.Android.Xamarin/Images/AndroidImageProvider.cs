using System;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Util;
using Das.Extensions;
using Das.Views.Core;
using Das.Views.Core.Drawing;
using Das.Views.Images;
using Das.Xamarin.Android.Rendering;

namespace Das.Xamarin.Android.Images;

public class AndroidImageProvider : IImageProvider
{
   public AndroidImageProvider(DisplayMetrics displayMetrics)
      //IVisualContext visualContext)
   {
      _displayMetrics = displayMetrics;
      // the visual context is too broad in scope for whatever this was intended for
      //_visualContext = visualContext;
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
         return GetScaledImage(img, null, imgMaxWidth, false, true);
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
      {
         stream.Position = 0;
         return GetImage(stream, isPreserveStream);
      }

      options = new BitmapFactory.Options
      {
         InSampleSize = scale - 1
      };

      var bmp = BitmapFactory.DecodeStream(stream, null,
         options);

      return GetScaledImage(bmp, stream, imgMaxWidth, isPreserveStream, true);
   }

   public IImage GetScaledImage(IImage input,
                                Double width,
                                Double height)
   {
      var bmp = input.Unwrap<Bitmap>();
      return GetScaledImage(bmp, null, width, false, false);
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

         var resultBitmap = Bitmap.CreateBitmap(1, 1, Bitmap.Config.Argb8888!);
         _nullBitmap = new AndroidBitmap(resultBitmap!, null);
         return _nullBitmap;
      }
   }

   public Double DeviceEffectiveDpi => 1.0;

   public IGraphicsPath GetNewGraphicsPath()
   {
      return new AndroidGraphicsPath();//_visualContext);
   }

   //public IImage GetImage(IGraphicsPath path,
   //                       IColor foreground)
   //{
   //    //var size = path.Size.ToRoundedSize();
   //    var size = new ValueRoundedSize(24, 24);
   //    var androidPath = path.Unwrap<Path>();

   //    var bmp = Bitmap.CreateBitmap(size.Width, size.Height, Bitmap.Config.Argb8888)
   //        ?? throw new InvalidOperationException();

   //    var canvas = new Canvas(bmp);
   //    using (var paint = new Paint())
   //    {
   //        paint.SetStyle(Paint.Style.Stroke);
   //        paint.SetARGB(foreground.A, foreground.R, foreground.G, foreground.B);
   //        //canvas.SetColor(pen);
   //        canvas.DrawPath(androidPath, paint);
   //    }

   //    return new AndroidBitmap(bmp, null);
   //}


   private IImage GetScaledImage(Bitmap? img,
                                 Stream? stream,
                                 Double imgDesiredWidth,
                                 Boolean isPreserveStream,
                                 Boolean isDisposeOriginal)
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
      if (isDisposeOriginal)
         img.Dispose();
      return new AndroidBitmap(scaledBitmap!, null);
   }

   private readonly DisplayMetrics _displayMetrics;
   //private readonly IVisualContext _visualContext;
   private readonly Object _nullBitmapLock;

   private AndroidBitmap? _nullBitmap;
}