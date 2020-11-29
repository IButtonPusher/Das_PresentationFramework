using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Util;
using Android.Views;
using Das.Extensions;
using Das.Views.Controls;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Measuring;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Xamarin.Android
{
    public class AndroidMeasureKit : BaseMeasureContext
    {
        public AndroidMeasureKit(IWindowManager windowManager,
                                 IFontProvider<AndroidFontPaint> fontProvider,
                                 IVisualSurrogateProvider surrogateProvider,
                                 Dictionary<IVisualElement, ValueSize> lastMeasurements,
                                 IStyleContext styleContext)
        : base(surrogateProvider, lastMeasurements, styleContext)
        {
            _windowManager = windowManager;
            _fontProvider = fontProvider;
            _contextBounds = GetCOntextBounds();
        }

        public override ISize ContextBounds
        {
            get
            {
                return _contextBounds;
            }
        }

        private ISize GetCOntextBounds()
        {
            if (!(_windowManager.DefaultDisplay is {} disp))
                throw new NullReferenceException();

            var metrics = new DisplayMetrics();
            disp.GetMetrics(metrics);

            return new ValueSize(metrics.WidthPixels / metrics.ScaledDensity,
                metrics.HeightPixels / metrics.ScaledDensity);
        }

        public override ValueSize MeasureString(String s, 
                                                IFont font)
        {
            var renderer = _fontProvider.GetRenderer(font);
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

        private ISize _contextBounds;
        private readonly IWindowManager _windowManager;
        private readonly IFontProvider<AndroidFontPaint> _fontProvider;
    }
}