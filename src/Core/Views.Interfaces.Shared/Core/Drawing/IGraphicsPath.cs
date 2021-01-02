using System;
using Das.Views.Core.Geometry;

namespace Das.Views.Core.Drawing
{
    public interface IGraphicsPath : IDisposable
    {
        void LineTo<TPoint>(TPoint p1)
            where TPoint : IPoint2D;

        void AddArc<TRectangle>(TRectangle arc,
                                Single startAngle,
                                Single endAngle)
            where TRectangle : IRectangle;

        void CloseFigure();

        void SetRoundedRectangle<TThickness, TRect>(TRect bounds,
                                                    TThickness cornerRadii)
            where TThickness : IThickness
            where TRect : IRectangle;
    }
}
