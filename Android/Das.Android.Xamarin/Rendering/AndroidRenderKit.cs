using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Util;
using Android.Views;
using Das.Views;
using Das.Views.Core;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Rendering;
using Das.Views.Styles;
using Das.Xamarin.Android.Images;
using Das.Xamarin.Android.Mvvm;
using Das.Xamarin.Android.Rendering;

namespace Das.Xamarin.Android
{
    public class AndroidRenderKit : BaseRenderKit,
                                    IRenderKit
    {
        public AndroidRenderKit(IViewPerspective viewPerspective,
                                IViewState viewState,
                                IFontProvider<AndroidFontPaint> fontProvider,
                                IWindowManager windowManager,
                                AndroidUiProvider uiProvider,
                                IStyleContext styleContext,
                                DisplayMetrics displayMetrics)
        : base(styleContext)
        {
            var lastMeasures = new Dictionary<IVisualElement, ValueSize>();
            MeasureContext = new AndroidMeasureKit(windowManager, fontProvider, this, lastMeasures);

            var visualPositions = new Dictionary<IVisualElement, ICube>();

            var imageProvider = new AndroidImageProvider(displayMetrics);

            RenderContext = new AndroidRenderContext(viewPerspective,
                fontProvider, viewState, this, visualPositions, displayMetrics, lastMeasures);

            RefreshRenderContext = new RefreshRenderContext(viewPerspective, this, visualPositions,
                lastMeasures);

            Container.ResolveTo<IImageProvider>(imageProvider);
            Container.ResolveTo<IUiProvider>(uiProvider);
            Container.ResolveTo(styleContext);
            
        }

        IMeasureContext IRenderKit.MeasureContext => MeasureContext;

        IRenderContext IRenderKit.RenderContext => RenderContext;

        public AndroidMeasureKit MeasureContext { get; }

        public RefreshRenderContext RefreshRenderContext { get; }

        public AndroidRenderContext RenderContext { get; }
    }
}