﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;

namespace Das.Views.Charting.Pie
{
    // ReSharper disable once UnusedType.Global
    public class PieChart<TKey, TValue> : BindableElement,
                                          IVisualFinder
        where TValue : IConvertible
    {
        private readonly IVisualBootstrapper _templateResolver;

        public PieChart(IVisualBootstrapper templateResolver) 
            : base(templateResolver)
        {
            _legendLock = new Object();

            _templateResolver = templateResolver;
            _desiredSize = new ValueSize(1, 1);
            _defaultedColors = new Dictionary<TKey, IBrush>();
            _random = new Random();
            _outline = new Pen(Color.DarkGray, 1);
            _legendOutline = new Pen(new Color(203, 212, 225), 1);
            _legendItems = new List<PieLegendItem>();
            _legendItemSizes = new List<ISize>();
            _legendBackground = new SolidColorBrush(Color.White);
        }

        public Boolean Contains(IVisualElement element)
        {
            lock (_legendLock)
                return _legendItems.Any(l => l.Contains(element));
        }

        public override void Arrange<TRenderSize>(TRenderSize availableSpace, 
                                                 IRenderContext renderContext)
        {
            var side = Math.Min(availableSpace.Width, availableSpace.Height);
            if (side.IsZero() || !(DataContext is {} binding))
                
            {
                return;
            }

            renderContext.FillRectangle(availableSpace.ToFullRectangle(),
                renderContext.ColorPalette.Background);

            var radius = side / 2;
            var center = new ValuePoint2D(availableSpace.Width - side + radius,
                availableSpace.Height - side + side / 2);
            
            var currentValue = binding as IPieData<TKey, TValue>;
            if (currentValue == null)
                return;
            
            var data = currentValue.Items.ToArray();

            var pntCnt = data.Length;
            var doubles = new Double[pntCnt];

            Double valSum = 0;
            var anglePcts = new Double[pntCnt];

            for (var c = 0; c < pntCnt; c++)
            {
                var dbl = Convert.ToDouble(data[c].Value);
                doubles[c] = dbl;
                valSum += dbl;
            }

            for (var c = 0; c < pntCnt; c++)
                anglePcts[c] = doubles[c] / valSum * 360;

            var currentRadius = 0.0;

            for (var c = 0; c < pntCnt; c++)
            {
                var current = data[c];
                var val = anglePcts[c] + currentRadius;
                var brush = GetBrush(currentValue, current);

                renderContext.FillPie(center, radius, currentRadius, val, brush);

                currentRadius = val;
            }

            //elliptical border
            renderContext.DrawEllipse(center, radius, _outline);

            ArrangeLegend(renderContext);
        }

        private void ArrangeLegend(IRenderContext renderContext)
        {
            lock (_legendLock)
            {
                if (_legendItems.Count == 0 || _legendItemSizes.Count == 0)
                    return;

                var padding = 20;
                var rowMargin = 5;
                var totalRowMargin = _legendItems.Count * rowMargin;

                var w = _legendItemSizes.Max(s => s.Width) + padding;
                var h = _legendItemSizes.Sum(s => s.Height) + padding + totalRowMargin;

                var rect = new RenderRectangle(0, 0, w, h, Point2D.Empty);

                renderContext.FillRectangle(rect, _legendBackground);
                renderContext.DrawRect(rect, _legendOutline);

                var i = 0;
                rect.X = 10;
                rect.Width -= 10;
                rect.Y = 10;
                foreach (var item in _legendItems)
                {
                    rect.Height = _legendItemSizes[i].Height;
                    renderContext.DrawElement(item, rect);

                    rect.Y += rect.Height + rowMargin;
                }
            }
        }

        public override void Dispose()
        {
            DisposeLegend();

           base.Dispose();
        }


        private IBrush GetBrush(IPieData<TKey, TValue> value, 
                                IDataPoint<TKey, TValue> current)
        {
            if (!value.ItemColors.TryGetValue(current.Description, out var brush)
                && !_defaultedColors.TryGetValue(current.Description, out brush))
            {
                var bytes = new Byte[3];
                _random.NextBytes(bytes);
                var color = new Color(bytes[0], bytes[1], bytes[2]);
                brush = new SolidColorBrush(color);
                _defaultedColors[current.Description] = brush;
            }

            return brush;
        }

        public override ValueSize Measure<TRenderSize>(TRenderSize availableSpace, 
                                                      IMeasureContext measureContext)
        {
            Double widestLegend;

            lock (_legendLock)
            {
                _legendItemSizes.Clear();
                foreach (var item in _legendItems)
                {
                    var sz = item.Measure(availableSpace, measureContext);
                    _legendItemSizes.Add(sz);
                }

                widestLegend = _legendItemSizes.Any()
                    ? _legendItemSizes.Max(l => l.Width)
                    : 0;
            }

            var side = Math.Min(availableSpace.Width, availableSpace.Height);
            if (side.IsZero())
                return ValueSize.Empty;


            _desiredSize.Height = side;

            if (side + widestLegend <= availableSpace.Width)
                side += widestLegend;
            _desiredSize.Width = side;

            return availableSpace.ToValueSize();
            //return _desiredSize;
        }

        private void DisposeLegend()
        {
           lock (_legendLock)
           {
              foreach (var item in _legendItems)
              {
                 item.Dispose();
              }

              _legendItems.Clear();
              _legendItemSizes.Clear();
           }
        }

        protected override void OnDataContextChanged(Object? newValue)
        {
            DisposeLegend();

            base.OnDataContextChanged(newValue);

            if (newValue is not IPieData<TKey, TValue> currentValue)
                return;

            var data = currentValue.Items.OrderByDescending(v => v.Value).ToArray();
            var pntCnt = data.Length;

            for (var c = 0; c < pntCnt; c++)
            {
                var current = data[c];

                var brush = GetBrush(currentValue, current);

                var legendItem = new PieLegendItem(_templateResolver);
                
                legendItem.DataContext = current;
                legendItem.Brush = brush;
                lock (_legendLock)
                    _legendItems.Add(legendItem);
            }
        }

        private readonly Dictionary<TKey, IBrush> _defaultedColors;

        private readonly Size _desiredSize;

        private readonly SolidColorBrush _legendBackground;
        private readonly List<PieLegendItem> _legendItems;
        private readonly Object _legendLock;
        private readonly List<ISize> _legendItemSizes;
        private readonly Pen _legendOutline;
        private readonly Pen _outline;
        private readonly Random _random;
    }
}