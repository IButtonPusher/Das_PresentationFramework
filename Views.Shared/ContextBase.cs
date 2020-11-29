using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        protected readonly Object _measureLock;
        

        public ContextBase(Dictionary<IVisualElement, ValueSize> lastMeasurements,
                           IStyleContext styleContext)
        {
            _measureLock = new Object();
            _lastMeasurements = lastMeasurements;
            _styleContext = styleContext;
        }

        public IViewState? ViewState { get; protected set; }

        public T GetStyleSetter<T>(StyleSetter setter, 
                                   IVisualElement element)
        {
            return GetViewState.GetStyleSetter<T>(setter, element);
        }

        public T GetStyleSetter<T>(StyleSetter setter, 
                                   StyleSelector selector, 
                                   IVisualElement element)
        {
            return GetViewState.GetStyleSetter<T>(setter, selector, element);
        }

        public void RegisterStyleSetter(IVisualElement element, 
                                        StyleSetter setter,
                                        Object value)
        {
            GetViewState.RegisterStyleSetter(element, setter, value);
        }

        public void RegisterStyleSetter(IVisualElement element, 
                                        StyleSetter setter, 
                                        StyleSelector selector, 
                                        Object value)
        {
            GetViewState.RegisterStyleSetter(element, setter, selector, value);
        }

        public IColorPalette ColorPalette => GetViewState.ColorPalette;

        public IColor GetCurrentAccentColor()
        {
            return GetViewState.GetCurrentAccentColor();
        }


        public ValueSize GetLastMeasure(IVisualElement element)
        {
            lock (_measureLock)
                return _lastMeasurements.TryGetValue(element, out var val) ? val : ValueSize.Empty;
        }

        public Double ZoomLevel => ViewState?.ZoomLevel ?? 1;
        

        protected IViewState GetViewState =>
            ViewState ??
            throw new Exception(
                "No view state has been set.  Call MeasureMainView");
    }
}