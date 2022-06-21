using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Colors;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Layout;
using Das.Views.Rendering;

namespace Das.Views
{
    public abstract class ContextBase : IVisualContext
    {
        protected ContextBase(Dictionary<IVisualElement, ValueSize> lastMeasurements,
                              IThemeProvider themeProvider,
                              IVisualSurrogateProvider surrogateProvider,
                              IVisualLineage visualLineage,
                              ILayoutQueue layoutQueue)
        {
            _measureLock = new Object();
            _lastMeasurements = lastMeasurements;
            _surrogateProvider = surrogateProvider;
            _themeProvider = themeProvider;
            VisualLineage = visualLineage;
            LayoutQueue = layoutQueue;
            ViewState = NullViewState.Instance;
            
        }

        public ValueSize GetLastMeasure(IVisualElement element)
        {
            lock (_measureLock)
            {
                return _lastMeasurements.TryGetValue(element, out var val) ? val : ValueSize.Empty;
            }
        }

        public IVisualLineage VisualLineage { get; }

        public ILayoutQueue LayoutQueue { get; }

        public Double ZoomLevel => ViewState?.ZoomLevel ?? 1;

        public IViewState ViewState { get; protected set; }


        public virtual Boolean TryGetElementSize(IVisualElement visual,
                                                 ISize availableSize,
                                                 out ValueSize size)
        {
            if (!(visual.Width is {} vWidth) || 
                !(visual.Height is {} vHeight))
                goto fail;

            var w = vWidth.GetQuantity(availableSize.Width);
            var h = vHeight.GetQuantity(availableSize.Height);

            size = new ValueSize(w, h);
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
        private readonly IVisualSurrogateProvider _surrogateProvider;
        protected readonly IThemeProvider _themeProvider;

        public IColorPalette ColorPalette => _themeProvider.ColorPalette;

        public Boolean IsDarkTheme => _themeProvider.IsDarkTheme;
    }
}