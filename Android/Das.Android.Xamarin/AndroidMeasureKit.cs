using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Util;
using Android.Views;
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
                                 IFontProvider<AndroidFontPaint> fontProvider,
                                 IVisualSurrogateProvider surrogateProvider,
                                 Dictionary<IVisualElement, ValueSize> lastMeasurements)
        : base(surrogateProvider, lastMeasurements)
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

            return new ValueSize(metrics.WidthPixels, metrics.HeightPixels);
        }

        public override ValueSize MeasureString(String s, 
                                                IFont font)
        {
            var renderer = _fontProvider.GetRenderer(font);
            return renderer.MeasureString(s);
        }

        private ISize _contextBounds;
        private readonly IWindowManager _windowManager;
        private readonly IFontProvider<AndroidFontPaint> _fontProvider;
    }
}