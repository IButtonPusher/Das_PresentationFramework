using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Util;
using Android.Views;
using Das.Extensions;
using Das.Views;
using Das.Views.Colors;
using Das.Views.Controls;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Measuring;
using Das.Views.Rendering;

namespace Das.Xamarin.Android
{
    public class AndroidMeasureKit : BaseMeasureContext
    {
        public AndroidMeasureKit(IWindowManager windowManager,
                                 AndroidFontProvider fontProvider,
                                 IVisualSurrogateProvider surrogateProvider,
                                 Dictionary<IVisualElement, ValueSize> lastMeasurements,
                                 IThemeProvider themeProvider,
                                 DisplayMetrics displayMetrics,
                                 IVisualLineage visualLineage,
                                 ILayoutQueue layoutQueue)
        : base(surrogateProvider, lastMeasurements, 
            themeProvider, visualLineage,layoutQueue)
        {
            _windowManager = windowManager;
            _fontProvider = fontProvider;
            _contextBounds = GetCOntextBounds(displayMetrics);
        }

        public override ValueSize ContextBounds
        {
            get
            {
                return _contextBounds;
            }
        }

        private ValueSize GetCOntextBounds(DisplayMetrics metrics)
        {
            if (!(_windowManager.DefaultDisplay is {}))
                throw new NullReferenceException();


            return new ValueSize(metrics.WidthPixels / metrics.ScaledDensity,
                metrics.HeightPixels / metrics.ScaledDensity);
        }

        public sealed override ValueSize MeasureElement<TRenderSize>(IVisualElement element,
                                                                     TRenderSize availableSpace)
        {
            _fontProvider.RemoveElement(element);
            return base.MeasureElement(element, availableSpace);
        }

        public sealed override ValueSize MeasureString(String s, 
                                                       IFont font)
        {
            var renderer = _fontProvider.GetRenderer(font, VisualLineage);
            var res = renderer.MeasureString(s);

            if (ZoomLevel.AreDifferent(1.0))
            {
                // android gives a nice dpi adjusted value here but that goes 
                // against dpi agnostic ambitions

                return new ValueSize(res.Width / ZoomLevel,
                    res.Height / ZoomLevel);
            }

            return res;

        }

        protected sealed override void OnElementDisposed(IVisualElement element)
        {
            base.OnElementDisposed(element);
            _fontProvider.RemoveElement(element);
        }

        private readonly ValueSize _contextBounds;
        private readonly IWindowManager _windowManager;
        private readonly AndroidFontProvider _fontProvider;
    }
}