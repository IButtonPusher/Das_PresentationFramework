using System;
using Das.Views.Core.Geometry;

namespace Das.Views.Core.Drawing
{
    public interface IGraphicsPath : IDisposable
    {
        void LineTo<TPoint>(TPoint p1)
            where TPoint : IPoint2D;

        void AddLine<TPoint>(TPoint p1,
                             TPoint p2)
            where TPoint : IPoint2F;

        void AddArc<TRectangle>(TRectangle arc,
                                Single startAngle,
                                Single endAngle)
            where TRectangle : IRectangle;

        void AddBezier(Single x1,
                       Single y1,
                       Single x2,
                       Single y2,
                       Single x3,
                       Single y3,
                       Single x4,
                       Single y4);

        void AddBezier(IPoint2F p1,
                       IPoint2F p2,
                       IPoint2F p3,
                       IPoint2F p4);

        void StartFigure();

        void CloseFigure();

        ValueSize Size { get; }

        void SetRoundedRectangle<TThickness, TRect>(TRect bounds,
                                                    TThickness cornerRadii)
            where TThickness : IThickness
            where TRect : IRectangle;

        IPathData PathData { get; }

        public IColor Stroke { get; set; }

        public IBrush? Fill { get; set; }

        /// <summary>
        /// Gets the underlying system specific object
        /// </summary>
        T Unwrap<T>();
    }
}
