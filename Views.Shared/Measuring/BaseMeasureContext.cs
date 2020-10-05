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
    public abstract class BaseMeasureContext : IMeasureContext
    {
        public BaseMeasureContext()
        {
            _empty = new Size(0, 0);
            _contextBounds = Size.Empty;
            _lastMeasurements = new Dictionary<IVisualElement, Size>();
        }

        public IViewState? ViewState { get; private set; }

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
        {
            return _lastMeasurements.TryGetValue(element, out var val) ? val : _empty;
        }

        public abstract Size MeasureString(String s, Font font);

        public virtual ISize ContextBounds
        {
            get => _contextBounds;
        }

        private IViewState GetViewState() => ViewState ??
                                             throw new Exception(
                                                 "No view state has been set.  Call MeasureMainView");

        //public virtual void UpdateContextBounds(ISize value) => _contextBounds = value;

        public T GetStyleSetter<T>(StyleSetters setter, IVisualElement element)
        {
            return GetViewState().GetStyleSetter<T>(setter, element);
        }

        private readonly Size _empty;
        private readonly Dictionary<IVisualElement, Size> _lastMeasurements;
        private ISize _contextBounds;
    }
}