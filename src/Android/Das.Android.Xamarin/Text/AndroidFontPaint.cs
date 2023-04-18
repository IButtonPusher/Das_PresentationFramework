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
      private readonly Boolean _isCacheStaticLayouts;

      public AndroidFontPaint(IFont font,
                              DisplayMetrics displayMetrics,
                              Boolean isCacheStaticLayouts)
         : base(PaintFlags.AntiAlias)
      {
         _isCacheStaticLayouts = isCacheStaticLayouts;
         _lockLayoutCache = new Object();
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

      public void DrawString<TBrush, TPoint>(String text,
                                             TBrush brush,
                                             TPoint point2D)
         where TBrush : IBrush
         where TPoint : IPoint2D
      {
         if (_isCacheStaticLayouts)
         {
            var layout = GetOrCreateStaticLayout(text);
            DrawStringWithCachedLayout(brush, point2D.X, point2D.Y, layout);
            return;
         }

         var size = MeasureString(text);
         var test = new ValueRectangle(point2D, size.Width, size.Height);
         DrawStringInRect(text, brush, test);
      }

      public void DrawStringInRect<TBrush, TRectangle>(String s,
                                                       TBrush brush,
                                                       TRectangle bounds)
         where TBrush : IBrush
         where TRectangle : IRectangle
      {

         if (_isCacheStaticLayouts)
         {
            var layout = GetOrCreateStaticLayout(s, _ => Convert.ToSingle(bounds.Width));
            DrawStringWithCachedLayout(brush, bounds.X, bounds.Y, layout);
            return;
         }

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

      private void DrawStringWithCachedLayout<TBrush>(TBrush brush,
                                                      Double x,
                                                      Double y,
                                                      StaticLayout textLayout)
         where TBrush : IBrush
      {
         var canvas = GetCanvas();
         SetColor(brush);

         canvas.Save();

         canvas.Translate(Convert.ToSingle(x),
            Convert.ToSingle(y));

         textLayout.Draw(canvas);

         canvas.Restore();
      }

      public ValueSize MeasureString(String text)
      {
         var width = MeasureText(text);

         if (_isCacheStaticLayouts)
         {
            var layout = GetOrCreateStaticLayout(text);
            return new ValueSize(layout.Width, layout.Height);
         }

         using (var textLayout = new StaticLayout(text, this,
                   Convert.ToInt32(width),
                   Layout.Alignment.AlignNormal,
                   1, 1, false))
         {
            var height = textLayout.Height;
            return new ValueSize(width, height);
         }
      }

      private StaticLayout GetOrCreateStaticLayout(String text)
      {
         return GetOrCreateStaticLayout(text, MeasureText);
      }

      private StaticLayout GetOrCreateStaticLayout(String text,
                                                   Func<String, Single> getWidth)
      {
         lock (_lockLayoutCache)
         {
            if (!_layoutCache.TryGetValue(text, out var textLayout))
            {
               var width = getWidth(text);
               textLayout = new StaticLayout(text, this,
                  Convert.ToInt32(width),
                  Layout.Alignment.AlignNormal,
                  1, 1, false);

               _layoutCache.Add(text, textLayout);
            }

            return textLayout;
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
      private readonly Object _lockLayoutCache;
   }
}