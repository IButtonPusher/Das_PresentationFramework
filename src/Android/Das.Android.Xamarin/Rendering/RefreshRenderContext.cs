﻿using System;
using System.Collections.Generic;
using Das.Views;
using Das.Views.Colors;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;

namespace Das.Xamarin.Android.Rendering
{
    public class RefreshRenderContext : BaseRenderContext
    {
        public RefreshRenderContext(IViewPerspective perspective,
                                    IVisualSurrogateProvider surrogateProvider,
                                    Dictionary<IVisualElement, ValueCube> renderPositions,
                                    Dictionary<IVisualElement, ValueSize> lastMeasurements,
                                    IThemeProvider themeProvider,
                                    IVisualLineage visualLineage,
                                    ILayoutQueue layoutQueue)
            : base(perspective, surrogateProvider, renderPositions, 
                lastMeasurements, themeProvider, visualLineage, layoutQueue)
        {
            
        }

        //public override IImage? GetImage(Stream stream)
        //{
        //    throw new NotSupportedException();
        //}

        //public override IImage? GetImage(Stream stream, 
        //                                 Double maximumWidthPct)
        //{
        //    throw new NotSupportedException();
        //}

        //public override IImage GetNullImage()
        //{
        //    throw new NotSupportedException();
        //}

        public override  void DrawImage<TRectangle>(IImage img, 
                                                    TRectangle destination)
        {
        }

        public override void DrawRoundedRect<TRectangle, TPen, TThickness>(TRectangle rect, TPen pen, TThickness cornerRadii)
        {
            throw new NotImplementedException();
        }

        public override void DrawString<TFont, TBrush, TPoint>(String s,
                                                               TFont font,
                                                               TBrush brush,
                                                               TPoint location)
        {
        }

        public override  void DrawImage<TRectangle1, TRectangle2>(IImage img,
                                                                  TRectangle1 sourceRect,
                                                                  TRectangle2 destination)
        {
        }

        public override void DrawLine<TPen, TPoint1, TPoint2>(TPen pen,
                                                              TPoint1 pt1,
                                                              TPoint2 pt2)
        {
        }

        public override void DrawLines(IPen pen, IPoint2D[] points)
        {
        }

        public override void FillRoundedRectangle<TRectangle, TBrush, TThickness>(TRectangle rect, TBrush brush, TThickness cornerRadii)
        {
            throw new NotImplementedException();
        }

        public override void DrawRect<TRectangle, TPen>(TRectangle rect,
                                                        TPen pen)
        {
        }

       

        protected override void PushClip<TRectangle>(TRectangle rect)
        {
            
        }

        protected override void PopClip<TRectangle>(TRectangle rect)
        {
            
        }

        protected override ValueRectangle GetCurrentClip()
        {
            return ValueRectangle.Empty;
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

        public override void DrawEllipse<TPoint, TPen>(TPoint center,
                                                       Double radius,
                                                       TPen pen)
        {
        }

        public override void DrawFrame(IFrame frame)
        {
        }

        public override void FillRectangle<TRectangle, TBrush>(TRectangle rect,
                                                               TBrush brush)
        {
        }
    }
}