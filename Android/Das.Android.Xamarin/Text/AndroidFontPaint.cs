using System;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Text;
using Android.Util;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Size = Das.Views.Core.Geometry.Size;

namespace Das.Xamarin.Android
{
    public class AndroidFontPaint : TextPaint, IFontRenderer
    {
        public AndroidFontPaint(IFont font,
                                DisplayMetrics displayMetrics)
            : base(PaintFlags.AntiAlias)
        {
            Font = font;
            SetStyle(Style.Fill);

            var px = TypedValue.ApplyDimension(ComplexUnitType.Dip,
                Convert.ToSingle(font.Size),
                displayMetrics);

            TextSize = px;
            StrokeWidth = 10;
        }

        public IFont Font { get; }


        public void DrawString(String text, 
                               IBrush brush, 
                               IPoint2D point2D)
        {
            SetColor(brush);

            //GetCanvas().DrawText(text, (Single) point2D.X + 50, (Single) point2D.Y + 50, this);
            GetCanvas().DrawText(text, (Single) point2D.X, (Single) point2D.Y, this);
        }

        public void DrawString(String s, 
                               IBrush brush, 
                               IRectangle bounds)
        {
            var canvas = GetCanvas();

            SetColor(brush);

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

        public Size MeasureString(String text)
        {
            //var bounds = new Rect();
            var width = MeasureText(text);

            //GetTextBounds(text, 0, text.Length, bounds);

            using (var textLayout = new StaticLayout(text, this,
                Convert.ToInt32(width),
                Layout.Alignment.AlignNormal,
                1, 1, false))
            {
                var height = textLayout.Height;
                return new Size(width, height);
            }

            //return new Size(bounds.Width(), bounds.Height());
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
    }
}