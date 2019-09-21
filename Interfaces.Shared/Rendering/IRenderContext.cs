using System;
using System.Collections.Generic;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Input;

namespace Das.Views.Rendering
{
    public interface IRenderContext : IVisualContext, IElementLocator
    {
        IViewState ViewState { get; set; }

        IViewPerspective Perspective { get; }

        void DrawString(String s, IFont font, IBrush brush, IPoint location);

        void DrawString(String s, IFont font, IBrush brush, IRectangle location);

        void DrawImage(IImage img, IRectangle rect);

        void DrawLine(IPen pen, IPoint pt1, IPoint pt2);

        void DrawLines(IPen pen, IPoint[] points);

        void FillRect(IRectangle rect, IBrush brush);

        void DrawRect(IRectangle rect, IPen pen);

        void FillPie(IPoint center, Double radius, Double startAngle, 
            Double endAngle, IBrush brush);

        void DrawEllipse(IPoint center, Double radius, IPen pen);

        void DrawFrame(IFrame frame);

        /// <summary>
        /// Returns the actual rectangle occupied by the element, including borders etc
        /// </summary>        
        Rectangle DrawElement(IVisualElement element, IRectangle rect);

        IDictionary<IVisualElement, ICube> RenderPositions { get; }
    }
}