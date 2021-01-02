using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Layout;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views
{
    public abstract class ContextBase : IVisualContext
    {
        protected ContextBase(Dictionary<IVisualElement, ValueSize> lastMeasurements,
                              IStyleContext styleContext,
                              IVisualSurrogateProvider surrogateProvider,
                              IVisualLineage visualLineage)
        {
            _measureLock = new Object();
            _lastMeasurements = lastMeasurements;
            _styleContext = styleContext;
            _surrogateProvider = surrogateProvider;
            VisualLineage = visualLineage;
            ViewState = NullViewState.Instance;
            
        }

        public T GetStyleSetter<T>(StyleSetterType setterType,
                                   IVisualElement element)
        {
            return ViewState.StyleContext.GetStyleSetter<T>(setterType, element, VisualLineage);
        }

        public T GetStyleSetter<T>(StyleSetterType setterType,
                                   VisualStateType type,
                                   IVisualElement element)
        {
            return ViewState.StyleContext.GetStyleSetter<T>(setterType, type, 
                element, VisualLineage);
        }

        //public void RegisterStyleSetter(IVisualElement element,
        //                                StyleSetterType setterType,
        //                                Object value)
        //{
        //    GetViewState.RegisterStyleSetter(element, setterType, value);
        //}

        //public void RegisterStyleSetter(IVisualElement element,
        //                                StyleSetterType setterType,
        //                                StyleSelector selector,
        //                                Object value)
        //{
        //    GetViewState.RegisterStyleSetter(element, setterType, selector, value);
        //}

        //public IColorPalette ColorPalette => GetViewState.ColorPalette;


        public ValueSize GetLastMeasure(IVisualElement element)
        {
            lock (_measureLock)
            {
                return _lastMeasurements.TryGetValue(element, out var val) ? val : ValueSize.Empty;
            }
        }

        public IVisualLineage VisualLineage { get; }

        public Double ZoomLevel => ViewState?.ZoomLevel ?? 1;

        public IViewState ViewState { get; protected set; }


        public virtual Boolean TryGetElementSize(IVisualElement visual,
                                                 out ValueSize size)
        {
            var width = visual.Width ?? GetStyleSetter<Double>(StyleSetterType.Width, visual);

            if (Double.IsNaN(width))
                goto fail;

            var height = visual.Height ?? GetStyleSetter<Double>(StyleSetterType.Height, visual);

            if (Double.IsNaN(height))
                goto fail;

            size = new ValueSize(width, height);
            return true;
            
            fail:
            size = ValueSize.Empty;
            return false;
        }

        protected IVisualElement GetElementForLayout(IVisualElement element)
        {
            _surrogateProvider.TrySetSurrogate(ref element);

            if (element.Template is {Content: { } validTemplateContent})
                return validTemplateContent;

            return element;
        }

        private readonly Dictionary<IVisualElement, ValueSize> _lastMeasurements;
        protected readonly Object _measureLock;
        private readonly IStyleContext _styleContext;
        private readonly IVisualSurrogateProvider _surrogateProvider;

        public IColorPalette ColorPalette => _styleContext.ColorPalette;
    }
}