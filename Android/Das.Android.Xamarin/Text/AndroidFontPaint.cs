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
    public class AndroidFontPaint : TextPaint, IFontRenderer
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
            _layoutCache = new Dictionary<String, StaticLayout>();
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
        public void DrawString(String text, 
                               IBrush brush, 
                               IPoint2D point2D)
        {
            //SetColor(brush);

            var size = MeasureString(text);
            var test = new ValueRectangle(point2D, size.Width, size.Height);
            DrawString(text, brush, test);

            //GetCanvas().DrawText(text, (Single) point2D.X, (Single) point2D.Y, this);
        }

        public void DrawString(String s, 
                               IBrush brush, 
                               IRectangle bounds)
        {
            var canvas = GetCanvas();

            SetColor(brush);

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

        private void DrawStringWithCachedLayout(String s,
                                                IBrush brush,
                                                IRectangle bounds)
        {
            var canvas = GetCanvas();
            SetColor(brush);

            if (!_layoutCache.TryGetValue(s, out var textLayout))
            {
                textLayout = new StaticLayout(s, this,
                    Convert.ToInt32(bounds.Width),
                    Layout.Alignment.AlignNormal,
                    1, 1, false);
                _layoutCache.Add(s, textLayout);
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

        private readonly Dictionary<String, StaticLayout> _layoutCache;
    }
}