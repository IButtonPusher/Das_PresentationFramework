using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Views;
using Das.Views;
using Das.Views.Core;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Rendering;
using Das.Views.Styles;
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
                                IStyleContext styleContext)
        {
            MeasureContext = new AndroidMeasureKit(windowManager, fontProvider, this);

            var visualPositions = new Dictionary<IVisualElement, ICube>();

            RenderContext = new AndroidRenderContext(viewPerspective,
                fontProvider, viewState, this, visualPositions);

            RefreshRenderContext = new RefreshRenderContext(viewPerspective, this, visualPositions);

            Resolver.ResolveTo<IImageProvider>(RenderContext);
            Resolver.ResolveTo<IUiProvider>(uiProvider);
            Resolver.ResolveTo(styleContext);
            
        }

        IMeasureContext IRenderKit.MeasureContext => MeasureContext;

        IRenderContext IRenderKit.RenderContext => RenderContext;

        public AndroidMeasureKit MeasureContext { get; }

        public RefreshRenderContext RefreshRenderContext { get; }

        public AndroidRenderContext RenderContext { get; }
    }
}