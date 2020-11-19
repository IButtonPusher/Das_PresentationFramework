using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly IVisualSurrogateProvider _surrogateProvider;

        protected BaseMeasureContext(IVisualSurrogateProvider surrogateProvider,
                                     Dictionary<IVisualElement, ValueSize> lastMeasurements)
        : base(lastMeasurements)
        {
            _surrogateProvider = surrogateProvider;
            _contextBounds = Size.Empty;
            _lastMeasurements = lastMeasurements;
        }

        public virtual ValueSize MeasureImage(IImage img)
        {
            return new ValueSize(img.Width, img.Height);
        }

        public ValueSize MeasureMainView(IVisualElement element,
                                    IRenderSize availableSpace,
                                    IViewState viewState)
        {
            ViewState = viewState;
            _contextBounds = availableSpace;
            return MeasureElement(element, availableSpace);
        }

        public ValueSize MeasureElement(IVisualElement element,
                                        IRenderSize availableSpace)
        {
            _surrogateProvider.EnsureSurrogate(ref element);

            var viewState = GetViewState;
            var zoom = viewState.ZoomLevel;

            var margin = viewState.GetStyleSetter<Thickness>(StyleSetter.Margin, element)
                         * zoom;
            var border = viewState.GetStyleSetter<Thickness>(StyleSetter.BorderThickness, element)
                         * zoom;

            ValueSize desiredSize;

            if (margin.IsEmpty && border.IsEmpty)
            {
                desiredSize = element.Measure(availableSpace, this);
            }
            else
            {
                //var specificSize = viewState.GetStyleSetter<Size>(StyleSetter.Size, element)
                //* zoom;
                desiredSize = element.Measure(
                    new ValueRenderSize(availableSpace.Width - (margin.Width - border.Width),
                        availableSpace.Height - (margin.Height + border.Height)), this);

                desiredSize = new ValueSize(desiredSize.Width + margin.Width + border.Width,
                    desiredSize.Height + margin.Height + border.Height);
            }

            SetLastMeasured(element, desiredSize);

            element.AcceptChanges(ChangeType.Measure);

            return desiredSize;

            //var specificHeight = viewState.GetStyleSetter<Double>(StyleSetter.Height, element)
            //                     * zoom;
            //var specificWidth = viewState.GetStyleSetter<Double>(StyleSetter.Width, element)
            //                    * zoom;

            //var size = specificSize ?? desiredSize;
            //var size = new Size(Double.IsNaN(specificWidth) ? desiredSize.Width : specificWidth,
            //    Double.IsNaN(specificHeight) ? desiredSize.Height : specificHeight);

            //var total = Size.Add(size, margin, border);

            //lock (_measureLock)
            //{
            //    if (!_lastMeasurements.ContainsKey(element))
            //    {
            //        element.Disposed += OnElementDisposed;
            //        _lastMeasurements.Add(element, total);
            //    }
            //    else
            //        _lastMeasurements[element] = total;
            //}

            //return total;
        }

        private void SetLastMeasured(IVisualElement element,
                                     ValueSize size)
        {
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

        private void OnElementDisposed(IVisualElement element)
        {
            lock (_measureLock)
                _lastMeasurements.Remove(element);

        }

       

        public abstract ValueSize MeasureString(String s, 
                                                IFont font);

        public ValueSize GetStyleDesiredSize(IVisualElement element)
        {
            var viewState = GetViewState;
            var zoom = viewState.ZoomLevel;

            //var specificSize = viewState.GetStyleSetter<Size>(StyleSetter.Size, element)
            //                   * zoom;

            var specificHeight = viewState.GetStyleSetter<Double>(StyleSetter.Height, element)
                                 * zoom;
            var specificWidth = viewState.GetStyleSetter<Double>(StyleSetter.Width, element)
                                * zoom;

            return new ValueSize(Double.IsNaN(specificWidth) ? 0 : specificWidth,
                Double.IsNaN(specificHeight) ? 0 : specificHeight);
        }

        public virtual ISize ContextBounds => _contextBounds;

        
        private readonly Dictionary<IVisualElement, ValueSize> _lastMeasurements;
        
        private ISize _contextBounds;
    }
}