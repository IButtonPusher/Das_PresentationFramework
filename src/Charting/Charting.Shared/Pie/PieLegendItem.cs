﻿using System;
using System.Threading.Tasks;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;

namespace Das.Views.Charting.Pie
{
    public class PieLegendItem : BindableElement,
                                 IVisualFinder
    {
        public PieLegendItem(IVisualBootstrapper templateResolver)
            : base(templateResolver)
        {
            _label = new Label(templateResolver);
        }

        public Boolean Contains(IVisualElement element)
        {
            return _label == element;
        }

        public IBrush? Brush { get; set; }

        public override void Arrange<TRenderSize>(TRenderSize availableSpace,
                                                 IRenderContext renderContext)
        {
            var h = availableSpace.Height * 0.7;
            var center = new ValuePoint2D(0, h);
            if (Brush is { } brush)
                renderContext.FillPie(center, h, 0, -90, brush);
            var rect = new ValueRenderRectangle(_offsetX, 0,
                availableSpace.Width - _offsetX, h, Point2D.Empty);
            renderContext.DrawElement(_label, rect);
        }

        public override ValueSize Measure<TRenderSize>(TRenderSize availableSpace,
                                                      IMeasureContext measureContext)
        {
            if (!_isStyleSet) _isStyleSet = true;

            var sz = _label.Measure(availableSpace, measureContext);
            _offsetX = sz.Height + 5;
            var res = new ValueSize(sz.Width + _offsetX, sz.Height);
            return res;
        }

        public override void Dispose()
        {
           _label.Dispose();
           
           base.Dispose();
           
        }

        protected override void OnDataContextChanged(Object? newValue)
        {
            base.OnDataContextChanged(newValue);
            _label.Text = newValue?.ToString() ?? String.Empty;
        }


        private readonly Label _label;
        private Boolean _isStyleSet;
        private Double _offsetX;
    }
}