using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Util;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Rendering;
using static Android.Graphics.BitmapFactory;
using Stream = System.IO.Stream;

namespace Das.Xamarin.Android
{
    public class AndroidRenderContext : BaseRenderContext
    {
        public AndroidRenderContext(IViewPerspective perspective,
                                    IFontProvider<AndroidFontPaint> fontProvider,
                                    IViewState viewState,
                                    IVisualSurrogateProvider surrogateProvider,
                                    Dictionary<IVisualElement, ValueCube> renderPositions,
                                    DisplayMetrics displayMetrics,
                                    Dictionary<IVisualElement, ValueSize> lastMeasurements)
            : base(perspective, surrogateProvider, renderPositions, lastMeasurements)
        {
            _fontProvider = fontProvider;
            _displayMetrics = displayMetrics;
            _paint = new Paint();
            ViewState = viewState;
        }

        public Canvas? Canvas { get; set; }

        public override void DrawEllipse<TPoint, TPen>(TPoint center,
                                                       Double radius,
                                                       TPen pen)
        {
            throw new NotImplementedException();
        }

        public override void DrawFrame(IFrame frame)
        {
            throw new NotImplementedException();
        }

        public override  void DrawImage<TRectangle>(IImage img, 
                                                    TRectangle destination)
        {
            var bmp = img.Unwrap<Bitmap>();
            var dest = GetAbsoluteAndroidRect(destination);


            if (dest.Width() == bmp.Width && dest.Height() == bmp.Height)
                GetCanvas().DrawBitmap(bmp, dest.Left, dest.Top, _paint);
            else
            {
                var src = new Rect(0, 0, bmp.Width, bmp.Height);
                GetCanvas().DrawBitmap(bmp, src, dest, _paint);
            }

            //DrawImage(img, new ValueRectangle(0, 0, img.Width, img.Height), destination);
        }

        

        public override void DrawImage<TRectangle1, TRectangle2>(IImage img,
                                                                 TRectangle1 srcRect,
                                                                 TRectangle2 destination)
        {
            var bmp = img.Unwrap<Bitmap>();
            var dest = GetAbsoluteAndroidRect(destination);
            var src = new Rect(Convert.ToInt32(srcRect.X),
                Convert.ToInt32(srcRect.Y),
                Convert.ToInt32(srcRect.Width),
                Convert.ToInt32(srcRect.Height));


            GetCanvas().DrawBitmap(bmp, src, dest, _paint);
        }

        public override void DrawLine<TPen, TPoint1, TPoint2>(TPen pen,
                                                              TPoint1 pt1,
                                                              TPoint2 pt2)
        {
            _paint.SetStyle(Paint.Style.Stroke);
            SetColor(pen);
            _paint.StrokeWidth = pen.Thickness;
            var l1 = GetAbsoluteAndroidPoint(pt1);
            var l2 = GetAbsoluteAndroidPoint(pt2);
            GetCanvas().DrawLine(l1.X, l1.Y, l2.X, l2.Y, _paint);
        }

        public override void DrawLines(IPen pen, 
                                       IPoint2D[] points)
        {
            if (points.Length < 2)
                return;

            _paint.SetStyle(Paint.Style.Stroke);
            SetColor(pen);

            var canvas = GetCanvas();

            var p1 = GetAbsoluteAndroidPoint(points[0]);

            for (var c = 1; c < points.Length; c++)
            {
                var p2 = GetAbsoluteAndroidPoint(points[c]);
                canvas.DrawLine(p1.X, p1.Y, p2.X, p2.Y, _paint);
                p1 = p2;
            }
        }

        public override void DrawRect<TRectangle, TPen>(TRectangle rect,
                                                        TPen pen)
        {
            _paint.SetStyle(Paint.Style.Stroke);
            SetColor(pen);
            GetCanvas().DrawRect(GetAbsoluteAndroidRect(rect), _paint);
        }

        public override void DrawRoundedRect<TRectangle, TPen>(TRectangle rect,
                                                               TPen pen,
                                                               Double cornerRadius)
        {
            _paint.SetStyle(Paint.Style.Stroke);
            SetColor(pen);
            GetCanvas().DrawRoundRect(GetAbsoluteAndroidRectF(rect),
                Convert.ToSingle(cornerRadius),
                Convert.ToSingle(cornerRadius),
                _paint);
        }

