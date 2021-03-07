using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views;
using Das.Views.Colors;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Images;
using Das.Views.Rendering;

namespace Das.Xamarin.Android.Rendering
{
    /// <summary>
    /// Faciliates size adjustments of visual elements without a full layout pass 
    ///  when surrogates (native android views) are in the visual tree
    /// </summary>
    public class RefreshRenderContext : BaseRenderContext
    {
        private readonly Func<ValueRectangle> _getClip;

        public RefreshRenderContext(IViewPerspective perspective,
                                    IVisualSurrogateProvider surrogateProvider,
                                    Dictionary<IVisualElement, ValueCube> renderPositions,
                                    Dictionary<IVisualElement, ValueSize> lastMeasurements,
                                    IThemeProvider themeProvider,
                                    IVisualLineage visualLineage,
                                    ILayoutQueue layoutQueue,
                                    Func<ValueRectangle> getClip)
            : base(perspective, surrogateProvider, renderPositions,
                lastMeasurements, themeProvider, visualLineage, layoutQueue)
        {
            _getClip = getClip;
        }

        public override void DrawEllipse<TPoint, TPen>(TPoint center,
                                                       Double radius,
                                                       TPen pen)
        {
        }

        public override void DrawFrame(IFrame frame)
        {
        }

        public override void DrawImage<TRectangle>(IImage img,
                                                   TRectangle destination)
        {
        }

        public override void DrawImage<TRectangle1, TRectangle2>(IImage img,
                                                                 TRectangle1 sourceRect,
                                                                 TRectangle2 destination)
        {
        }

        public override void DrawLine<TPen, TPoint1, TPoint2>(TPen pen,
                                                              TPoint1 pt1,
                                                              TPoint2 pt2)
        {
        }

        public override void DrawLines(IPen pen,
                                       IPoint2D[] points)
        {
        }

        public override void DrawRect<TRectangle, TPen>(TRectangle rect,
                                                        TPen pen)
        {
        }

        public override void DrawRoundedRect<TRectangle, TPen, TThickness>(TRectangle rect,
                                                                           TPen pen,
                                                                           TThickness cornerRadii)
        {
            
        }

        public override void DrawString<TFont, TBrush, TPoint>(String s,
                                                               TFont font,
                                                               TBrush brush,
                                                               TPoint location)
        {
        }

        public override void DrawString<TFont, TBrush, TRectangle>(String s,
                                                                   TFont font,
                                                                   TRectangle location,
                                                                   TBrush brush)
        {
        }

        public override void FillPie<TPoint, TBrush>(TPoint center,
                                                     Double radius,
                                                     Double startAngle,
                                                     Double endAngle,
                                                     TBrush brush)
        {
        }

        public override void FillRectangle<TRectangle, TBrush>(TRectangle rect,
                                                               TBrush brush)
        {
        }

        public override void FillRoundedRectangle<TRectangle, TBrush, TThickness>(TRectangle rect,
            TBrush brush,
            TThickness cornerRadii)
        {
        }

        protected override ValueRectangle GetCurrentClip()
        {
            return _getClip();
            //return ValueRectangle.Empty;
        }

        protected override void PopClip<TRectangle>(TRectangle rect)
        {
        }


        protected override void PushClip<TRectangle>(TRectangle rect)
        {
        }

        //protected override void SetElementRenderPosition(ValueRenderRectangle useRect,
        //                                                 IVisualElement element)
        //{
        //    //base.SetElementRenderPosition(useRect, element);
        //}
    }
}
