using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views
{
    public abstract class ContextBase : IVisualContext
    {
        private readonly Dictionary<IVisualElement, ValueSize> _lastMeasurements;
        private readonly IStyleContext _styleContext;
        private readonly IVisualSurrogateProvider _surrogateProvider;
        protected readonly Object _measureLock;
        

        public ContextBase(Dictionary<IVisualElement, ValueSize> lastMeasurements,
                           IStyleContext styleContext,
                           IVisualSurrogateProvider surrogateProvider)
        {
            _measureLock = new Object();
            _lastMeasurements = lastMeasurements;
            _styleContext = styleContext;
            _surrogateProvider = surrogateProvider;
        }

        public IViewState? ViewState { get; protected set; }

        public T GetStyleSetter<T>(StyleSetterType setterType, 
                                   IVisualElement element)
        {
            return GetViewState.GetStyleSetter<T>(setterType, element);
        }

        public T GetStyleSetter<T>(StyleSetterType setterType, 
                                   StyleSelector selector, 
                                   IVisualElement element)
        {
            return GetViewState.GetStyleSetter<T>(setterType, selector, element);
        }

        public void RegisterStyleSetter(IVisualElement element, 
                                        StyleSetterType setterType,
                                        Object value)
        {
            GetViewState.RegisterStyleSetter(element, setterType, value);
        }

        public void RegisterStyleSetter(IVisualElement element, 
                                        StyleSetterType setterType, 
                                        StyleSelector selector, 
                                        Object value)
        {
            GetViewState.RegisterStyleSetter(element, setterType, selector, value);
        }
        
        protected IVisualElement GetElementForLayout(IVisualElement element)
        {
            _surrogateProvider.EnsureSurrogate(ref element);
            
            if (element.Template is {Content: { } validTemplateContent})
                return validTemplateContent;

            return element;
        }

        public IColorPalette ColorPalette => GetViewState.ColorPalette;


        public ValueSize GetLastMeasure(IVisualElement element)
        {
            lock (_measureLock)
                return _lastMeasurements.TryGetValue(element, out var val) ? val : ValueSize.Empty;
        }

        public IVisualLineage VisualLineage { get; }

        public Double ZoomLevel => ViewState?.ZoomLevel ?? 1;
        

        protected IViewState GetViewState =>
            ViewState ?? throw new Exception("No view state has been set.  Call MeasureMainView");
    }
}