        public override void DrawString<TFont, TBrush, TPoint>(String s,
                                                               TFont font,
                                                               TBrush brush,
                                                               TPoint location)
        {
            var renderer = _fontProvider.GetRenderer(font);
            var dest = GetAbsolutePoint(location);
            renderer.Canvas = GetCanvas();
            renderer.DrawString(s, brush, dest);
        }

        //private Boolean _didClip;
        //private Int32 _clipCount;


        //private static void DoClip(Canvas canvas,
        //                           Int32 left,
        //                           Int32 top,
        //                           Int32 width,
        //                           Int32 height)
        //{
        //    System.Diagnostics.Debug.WriteLine("Clipping: " + left + ", " + top + " w: " + width +
        //                                       "h: " + height);

        //    canvas.ClipRect(left, top, width, height);
        //}

        protected override void PushClip<TRectangle>(TRectangle rect)
        {
            //_clipCount++;
            var canvas = GetCanvas();

            //var useRect = GetAbsoluteIntRect(rect);

            //var clipLeft = Convert.ToInt32(rect.Left + CurrentLocation.X);
            //var clipRight = Convert.ToInt32(rect.Right + CurrentLocation.X);
            //var clipTop = Convert.ToInt32(rect.Top + CurrentLocation.Y);
            //var clipBottom = Convert.ToInt32(rect.Bottom + CurrentLocation.Y);


            //var useRect = GetAbsoluteAndroidRect(rect);
            //var rdrr = canvas.ClipBounds;
            //GetCanvas().ClipRect(useRect);

            //if (_didClip)
            //    return;

            //_didClip = true;

            //canvas.Save();

            //canvas.ClipRect(0,0, 700, 250);
            //rdrr = canvas.ClipBounds;

            //return;

            ////////////////////////////////

            //System.Diagnostics.Debug.WriteLine("PUSH CLIP: " + rect);

            //System.Diagnostics.Debug.WriteLine("PUSH CLIP: " + clipLeft + ", " + clipTop + " w: " + 
            //                                   (clipRight - clipLeft) +
            //                                   " h: " + (clipBottom - clipTop));

            canvas.Save();

            canvas.ClipRect(
                Convert.ToInt32(rect.X),
                Convert.ToInt32(rect.Y),
                Convert.ToInt32(rect.Right),
                Convert.ToInt32(rect.Bottom));
            return;
            

            //if (clipLeft > 0)
            //{
            //    //rect running down left side
            //    DoClip(canvas, 0, 0, clipLeft, _displayMetrics.HeightPixels);

            //    if (clipTop > 0)
            //    {
            //        //top rect across
            //        DoClip(canvas, clipLeft, 0, _displayMetrics.WidthPixels - clipLeft, 
            //            clipTop);
            //    }
            //    else if (clipRight < _displayMetrics.WidthPixels)
            //    {
            //        //top right rectangle
            //        DoClip(canvas, clipRight, 0, _displayMetrics.WidthPixels - clipRight, 
            //            clipBottom);
            //    }

            //    if (clipBottom < _displayMetrics.HeightPixels)
            //    {
            //        //top rect all the way across bottom
            //        DoClip(canvas, clipLeft, clipBottom, _displayMetrics.WidthPixels - clipLeft, 
            //            _displayMetrics.HeightPixels - clipBottom);
            //    }
            //}
            //else
            //{
            //    if (clipRight < _displayMetrics.WidthPixels)
            //    {
            //        //rect running down right side
            //        DoClip(canvas, clipRight, 0, _displayMetrics.WidthPixels - clipRight, 
            //            _displayMetrics.HeightPixels);
            //    }

            //    if (clipBottom < _displayMetrics.HeightPixels)
            //    {
            //        // rect running across bottom
            //        DoClip(canvas, 0, clipBottom, _displayMetrics.WidthPixels,
            //            _displayMetrics.HeightPixels - clipBottom);
            //    }
            //}
        }

        protected override void PopClip<TRectangle>(TRectangle rect)
        {
            //System.Diagnostics.Debug.WriteLine("POP CLIP");
            //_clipCount--;

            var canvas = GetCanvas();
            //var useRect = GetAbsoluteAndroidRect(rect);

            canvas.Restore();
           // GetCanvas().ClipRect(0, 0, 0, 0);
            
        }

        protected override IRectangle GetCurrentClip()
        {
            var clip = GetCanvas().ClipBounds;
            if (clip == null)
                return Rectangle.Empty;
            if (clip.Width() == 0 && clip.Height() == 0)
                return Rectangle.Empty;

            return new Rectangle(clip.Left, clip.Top, clip.Width(), clip.Height());

        }

