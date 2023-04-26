using System;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Images;
using Das.Views.Input;

namespace Das.Views.Rendering;

public interface IRenderContext : IVisualContext,
                                  IElementLocator
{
   void DrawElement<TRenderRectangle>(IVisualElement visual,
                                      TRenderRectangle rect)
      where TRenderRectangle : IRenderRectangle;


   void DrawEllipse<TPoint, TPen>(TPoint center,
                                  Double radius,
                                  TPen pen)
      where TPoint : IPoint2D
      where TPen : IPen;

   void DrawFrame(IFrame frame);

   void DrawImageAt<TLocation>(IImage img,
                               TLocation destination)
      where TLocation : IPoint2D;

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

   void DrawRoundedRect<TRectangle, TPen, TThickness>(TRectangle rect,
                                                      TPen pen,
                                                      TThickness cornerRadii)
      where TRectangle : IRectangle
      where TPen : IPen
      where TThickness : IThickness;

   void DrawString<TFont, TBrush, TPoint>(String s,
                                          TFont font,
                                          TBrush brush,
                                          TPoint location)
      where TFont : IFont
      where TBrush : IBrush
      where TPoint : IPoint2D;

   /// <summary>
   ///    Draws the string within the provided rectangle.  Wraps text as needed
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

   void FillRoundedRectangle<TRectangle, TBrush, TThickness>(TRectangle rect,
                                                             TBrush brush,
                                                             TThickness cornerRadii)
      where TRectangle : IRectangle
      where TBrush : IBrush
      where TThickness : IThickness;

   IViewPerspective Perspective { get; }

   IViewState ViewState { get; }

   ValuePoint2D CurrentLocation { get; }
}