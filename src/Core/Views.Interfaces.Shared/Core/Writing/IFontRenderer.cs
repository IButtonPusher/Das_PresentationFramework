using System;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;

namespace Das.Views.Core.Writing;

public interface IFontRenderer : IDisposable
{
   IFont Font { get; }

   void DrawString<TBrush, TPoint>(String text,
                                   TBrush brush,
                                   TPoint point2D)
      where TBrush : IBrush
      where TPoint : IPoint2D;

   void DrawStringInRect<TBrush, TRectangle>(String s,
                                             TBrush brush,
                                             TRectangle bounds)
      where TBrush : IBrush
      where TRectangle : IRectangle;

   ValueSize MeasureString(String text);
}