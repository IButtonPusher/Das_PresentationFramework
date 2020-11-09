﻿using System;
using System.Collections.Generic;
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
        public AndroidRenderContext(IViewPerspective perspective,
                                    IFontProvider<AndroidFontPaint> fontProvider,
                                    IViewState viewState,
                                    IVisualSurrogateProvider surrogateProvider,
                                    Dictionary<IVisualElement, ICube> renderPositions)
            : base(perspective, surrogateProvider, renderPositions)
        {
            _fontProvider = fontProvider;
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
            DrawImage(img, new ValueRectangle(0, 0, img.Width, img.Height), destination);
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

        private RectF GetAbsoluteAndroidRectF(IRectangle rect)
        {
            return new RectF(Convert.ToSingle(rect.Left + CurrentLocation.X),
                Convert.ToSingle(rect.Top + CurrentLocation.Y),
                Convert.ToSingle(rect.Right + CurrentLocation.X),
                Convert.ToSingle(rect.Bottom + CurrentLocation.Y));
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

        public override IImage GetNullImage()
        {
            var bmp = BitmapFactory.DecodeByteArray(Array.Empty<Byte>(), 0, 0);
            return new AndroidBitmap(bmp!);
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