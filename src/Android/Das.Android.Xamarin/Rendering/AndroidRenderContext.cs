using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Graphics;
using Das.Extensions;
using Das.Views;
using Das.Views.Colors;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Images;
using Das.Views.Rendering;
using Das.Xamarin.Android.Rendering;

namespace Das.Xamarin.Android
{
    public class AndroidRenderContext : BaseRenderContext
    {
        public AndroidRenderContext(IViewPerspective perspective,
                                    AndroidFontProvider fontProvider,
                                    IViewState viewState,
                                    IVisualSurrogateProvider surrogateProvider,
                                    Dictionary<IVisualElement, ValueCube> renderPositions,
                                    Dictionary<IVisualElement, ValueSize> lastMeasurements,
                                    IThemeProvider themeProvider,
                                    IVisualLineage visualLineage,
                                    ILayoutQueue layoutQueue)
            : base(perspective, surrogateProvider, renderPositions, lastMeasurements,
                themeProvider, visualLineage, layoutQueue)
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

        public override void DrawImage<TRectangle>(IImage img,
                                                   TRectangle destination)
        {
            var bmp = img.Unwrap<Bitmap>();
            var dest = GetAbsoluteAndroidRect(destination);


            if (dest.Width() == bmp.Width && dest.Height() == bmp.Height)
                GetCanvas().DrawBitmap(bmp, dest.Left, dest.Top, _paint);
            else
            {
                var src = BakeRect(0, 0, bmp.Width, bmp.Height);

                GetCanvas().DrawBitmap(bmp, src, dest, _paint);
            }
        }


        public override void DrawImage<TRectangle1, TRectangle2>(IImage img,
                                                                 TRectangle1 srcRect,
                                                                 TRectangle2 destination)
        {
            var bmp = img.Unwrap<Bitmap>();
            var dest = GetAbsoluteAndroidRect(destination);

            var src = BakeRect(srcRect.Left, srcRect.Top, srcRect.Right, srcRect.Bottom);

            GetCanvas().DrawBitmap(bmp, src, dest, _paint);
        }

        public override void DrawLine<TPen, TPoint1, TPoint2>(TPen pen,
                                                              TPoint1 pt1,
                                                              TPoint2 pt2)
        {
            _paint.SetStrokeColor(pen.Color);
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

            _paint.SetStrokeColor(pen.Color);

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
            _paint.SetStrokeColor(pen.Color);
            GetCanvas().DrawRect(GetAbsoluteAndroidRect(rect), _paint);
        }

        public override void DrawRoundedRect<TRectangle, TPen, TThickness>(TRectangle rect,
                                                                           TPen pen,
                                                                           TThickness cornerRadii)
        {
            _paint.SetStrokeColor(pen.Color);
            RoundedRectImpl(rect, cornerRadii);
        }

        public override void DrawString<TFont, TBrush, TPoint>(String s,
                                                               TFont font,
                                                               TBrush brush,
                                                               TPoint location)
        {
            var renderer = _fontProvider.GetRenderer(font, VisualLineage);
            var dest = _boxModel.GetAbsolutePoint(location, ZoomLevel);
            renderer.Canvas = GetCanvas();
            renderer.DrawString(s, brush, dest);
        }

        public override void DrawString<TFont, TBrush, TRectangle>(String s,
                                                                   TFont font,
                                                                   TRectangle location,
                                                                   TBrush brush)
        {
            var renderer = _fontProvider.GetRenderer(font, VisualLineage);
            var dest = _boxModel.GetAbsoluteRect(location, ZoomLevel);
            renderer.Canvas = GetCanvas();
            renderer.DrawStringInRect(s, brush, dest);
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
            _paint.SetBackgroundColor(brush);

            var letsDraw = GetAbsoluteAndroidRect(rect);

            GetCanvas().DrawRect(letsDraw, _paint);
        }

        public override void FillRoundedRectangle<TRectangle, TBrush, TThickness>(TRectangle rect,
            TBrush brush,
            TThickness cornerRadii)
        {
            _paint.SetBackgroundColor(brush);

            RoundedRectImpl(rect, cornerRadii);
        }

        protected override ValueRectangle GetCurrentClip()
        {
            var clip = GetCanvas().ClipBounds;
            if (clip == null)
                return ValueRectangle.Empty;
            if (clip.Width() == 0 && clip.Height() == 0)
                return ValueRectangle.Empty;

            if (ZoomLevel.AreDifferent(1.0))
                return new ValueRectangle(clip.Left / ZoomLevel,
                    clip.Top / ZoomLevel,
                    clip.Width() / ZoomLevel,
                    clip.Height() / ZoomLevel);
            return new ValueRectangle(clip.Left,
                clip.Top,
                clip.Width(),
                clip.Height());
        }
        
