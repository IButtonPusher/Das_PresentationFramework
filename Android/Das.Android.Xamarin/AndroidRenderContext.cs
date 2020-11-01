using System;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Rendering;

namespace Das.Xamarin.Android
{
    public class AndroidRenderContext : BaseRenderContext
    {
        public AndroidRenderContext(IMeasureContext measureContext,
                                    IViewPerspective perspective,
                                    IFontProvider<AndroidFontPaint> fontProvider,
                                    IViewState viewState,
                                    IVisualSurrogateProvider surrogateProvider)
            : base(measureContext, perspective, surrogateProvider)
        {
            _fontProvider = fontProvider;
            _paint = new Paint();
            ViewState = viewState;
        }

        public Canvas? Canvas { get; set; }

        public override void DrawEllipse(IPoint2D center,
                                         Double radius,
                                         IPen pen)
        {
            throw new NotImplementedException();
        }

        public override void DrawFrame(IFrame frame)
        {
            throw new NotImplementedException();
        }

        public override void DrawImage(IImage img,
                                       IRectangle rect)
        {
            DrawImage(img, new ValueRectangle(0, 0, img.Width, img.Height), rect);
        }

        public override void DrawImage(IImage img,
                                       IRectangle srcRect,
                                       IRectangle destination)
        {
            var bmp = img.Unwrap<Bitmap>();
            var dest = GetAbsoluteAndroidRect(destination);
            var src = new Rect(Convert.ToInt32(srcRect.X),
                Convert.ToInt32(srcRect.Y),
                Convert.ToInt32(srcRect.Width),
                Convert.ToInt32(srcRect.Height));


            GetCanvas().DrawBitmap(bmp, src, dest, _paint);
        }

        public override void DrawLine(IPen pen,
                                      IPoint2D pt1,
                                      IPoint2D pt2)
        {
            _paint.SetStyle(Paint.Style.Stroke);
            SetColor(pen);
            _paint.StrokeWidth = pen.Thickness;
            var l1 = GetAbsoluteAndroidPoint(pt1);
            var l2 = GetAbsoluteAndroidPoint(pt2);
            GetCanvas().DrawLine(l1.X, l1.Y, l2.X, l2.Y, _paint);
        }

        public override void DrawLines(IPen pen, IPoint2D[] points)
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

        public override void DrawRect(IRectangle rect, IPen pen)
        {
            _paint.SetStyle(Paint.Style.Stroke);
            SetColor(pen);
            GetCanvas().DrawRect(GetAbsoluteAndroidRect(rect), _paint);
        }

        public override void DrawRoundedRect(IRectangle rect,
                                             IPen pen,
                                             Double cornerRadius)
        {
            throw new NotImplementedException();
        }

        public override void DrawString(String s,
                                        IFont font,
                                        IBrush brush,
                                        IPoint2D point2D)
        {
            var renderer = _fontProvider.GetRenderer(font);
            var dest = GetAbsolutePoint(point2D);
            renderer.Canvas = GetCanvas();
            renderer.DrawString(s, brush, dest);
        }

        public override void DrawString(String s,
                                        IFont font,
                                        IBrush brush,
                                        IRectangle rect)
        {
            var renderer = _fontProvider.GetRenderer(font);
            var dest = GetAbsoluteRect(rect);
            renderer.Canvas = GetCanvas();
            renderer.DrawString(s, brush, dest);
        }

        public override void FillPie(IPoint2D center,
                                     Double radius,
                                     Double startAngle,
                                     Double endAngle,
                                     IBrush brush)
        {
            throw new NotImplementedException();
        }

        public override void FillRectangle(IRectangle rect, IBrush brush)
        {
            _paint.SetStyle(Paint.Style.Fill);
            SetColor(brush);
            GetCanvas().DrawRect(GetAbsoluteAndroidRect(rect), _paint);
        }

        public override void FillRoundedRectangle(IRectangle rect, IBrush brush, Double cornerRadius)
        {
            throw new NotImplementedException();
        }

        private Point GetAbsoluteAndroidPoint(IPoint2D relativePoint2D)
        {
            var to = GetAbsolutePoint(relativePoint2D);
            return new Point(Convert.ToInt32(to.X),
                Convert.ToInt32(to.Y));
        }

        private Rect GetAbsoluteAndroidRect(IRectangle rect)
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

        private readonly Paint _paint;
    }
}