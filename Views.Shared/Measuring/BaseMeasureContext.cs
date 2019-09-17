using Das.Views.Rendering;
using Das.Views.Styles;
using System;
using System.Collections.Generic;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;

namespace Das.Views.Measuring
{
    public abstract class BaseMeasureContext : IMeasureContext
    {
        public BaseMeasureContext()
        {
            _empty = new Size(0, 0);
            _lastMeasurements = new Dictionary<IVisualElement, Size>();
        }

        public IViewState ViewState { get; set; }

        private readonly Size _empty;
        private readonly Dictionary<IVisualElement, Size> _lastMeasurements;

        public abstract Size MeasureImage(IImage img);

        public Size MeasureElement(IVisualElement element, ISize availableSpace)
        {
            var viewState = ViewState;
            var zoom = viewState.ZoomLevel;

            var margin = viewState.GetStyleSetter<Thickness>(StyleSetters.Margin, element)
                         * zoom;
            var border = viewState.GetStyleSetter<Thickness>(StyleSetters.BorderThickness, element)
                         * zoom;

            var specificSize = viewState.GetStyleSetter<Size>(StyleSetters.Size, element)
                               * zoom;
            var desiredSize = element.Measure(availableSpace, this);

            var specificHeight = viewState.GetStyleSetter<Double>(StyleSetters.Height, element)
                                 * zoom;
            var specificWidth = viewState.GetStyleSetter<Double>(StyleSetters.Width, element)
                                * zoom;

            var size = specificSize ?? desiredSize;
            size = new Size(Double.IsNaN(specificWidth) ? size.Width : specificWidth,
                Double.IsNaN(specificHeight) ? size.Height : specificHeight);

            var total = Size.Add(size, margin, border);
            _lastMeasurements[element] = total;
            return total;
        }

        public Size GetLastMeasure(IVisualElement element)
            => _lastMeasurements.TryGetValue(element, out var val) ? val : _empty;

        public abstract Size MeasureString(String s, Font font);

        public T GetStyleSetter<T>(StyleSetters setter, IVisualElement element)
            => ViewState.GetStyleSetter<T>(setter, element);
    }
}