        public override void DrawString<TFont, TBrush, TRectangle>(String s,
                                                                   TFont font,
                                                                   TRectangle location,
                                                                   TBrush brush)
        {
            var renderer = _fontProvider.GetRenderer(font);
            var dest = GetAbsoluteRect(location);
            renderer.Canvas = GetCanvas();
            renderer.DrawString(s, brush, dest);
        }

        public override void FillPie<TPoint, TBrush>(TPoint center,
                                                     Double radius,
                                                     Double startAngle,
                                                     Double endAngle,
                                                     TBrush brush)
        {
            throw new NotImplementedException();
        }

        public override void FillRectangle<TRectangle, TBrush>(TRectangle rect,
                                                               TBrush brush)
        {
            _paint.SetStyle(Paint.Style.Fill);
            SetColor(brush);
            GetCanvas().DrawRect(GetAbsoluteAndroidRect(rect), _paint);
        }

        public override void FillRoundedRectangle<TRectangle, TBrush>(TRectangle rect,
                                                                      TBrush brush,
                                                                      Double cornerRadius)
        {
            _paint.SetStyle(Paint.Style.Fill);
            SetColor(brush);
            GetCanvas().DrawRoundRect(GetAbsoluteAndroidRectF(rect),
                Convert.ToSingle(cornerRadius),
                Convert.ToSingle(cornerRadius),
                _paint);
        }

        private Point GetAbsoluteAndroidPoint(IPoint2D relativePoint2D)
        {
            var to = GetAbsolutePoint(relativePoint2D);
            return new Point(Convert.ToInt32(to.X),
                Convert.ToInt32(to.Y));
        }

        private RectF GetAbsoluteAndroidRectF<TRectangle>(TRectangle rect)
            where TRectangle : IRectangle
        {
            return new RectF(Convert.ToSingle(rect.Left + CurrentLocation.X),
                Convert.ToSingle(rect.Top + CurrentLocation.Y),
                Convert.ToSingle(rect.Right + CurrentLocation.X),
                Convert.ToSingle(rect.Bottom + CurrentLocation.Y));
        }

        private Rect GetAbsoluteAndroidRect<TRectangle>(TRectangle rect)
        where TRectangle : IRectangle
        {
            return new Rect(Convert.ToInt32(rect.Left + CurrentLocation.X),
                Convert.ToInt32(rect.Top + CurrentLocation.Y),
                Convert.ToInt32(rect.Right + CurrentLocation.X),
                Convert.ToInt32(rect.Bottom + CurrentLocation.Y));
        }

        private Canvas GetCanvas()
        {
            return Canvas ?? throw new NullReferenceException("Canvas must be set");
        }

        public override IImage GetNullImage()
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

        public override IImage? GetImage(Stream stream, 
                                         Double maximumWidthPct)
        {
            var imgMaxWidth = _displayMetrics.WidthPixels * maximumWidthPct;

            if (!stream.CanSeek)
            {
                var img = BitmapFactory.DecodeStream(stream);
                return GetScaledImage(img, imgMaxWidth);
            }


            var options = new Options
            {
                InJustDecodeBounds = true
            };

            DecodeStream(stream, null, options);

            var scale = 1;
            while (options.OutWidth * (1 / Math.Pow(scale, 2)) > 
                   imgMaxWidth) {
                scale++;
            }

            if (scale == 1)
                return GetImage(stream);

            options = new Options
            {
                InSampleSize = scale - 1
            };

            var bmp = BitmapFactory.DecodeStream(stream, null,
                options);

            return GetScaledImage(bmp, imgMaxWidth);
        }

        public override IImage? GetImage(Byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
                return GetImage(ms);
        }

        public override IImage? GetImage(Stream stream)
        {
            var bmp = BitmapFactory.DecodeStream(stream);
            if (bmp == null)
                return default;

            return new AndroidBitmap(bmp);
        }

        private void SetColor(IPen pen)
        {
            _paint.SetARGB(pen.Color.A, pen.Color.R, pen.Color.G, pen.Color.B);
        }

        private void SetColor(IBrush brush)
        {
            switch (brush)
            {
                case SolidColorBrush scb:
                    _paint.SetARGB(scb.Color.A, scb.Color.R, scb.Color.G, scb.Color.B);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private readonly IFontProvider<AndroidFontPaint> _fontProvider;
        private readonly DisplayMetrics _displayMetrics;

        private readonly Paint _paint;
        
    }
}