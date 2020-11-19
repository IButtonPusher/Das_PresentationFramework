using System;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;

namespace Das.Views.Core.Writing
{
    public interface IFontRenderer : IDisposable
    {
        IFont Font { get; }

        void DrawString(String text,
                        IBrush brush,
                        IPoint2D point2D);

        void DrawString(String s,
                        IBrush brush,
                        IRectangle bounds);

        ValueSize MeasureString(String text);
    }
}