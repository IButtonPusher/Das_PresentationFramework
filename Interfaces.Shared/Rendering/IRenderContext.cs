using System;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Input;
using Das.Views.Core;

namespace Das.Views.Rendering
{
    public interface IRenderContext : IVisualContext, 
                                      IElementLocator,
                                      IImageProvider
    {
        IViewPerspective Perspective { get; }

        //IDictionary<IVisualElement, ICube> RenderPositions { get; }

        IViewState? ViewState { get; set; }

        /// <summary>
        ///     Returns the actual rectangle occupied by the element, including borders etc
        /// </summary>
        Rectangle DrawElement(IVisualElement element, 
                              IRectangle rect);

        void DrawEllipse(IPoint2D center, 
                         Double radius, 
                         IPen pen);

        void DrawFrame(IFrame frame);

        void DrawImage(IImage img, 
                       IRectangle destination);

        void DrawImage(IImage img, 
                       IRectangle sourceRest,
                       IRectangle destination);

        void DrawLine(IPen pen, IPoint2D pt1, 
                      IPoint2D pt2);

        void DrawLines(IPen pen, 
                       IPoint2D[] points);

        void DrawRect(IRectangle rect, 
                      IPen pen);

        void DrawRoundedRect(IRectangle rect, 
                      IPen pen,
                      Double cornerRadius);

        void DrawString(String s, 
                        IFont font, 
                        IBrush brush, 
                        IPoint2D location);

        /// <summary>
        /// Draws the string within the provided rectangle.  Wraps text as needed
        /// </summary>
        void DrawString(String s, 
                        IFont font, 
                        IBrush brush, 
                        IRectangle rect);

        void FillPie(IPoint2D center, 
                     Double radius, 
                     Double startAngle,
                     Double endAngle, 
                     IBrush brush);

        void FillRectangle(IRectangle rect, 
                      IBrush brush);

        void FillRoundedRectangle(IRectangle rect, 
                             IBrush brush,
                             Double cornerRadius);
    }
}