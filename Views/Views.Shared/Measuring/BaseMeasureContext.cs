using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views.Measuring
{
    public abstract class BaseMeasureContext : ContextBase,
                                               IMeasureContext
    {
        protected BaseMeasureContext(IVisualSurrogateProvider surrogateProvider,
                                     Dictionary<IVisualElement, ValueSize> lastMeasurements,
                                     IStyleContext styleContext)
            : base(lastMeasurements, styleContext, surrogateProvider)
        {
            _surrogateProvider = surrogateProvider;
            _contextBounds = ValueSize.Empty;
            _lastMeasurements = lastMeasurements;
            _styleContext = styleContext;
        }

        public virtual ValueSize MeasureImage(IImage img)
        {
            return new ValueSize(img.Width, img.Height);
        }

        public ValueSize MeasureMainView(IVisualElement element,
                                         IRenderSize availableSpace,
                                         IViewState viewState)
        {
            //Debug.WriteLine("********** BEGIN MEASURE ***********");

            ViewState = viewState;

            if (viewState.ZoomLevel.AreDifferent(1.0))
            {
                var zoomWidth = availableSpace.Width / viewState.ZoomLevel;
                var zoomHeight = availableSpace.Height / viewState.ZoomLevel;

                if (_contextBounds.Width.AreDifferent(zoomWidth) ||
                    _contextBounds.Height.AreDifferent(zoomHeight))
                    _contextBounds = new ValueSize(zoomWidth, zoomHeight);

                availableSpace = new RenderSize(zoomWidth, zoomHeight, availableSpace.Offset);
            }
            else if (_contextBounds.Width.AreDifferent(availableSpace.Width) ||
                     _contextBounds.Height.AreDifferent(availableSpace.Height))
                _contextBounds = new ValueSize(availableSpace);

            var res = MeasureElement(element, availableSpace);
            //Debug.WriteLine("********** END MEASURE ***********");
            return res;
        }


        public virtual ValueSize MeasureElement(IVisualElement element,
                                                IRenderSize availableSpace)
        {
            //_surrogateProvider.EnsureSurrogate(ref element);

            if (!element.IsRequiresMeasure)
            {
                lock (_measureLock)
                {
                    if (_lastMeasurements.TryGetValue(element, out var val))
                        return val;
                }

                return ValueSize.Empty;
            }

            _styleContext.PushVisual(element);

            //if (element.Template is {Content: { } validTemplateContent})
            //    element = validTemplateContent;

            var layoutElement = GetElementForLayout(element);

            var res = MeasureElementImpl(layoutElement, availableSpace);

            element.AcceptChanges(ChangeType.Measure);
            return res;

            ////System.Diagnostics.Debug.WriteLine("measuring " + element);

            //var viewState = GetViewState;


            //var margin = element.Margin ?? viewState.GetStyleSetter<Thickness>(StyleSetter.Margin, element);

            //var border = viewState.GetStyleSetter<Thickness>(StyleSetter.BorderThickness, element);


            //ValueSize desiredSize;

            //if (margin.IsEmpty && border.IsEmpty)
            //{
            //    desiredSize = element.Measure(availableSpace, this);
            //    //System.Diagnostics.Debug.WriteLine(element + " wants " + desiredSize);
            //}
            //else
            //{
            //    desiredSize = element.Measure(
            //        new ValueRenderSize(availableSpace.Width - (margin.Width - border.Width),
            //            availableSpace.Height - (margin.Height + border.Height)), this);

            //    desiredSize = new ValueSize(desiredSize.Width + margin.Width + border.Width,
            //        desiredSize.Height + margin.Height + border.Height);
            //}

            //SetLastMeasured(element, desiredSize);

            //_styleContext.PopVisual();

            //element.AcceptChanges(ChangeType.Measure);

            //return desiredSize;
        }


        public abstract ValueSize MeasureString(String s,
                                                IFont font);

        public ValueSize GetStyleDesiredSize(IVisualElement element)
        {
            var viewState = GetViewState;
            var zoom = viewState.ZoomLevel;

            //var specificSize = viewState.GetStyleSetter<Size>(StyleSetter.Size, element)
            //                   * zoom;

            var specificHeight = viewState.GetStyleSetter<Double>(StyleSetterType.Height, element)
                                 * zoom;
            var specificWidth = viewState.GetStyleSetter<Double>(StyleSetterType.Width, element)
                                * zoom;

            return new ValueSize(Double.IsNaN(specificWidth) ? 0 : specificWidth,
                Double.IsNaN(specificHeight) ? 0 : specificHeight);
        }

        public virtual ValueSize ContextBounds => _contextBounds;

        protected virtual void OnElementDisposed(IVisualElement element)
        {
            lock (_measureLock)
            {
                _lastMeasurements.Remove(element);
            }
        }

        protected virtual Boolean TryGetElementSpecifiedSize(IVisualElement visual,
                                                             out ValueSize size)
        {
            var width = visual.Width ?? GetStyleSetter<Double>(StyleSetterType.Width, visual);

            if (Double.IsNaN(width))
            {
                size = ValueSize.Empty;
                return false;
            }

            var height = visual.Height ?? GetStyleSetter<Double>(StyleSetterType.Height, visual);

            if (Double.IsNaN(height))
            {
                size = ValueSize.Empty;
                return false;
            }

            size = new ValueSize(width, height);
            return true;
        }

        private ValueSize MeasureElementImpl(IVisualElement element,
                                             IRenderSize availableSpace)
        {
            //System.Diagnostics.Debug.WriteLine("measuring " + element);

            var viewState = GetViewState;


            var margin = element.Margin ?? viewState.GetStyleSetter<Thickness>(StyleSetterType.Margin, element);

            var border = viewState.GetStyleSetter<Thickness>(StyleSetterType.BorderThickness, element);


            ValueSize desiredSize;

            if (margin.IsEmpty && border.IsEmpty)
                desiredSize = element.Measure(availableSpace, this);
            //System.Diagnostics.Debug.WriteLine(element + " wants " + desiredSize);
            else
            {
                desiredSize = element.Measure(
                    new ValueRenderSize(availableSpace.Width - (margin.Width - border.Width),
                        availableSpace.Height - (margin.Height + border.Height)), this);

                desiredSize = new ValueSize(desiredSize.Width + margin.Width + border.Width,
                    desiredSize.Height + margin.Height + border.Height);
            }

            SetLastMeasured(element, desiredSize);

            _styleContext.PopVisual();


            return desiredSize;
        }

        private void SetLastMeasured(IVisualElement element,
                                     ValueSize size)
        {
            //   Debug.WriteLine("visual " + element + " measured: " + size);

            lock (_measureLock)
            {
                if (!_lastMeasurements.ContainsKey(element))
                {
                    element.Disposed += OnElementDisposed;
                    _lastMeasurements.Add(element, size);
                }
                else
                    _lastMeasurements[element] = size;
            }
        }


        private readonly Dictionary<IVisualElement, ValueSize> _lastMeasurements;
        private readonly IStyleContext _styleContext;
        private readonly IVisualSurrogateProvider _surrogateProvider;

        private ValueSize _contextBounds;
    }
}