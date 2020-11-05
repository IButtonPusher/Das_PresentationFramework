using System;
using System.Collections.Generic;
using System.IO;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Rendering;

namespace Das.Xamarin.Android.Rendering
{
    public class RefreshRenderContext : BaseRenderContext
    {
        public RefreshRenderContext(IViewPerspective perspective,
                                    IVisualSurrogateProvider surrogateProvider,
                                    Dictionary<IVisualElement, ICube> renderPositions)
            : base(perspective, surrogateProvider, renderPositions)
        {
            
        }

        public override IImage? GetImage(Stream stream)
        {
            throw new NotSupportedException();
        }

        public override IImage GetNullImage()
        {
            throw new NotSupportedException();
        }

        public override void DrawImage(IImage img, IRectangle rect)
        {
        }

        public override void DrawString(String s, IFont font, IBrush brush, IPoint2D point2D)
        {
        }

        public override void DrawImage(IImage img, IRectangle sourceRest, IRectangle destination)
        {
        }

        public override void DrawLine(IPen pen, IPoint2D pt1, IPoint2D pt2)
        {
        }

        public override void DrawLines(IPen pen, IPoint2D[] points)
        {
        }

        public override void DrawRect(IRectangle rect, IPen pen)
        {
        }

        public override void DrawRoundedRect(IRectangle rect, IPen pen, Double cornerRadius)
        {
        }

        public override void DrawString(String s, IFont font, IBrush brush, IRectangle location)
        {
        }

        public override void FillPie(IPoint2D center, Double radius, Double startAngle, Double endAngle, IBrush brush)
        {
        }

        public override void DrawEllipse(IPoint2D center, Double radius, IPen pen)
        {
        }

        public override void DrawFrame(IFrame frame)
        {
        }

        public override void FillRectangle(IRectangle rect, IBrush brush)
        {
        }

        public override void FillRoundedRectangle(IRectangle rect, IBrush brush, Double cornerRadius)
        {
        }
    }
}