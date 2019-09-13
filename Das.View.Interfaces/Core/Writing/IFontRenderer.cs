using System;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;

namespace Das.Views.Core.Writing
{
    public interface IFontRenderer : IDisposable
    {
        IFont Font { get; }

        void DrawString(String text, IBrush brush, IPoint point);

        Size MeasureString(String text);
    }
}
