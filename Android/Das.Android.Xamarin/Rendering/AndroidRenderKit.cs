using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Util;
using Android.Views;
using Das.Container;
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
                                DisplayMetrics displayMetrics,
                                IResolver container)
        : base(container, styleContext)
        {
            DisplayMetrics = displayMetrics;
            var lastMeasures = new Dictionary<IVisualElement, ValueSize>();
            MeasureContext = new AndroidMeasureKit(windowManager, fontProvider, 
                this, lastMeasures,styleContext);

            var visualPositions = new Dictionary<IVisualElement, ValueCube>();

            var imageProvider = new AndroidImageProvider(displayMetrics);

            RenderContext = new AndroidRenderContext(viewPerspective,
                fontProvider, viewState, this, visualPositions, displayMetrics,
                lastMeasures, styleContext);

            RefreshRenderContext = new RefreshRenderContext(viewPerspective, this, visualPositions,
                lastMeasures, styleContext);

            Container.ResolveTo<IImageProvider>(imageProvider);
            Container.ResolveTo<IUiProvider>(uiProvider);
            Container.ResolveTo(styleContext);
        }

        public AndroidRenderKit(IViewPerspective viewPerspective,
                                IViewState viewState,
                                IFontProvider<AndroidFontPaint> fontProvider,
                                IWindowManager windowManager,
                                AndroidUiProvider uiProvider,
                                IStyleContext styleContext,
                                DisplayMetrics displayMetrics)
            : this(viewPerspective, viewState, fontProvider, windowManager, uiProvider,
                styleContext, displayMetrics, new BaseResolver(TimeSpan.FromSeconds(5)))
        {
            
        }

        IMeasureContext IRenderKit.MeasureContext => MeasureContext;

        IRenderContext IRenderKit.RenderContext => RenderContext;

        public DisplayMetrics DisplayMetrics { get; }

        public AndroidMeasureKit MeasureContext { get; }

        public RefreshRenderContext RefreshRenderContext { get; }

        public AndroidRenderContext RenderContext { get; }
    }
}