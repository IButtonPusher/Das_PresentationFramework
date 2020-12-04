using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Graphics;

using Android.Text;
using Android.Util;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;

namespace Das.Xamarin.Android
{
    public class AndroidFontPaint : TextPaint, 
                                    IFontRenderer
    {
        private readonly DisplayMetrics _displayMetrics;
        private readonly Boolean _isCacheStaticLayouts;

        public AndroidFontPaint(IFont font,
                                DisplayMetrics displayMetrics,
                                Boolean isCacheStaticLayouts)
            : base(PaintFlags.AntiAlias)
        {
            _displayMetrics = displayMetrics;
            _isCacheStaticLayouts = isCacheStaticLayouts;
            _layoutCache = new Dictionary<IRectangle, StaticLayout>();
            Font = font;
            SetStyle(Style.Fill);

            var px = TypedValue.ApplyDimension(ComplexUnitType.Dip,
                Convert.ToSingle(font.Size),
                displayMetrics);

            TextSize = px;
            StrokeWidth = 10;
        }

        public IFont Font { get; }

        [Obsolete("dont forget to fix this!")]
        public void DrawString<TBrush, TPoint>(String text,
                                               TBrush brush,
                                               TPoint point2D)
            where TBrush : IBrush
            where TPoint : IPoint2D
        {
            //SetColor(brush);

            var size = MeasureString(text);
            var test = new ValueRectangle(point2D, size.Width, size.Height);
            DrawStringInRect(text, brush, test);

            //GetCanvas().DrawText(text, (Single) point2D.X, (Single) point2D.Y, this);
        }

        public void DrawStringInRect<TBrush, TRectangle>(String s,
                                                         TBrush brush,
                                                         TRectangle bounds)
            where TBrush : IBrush
            where TRectangle : IRectangle
        {
            var canvas = GetCanvas();

            SetColor(brush);

            //System.Diagnostics.Debug.WriteLine("Drawing string " + s + " in rect " + bounds + 
            //                                   " brush " + brush);

            if (_isCacheStaticLayouts)
            {
                DrawStringWithCachedLayout(s, brush, bounds);
                return;
            }

            using (var textLayout = new StaticLayout(s, this,
                Convert.ToInt32(bounds.Width),
                Layout.Alignment.AlignNormal,
                1, 1, false))
            {
                canvas.Save();

                canvas.Translate(Convert.ToSingle(bounds.X),
                    Convert.ToSingle(bounds.Y));

                textLayout.Draw(canvas);

                canvas.Restore();
            }
        }

        private void DrawStringWithCachedLayout<TBrush, TRectangle>(String s,
                                                                    TBrush brush,
                                                                    TRectangle bounds)
            where TBrush : IBrush
            where TRectangle : IRectangle
        {
            var canvas = GetCanvas();
            SetColor(brush);

            if (!_layoutCache.TryGetValue(bounds, out var textLayout))
            {
                textLayout = new StaticLayout(s, this,
                    Convert.ToInt32(bounds.Width),
                    Layout.Alignment.AlignNormal,
                    1, 1, false);
                _layoutCache.Add(bounds, textLayout);
            }

            canvas.Save();

            canvas.Translate(Convert.ToSingle(bounds.X),
                Convert.ToSingle(bounds.Y));

            textLayout.Draw(canvas);

            canvas.Restore();
        }

        public ValueSize MeasureString(String text)
        {
            var width = MeasureText(text);

            using (var textLayout = new StaticLayout(text, this,
                Convert.ToInt32(width),
                Layout.Alignment.AlignNormal,
                1, 1, false))
            {
                var height = textLayout.Height;
                return new ValueSize(width, height);
            }
        }

        public Canvas? Canvas { get; set; }

        public sealed override Single StrokeWidth
        {
            get => base.StrokeWidth;
            set => base.StrokeWidth = value;
        }

        public sealed override Single TextSize
        {
            get => base.TextSize;
            set => base.TextSize = value;
        }

        private Canvas GetCanvas()
        {
            return Canvas ?? throw new NullReferenceException();
        }

        private void SetColor(IBrush brush)
        {
            if (brush is SolidColorBrush scb)
                SetARGB(scb.Color.A, scb.Color.R, scb.Color.G, scb.Color.B);
        }

        public sealed override void SetStyle(Style? style)
        {
            base.SetStyle(style);
        }

        private readonly Dictionary<IRectangle, StaticLayout> _layoutCache;
    }
}