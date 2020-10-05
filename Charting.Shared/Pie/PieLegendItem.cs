using System;
using System.Threading.Tasks;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Panels;
using Das.Views.Rendering;

namespace Das.Views.Charting.Pie
{
    public class PieLegendItem<TKey, TValue> : BindableElement<IDataPoint<TKey, TValue>>,
                                               IVisualFinder
        where TValue : IConvertible
    {
        public PieLegendItem()
        {
            _label = new Label<IDataPoint<TKey, TValue>>();
        }

        public Boolean Contains(IVisualElement element)
        {
            return _label == element;
        }

        public IBrush Brush { get; set; }

        public override void Arrange(ISize availableSpace, IRenderContext renderContext)
        {
            var h = availableSpace.Height * 0.7;
            var center = new Point2D(0, h);
            renderContext.FillPie(center, h, 0, -90, Brush);
            var rect = new Rectangle(_offsetX, 0, availableSpace.Width - _offsetX, h);
            renderContext.DrawElement(_label, rect);
        }

        public override void Dispose()
        {
        }

        public override ISize Measure(ISize availableSpace, IMeasureContext measureContext)
        {
            if (!_isStyleSet) _isStyleSet = true;

            var sz = _label.Measure(availableSpace, measureContext);
            _offsetX = sz.Height + 5;
            var res = new Size(sz.Width + _offsetX, sz.Height);
            return res;
        }

        public override void SetBoundValue(IDataPoint<TKey, TValue> value)
        {
            base.SetBoundValue(value);
            _label.SetBoundValue(value);
        }

        private readonly Label<IDataPoint<TKey, TValue>> _label;
        private Boolean _isStyleSet;
        private Double _offsetX;
    }
}