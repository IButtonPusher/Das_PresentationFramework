using System;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Input;

namespace Das.Views.Rendering
{
    public interface IRenderContext : IVisualContext,
                                      IElementLocator
    {
        IViewPerspective Perspective { get; }

        IViewState? ViewState { get; }

        /// <summary>
        ///     Returns the actual rectangle occupied by the element, including borders etc
        /// </summary>
        void DrawElement<TRenderRectangle>(IVisualElement element,
                                                TRenderRectangle rect)
            where TRenderRectangle : IRenderRectangle;

        void DrawContentElement<TSize>(IVisualElement element,
                                          TSize size)
            where TSize : ISize;


        void DrawElementAt<TPosition>(IVisualElement element,
                                         TPosition location)
            where TPosition : IPoint2D;


        void DrawEllipse<TPoint, TPen>(TPoint center,
                                       Double radius,
                                       TPen pen)
            where TPoint : IPoint2D
            where TPen : IPen;

        void DrawFrame(IFrame frame);

        void DrawImage<TRectangle>(IImage img,
                                   TRectangle destination)
            where TRectangle : IRectangle;

        void DrawImage<TRectangle1, TRectangle2>(IImage img,
                                                 TRectangle1 sourceRect,
                                                 TRectangle2 destination)
            where TRectangle1 : IRectangle
            where TRectangle2 : IRectangle;

        void DrawLine<TPen, TPoint1, TPoint2>(TPen pen,
                                              TPoint1 pt1,
                                              TPoint2 pt2)
            where TPen : IPen
            where TPoint1 : IPoint2D
            where TPoint2 : IPoint2D;

        void DrawLines(IPen pen,
                       IPoint2D[] points);


        void DrawMainElement<TRectangle>(IVisualElement element,
                                              TRectangle rect,
                                              IViewState viewState)
            where TRectangle : IRectangle;

        void DrawRect<TRectangle, TPen>(TRectangle rect,
                                        TPen pen)
            where TRectangle : IRectangle
            where TPen : IPen;

        void DrawRoundedRect<TRectangle, TPen>(TRectangle rect,
                                               TPen pen,
                                               Double cornerRadius)
            where TRectangle : IRectangle
            where TPen : IPen;

        void DrawString<TFont, TBrush, TPoint>(String s,
                                               TFont font,
                                               TBrush brush,
                                               TPoint location)
            where TFont : IFont
            where TBrush : IBrush
            where TPoint : IPoint2D;

        /// <summary>
        ///     Draws the string within the provided rectangle.  Wraps text as needed
        /// </summary>
        void DrawString<TFont, TBrush, TRectangle>(String s,
                                                   TFont font,
                                                   TRectangle rect,
                                                   TBrush brush)
            where TFont : IFont
            where TBrush : IBrush
            where TRectangle : IRectangle;


        void FillPie<TPoint, TBrush>(TPoint center,
                                     Double radius,
                                     Double startAngle,
                                     Double endAngle,
                                     TBrush brush)
            where TPoint : IPoint2D
            where TBrush : IBrush;

        void FillRectangle<TRectangle, TBrush>(TRectangle rect,
                                               TBrush brush)
            where TRectangle : IRectangle
            where TBrush : IBrush;

        void FillRoundedRectangle<TRectangle, TBrush>(TRectangle rect,
                                                      TBrush brush,
                                                      Double cornerRadius)
            where TRectangle : IRectangle
            where TBrush : IBrush;
    }
}