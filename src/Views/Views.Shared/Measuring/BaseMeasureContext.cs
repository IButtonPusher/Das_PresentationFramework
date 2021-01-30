using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Colors;
using Das.Views.Controls;
using Das.Views.Core;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Rendering;

namespace Das.Views.Measuring
{
    public abstract class BaseMeasureContext : ContextBase,
                                               IMeasureContext
    {
        protected BaseMeasureContext(IVisualSurrogateProvider surrogateProvider,
                                     Dictionary<IVisualElement, ValueSize> lastMeasurements,
                                     IThemeProvider themeProvider,
                                     //IStyleContext styleContext,
                                     IVisualLineage visualLineage,
                                     ILayoutQueue layoutQueue)
            : base(lastMeasurements, themeProvider,
                surrogateProvider, visualLineage, layoutQueue)
        {
            _contextBounds = ValueSize.Empty;
            _lastMeasurements = lastMeasurements;
        }

        public virtual ValueSize MeasureImage(IImage img)
        {
            return new ValueSize(img.Width, img.Height);
        }

        public ValueSize MeasureMainView<TRenderSize>(IVisualElement element,
                                         TRenderSize availableSpace,
                                         IViewState viewState)
            where TRenderSize : IRenderSize
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

                var availableSpace2 = new ValueRenderSize(zoomWidth, zoomHeight, availableSpace.Offset);
                
                //Debug.WriteLine("********** END MEASURE ***********");
                return MeasureElement(element, availableSpace2);
            }

            if (_contextBounds.Width.AreDifferent(availableSpace.Width) ||
                _contextBounds.Height.AreDifferent(availableSpace.Height))
            {
                _contextBounds = new ValueSize(availableSpace);
            }

            var res = MeasureElement(element, availableSpace);
            //Debug.WriteLine("********** END MEASURE ***********");
            return res;
        }

        public ValueSize MeasureRenderer<TRenderSize>(IVisualRenderer visualRenderer, 
                                                      TRenderSize availableSpace) 
            where TRenderSize : IRenderSize
        {
            throw new NotImplementedException();
        }


        public virtual ValueSize MeasureElement<TRenderSize>(IVisualElement element,
                                                             TRenderSize availableSpace)
        where TRenderSize : IRenderSize
        {
            if (!element.IsRequiresMeasure)
            {
                lock (_measureLock)
                {
                    if (_lastMeasurements.TryGetValue(element, out var val))
                    {
                        LayoutQueue.RemoveVisualFromMeasureQueue(element);
                        return val;
                    }
                }

                // measure anyways if we know nothing of it...?
                //return ValueSize.Empty;
            }

            VisualLineage.PushVisual(element);
            //_styleContext.PushVisual(element);

            var layoutElement = GetElementForLayout(element);

            ////////////////////////
            var res = MeasureElementImpl(layoutElement, availableSpace);
            ////////////////////////
            
            LayoutQueue.RemoveVisualFromMeasureQueue(element);
            element.AcceptChanges(ChangeType.Measure);
            return res;
        }


        public abstract ValueSize MeasureString(String s,
                                                IFont font);

        //public ValueSize GetStyleDesiredSize(IVisualElement element)
        //{
        //    //var viewState = GetViewState;
        //    var styles = ViewState.StyleContext;
        //    //var zoom = ViewState.ZoomLevel;

        //    var specificHeight = styles.GetStyleSetter<Double>(StyleSetterType.Height, element,
        //        VisualLineage);

        //    var specificWidth = styles.GetStyleSetter<Double>(StyleSetterType.Width, element,
        //        VisualLineage);


        //    return new ValueSize(Double.IsNaN(specificWidth) ? 0 : specificWidth,
        //        Double.IsNaN(specificHeight) ? 0 : specificHeight);
        //}

        public virtual ValueSize ContextBounds => _contextBounds;

        protected virtual void OnElementDisposed(IVisualElement element)
        {
            lock (_measureLock)
            {
                _lastMeasurements.Remove(element);
            }
        }



        private ValueSize MeasureElementImpl<TRenderSize>(IVisualElement element,
                                                          TRenderSize availableSpace)
            where TRenderSize : IRenderSize
        {
            if (element.Visibility == Visibility.Collapsed)
                return ValueSize.Empty;

            var margin = element.Margin.GetValue(availableSpace);
            var border = element.Border;
            var borderThickness = border.GetThickness(availableSpace);

            ValueSize desiredSize;
            Double extraWidth;
            Double extraHeight;

            if (margin.IsEmpty && border.IsEmpty)
            {
                desiredSize = element.Measure(availableSpace, this);

                extraWidth = 0;
                extraHeight = 0;
            }

            else
            {
                extraWidth = margin.Width + borderThickness.Width;
                extraHeight = margin.Height + borderThickness.Height;

                desiredSize = element.Measure(
                    new ValueRenderSize(availableSpace.Width - extraWidth,
                        availableSpace.Height - extraHeight), this);
            }

            var h = desiredSize.Height;
            switch (element.Height?.Units ?? LengthUnits.Invalid)
            {
                case LengthUnits.Invalid:
                case LengthUnits.Percent:
                    h = desiredSize.Height + extraHeight;
                    break;

                case LengthUnits.Px:
                    h = element.Height!.Value.GetQuantity(availableSpace.Height) + extraHeight;
                    break;
            }


            var w = desiredSize.Width;
            switch (element.Width?.Units ?? LengthUnits.Invalid)
            {
                case LengthUnits.Invalid:
                case LengthUnits.Percent:
                    w = desiredSize.Width + margin.Width + borderThickness.Width;
                    break;

                case LengthUnits.Px:
                    w = element.Width!.Value.GetQuantity(availableSpace.Width) + extraWidth;
                    break;
            }


            desiredSize = new ValueSize(w, h);

            var useHeight = desiredSize.Height;
            var useWidth = desiredSize.Width;


            if (element.BeforeLabel is { } beforeLabel)
            {
                VisualLineage.PushVisual(beforeLabel);
                var beforeLabelMeasured = MeasureElementImpl(beforeLabel, availableSpace);
                useHeight = Math.Max(useHeight, beforeLabelMeasured.Height);
                useWidth += beforeLabelMeasured.Width;
            }

            if (element.AfterLabel is { } afterLabel)
            {
                VisualLineage.PushVisual(afterLabel);
                var afterLabelMeasured = MeasureElementImpl(afterLabel, availableSpace);
                useHeight = Math.Max(useHeight, afterLabelMeasured.Height);
                useWidth += afterLabelMeasured.Width;
            }

            desiredSize = new ValueSize(useWidth, useHeight);

            SetLastMeasured(element, desiredSize);

            VisualLineage.PopVisual();

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

        private ValueSize _contextBounds;
    }
}