        protected override void PopClip<TRectangle>(TRectangle rect)
        {
            var canvas = GetCanvas();
            canvas.Restore();
        }

        protected override void PushClip<TRectangle>(TRectangle rect)
        {
            var canvas = GetCanvas();

            canvas.Save();

            canvas.ClipRect(Convert.ToInt32(rect.X),
                Convert.ToInt32(rect.Y),
                Convert.ToInt32(rect.Right),
                Convert.ToInt32(rect.Bottom));
        }

        private static Rect BakeRect(Double left,
                                     Double top,
                                     Double right,
                                     Double bottom)
        {
            if (_rect == null)
            {
                _rect = new Rect(Convert.ToInt32(left),
                    Convert.ToInt32(top),
                    Convert.ToInt32(right),
                    Convert.ToInt32(bottom));
                return _rect;
            }

            _rect.Left = Convert.ToInt32(left);
            _rect.Top = Convert.ToInt32(top);
            _rect.Right = Convert.ToInt32(right);
            _rect.Bottom = Convert.ToInt32(bottom);

            return _rect;
        }

        private static Rect BakeRect(Int32 left,
                                     Int32 top,
                                     Int32 right,
                                     Int32 bottom)
        {
            if (_rect == null)
            {
                _rect = new Rect(left,
                    top,
                    right,
                    bottom);
                return _rect;
            }

            _rect.Left = left;
            _rect.Top = top;
            _rect.Right = right;
            _rect.Bottom = bottom;

            return _rect;
        }

        private Point GetAbsoluteAndroidPoint(IPoint2D relativePoint2D)
        {
            if (ZoomLevel.AreDifferent(1.0))
                return new Point(
                    Convert.ToInt32((CurrentLocation.X + relativePoint2D.X) * ZoomLevel),
                    Convert.ToInt32((CurrentLocation.Y + relativePoint2D.Y) * ZoomLevel));

            return new Point(
                Convert.ToInt32(CurrentLocation.X + relativePoint2D.X),
                Convert.ToInt32(CurrentLocation.Y + relativePoint2D.Y));
        }


        private Rect GetAbsoluteAndroidRect<TRectangle>(TRectangle rect)
            where TRectangle : IRectangle
        {
            if (ZoomLevel.AreDifferent(1.0))
                return new Rect(Convert.ToInt32((rect.Left + CurrentLocation.X) * ZoomLevel),
                    Convert.ToInt32((rect.Top + CurrentLocation.Y) * ZoomLevel),
                    Convert.ToInt32((rect.Right + CurrentLocation.X) * ZoomLevel),
                    Convert.ToInt32((rect.Bottom + CurrentLocation.Y) * ZoomLevel));

            return new Rect(Convert.ToInt32(rect.Left + CurrentLocation.X),
                Convert.ToInt32(rect.Top + CurrentLocation.Y),
                Convert.ToInt32(rect.Right + CurrentLocation.X),
                Convert.ToInt32(rect.Bottom + CurrentLocation.Y));
        }

        private Canvas GetCanvas()
        {
            return Canvas ?? throw new NullReferenceException("Canvas must be set");
        }

        /// <summary>
        ///     _paint.SetStyle and SetColor have to be called before this
        /// </summary>
        private void RoundedRectImpl<TRectangle, TThickness>(TRectangle rect,
                                                             TThickness cornerRadii)
            where TRectangle : IRectangle
            where TThickness : IThickness
        {
            var useRect = _boxModel.GetAbsoluteRect(rect, ZoomLevel);

            if (cornerRadii.AreAllSidesEqual())
            {
                var androidRect = new RectF(Convert.ToSingle(useRect.Left),
                    Convert.ToSingle(useRect.Top),
                    Convert.ToSingle(useRect.Right),
                    Convert.ToSingle(useRect.Bottom));

                var sRadius = Convert.ToSingle(cornerRadii.Left);

                GetCanvas().DrawRoundRect(androidRect, sRadius, sRadius, _paint);

                return;
            }

            var path = new AndroidGraphicsPath();
            path.SetRoundedRectangle(useRect, cornerRadii);
            GetCanvas().DrawPath(path.Path, _paint);
        }


        [ThreadStatic]
        private static Rect? _rect;
        private readonly AndroidFontProvider _fontProvider;
        private readonly Paint _paint;
    }
}
