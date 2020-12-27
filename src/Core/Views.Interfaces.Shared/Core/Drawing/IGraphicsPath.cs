using System;
using System.Collections.Generic;
using System.Text;
using Das.Views.Core.Geometry;

namespace Das.Views.Core.Drawing
{
    public interface IGraphicsPath : IDisposable
    {
        void LineTo<TPoint>(TPoint p1)
            where TPoint : IPoint2D;

        //void AddLine<TPoint>(TPoint p1,
        //                     TPoint p2)
        //    where TPoint : IPoint2D;

        void AddArc<TRectangle>(TRectangle arc, 
                                Single startAngle,
                                Single endAngle)
            where TRectangle : IRectangle;

        void CloseFigure();
    }
}
