using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views.Measuring
{
    public abstract class BaseMeasureContext : ContextBase, IMeasureContext
    {
        protected BaseMeasureContext()
        {
            _empty = new Size(0, 0);
            _contextBounds = Size.Empty;
            _lastMeasurements = new Dictionary<IVisualElement, Size>();
        }

        public virtual Size MeasureImage(IImage img)
        {
            return new Size(img.Width, img.Height);
        }

        public Size MeasureMainView(IVisualElement element,
                                    ISize availableSpace,
                                    IViewState viewState)
        {
            ViewState = viewState;
            _contextBounds = availableSpace;
            return MeasureElement(element, availableSpace);
        }

        public Size MeasureElement(IVisualElement element,
                                   ISize availableSpace)
        {
            var viewState = GetViewState();
            var zoom = viewState.ZoomLevel;

            var margin = viewState.GetStyleSetter<Thickness>(StyleSetter.Margin, element)
                         * zoom;
            var border = viewState.GetStyleSetter<Thickness>(StyleSetter.BorderThickness, element)
                         * zoom;

            var specificSize = viewState.GetStyleSetter<Size>(StyleSetter.Size, element)
                               * zoom;
            var desiredSize = element.Measure(availableSpace, this);

            var specificHeight = viewState.GetStyleSetter<Double>(StyleSetter.Height, element)
                                 * zoom;
            var specificWidth = viewState.GetStyleSetter<Double>(StyleSetter.Width, element)
                                * zoom;

            var size = specificSize ?? desiredSize;
            size = new Size(Double.IsNaN(specificWidth) ? size.Width : specificWidth,
                Double.IsNaN(specificHeight) ? size.Height : specificHeight);

            var total = Size.Add(size, margin, border);
            _lastMeasurements[element] = total;
            return total;
        }

        public Size GetLastMeasure(IVisualElement element)
        {
            return _lastMeasurements.TryGetValue(element, out var val) ? val : _empty;
        }

        public abstract Size MeasureString(String s, 
                                           IFont font);

        public virtual ISize ContextBounds => _contextBounds;

        public Double GetZoomLevel()
        {
            return ViewState?.ZoomLevel ?? 1;
        }

        private readonly Size _empty;
        private readonly Dictionary<IVisualElement, Size> _lastMeasurements;
        private ISize _contextBounds;
    }
}