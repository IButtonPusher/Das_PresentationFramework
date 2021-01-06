using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Android.Util;
using Android.Views;
using Das.Container;
using Das.Serializer;
using Das.Views;
using Das.Views.Colors;
using Das.Views.Construction;
using Das.Views.Core;
using Das.Views.Core.Geometry;
using Das.Views.Layout;
using Das.Views.Rendering;
using Das.Views.Styles;
using Das.Xamarin.Android.Images;
using Das.Xamarin.Android.Rendering;
// ReSharper disable UnusedMember.Global

namespace Das.Xamarin.Android
{
    public class AndroidRenderKit : BaseRenderKit,
                                    IRenderKit
    {
        public AndroidRenderKit(IViewPerspective viewPerspective,
                                IViewState viewState,
                                AndroidFontProvider fontProvider,
                                IWindowManager windowManager,
                                IUiProvider uiProvider,
                                IThemeProvider themeProvider,
                                DisplayMetrics displayMetrics,
                                IResolver container)
        : base(container, themeProvider, Serializer.AttributeParser, 
            Serializer.TypeInferrer, Serializer.TypeManipulator, 
            new Dictionary<IVisualElement, ValueCube>())
        {
            ViewState = viewState;
            DisplayMetrics = displayMetrics;
            
            Init(windowManager, themeProvider, viewPerspective, displayMetrics, 
                fontProvider, viewState, uiProvider, 
                ref _measureContext!, ref _renderContext!, ref _refreshRenderContext!);
        }

        public AndroidRenderKit(IViewPerspective viewPerspective,
                                IViewState viewState,
                                AndroidFontProvider fontProvider,
                                IWindowManager windowManager,
                                IUiProvider uiProvider,
                                IThemeProvider themeProvider,
                                DisplayMetrics displayMetrics)
            : this(viewPerspective, viewState, fontProvider, windowManager, uiProvider,
                themeProvider, displayMetrics, new BaseResolver(TimeSpan.FromSeconds(5)))
        {
            
        }

        public AndroidRenderKit(IViewPerspective viewPerspective,
                                IViewState viewState,
                                IStyleContext styleContext,
                                AndroidFontProvider fontProvider,
                                IWindowManager windowManager,
                                IUiProvider uiProvider,
                                DisplayMetrics displayMetrics,
                                IStringPrimitiveScanner attributeScanner,
                                ITypeInferrer typeInferrer,
                                IPropertyProvider propertyProvider)
            : base(styleContext, attributeScanner, typeInferrer, propertyProvider,
                new Dictionary<IVisualElement, ValueCube>())
        {
            ViewState = viewState;
            DisplayMetrics = displayMetrics;

            Init(windowManager, styleContext, viewPerspective, displayMetrics,
                fontProvider, viewState, uiProvider, 
                ref _measureContext!, ref _renderContext!, ref _refreshRenderContext!);
        }

        public AndroidRenderKit(IViewPerspective viewPerspective,
                                IViewState viewState,
                                IStyleContext styleContext,
                                AndroidFontProvider fontProvider,
                                IWindowManager windowManager,
                                IUiProvider uiProvider,
                                DisplayMetrics displayMetrics,
                                IResolver resolver,
                                IStringPrimitiveScanner attributeScanner,
                                ITypeInferrer typeInferrer, 
                                IPropertyProvider propertyProvider, 
                                IVisualBootstrapper visualBootstrapper) 
            : base(resolver, styleContext, attributeScanner, typeInferrer, propertyProvider, 
                visualBootstrapper,new Dictionary<IVisualElement, ValueCube>())
        {
            ViewState = viewState;
            DisplayMetrics = displayMetrics;

            Init(windowManager, styleContext, viewPerspective, displayMetrics,
                fontProvider, viewState, uiProvider, 
                ref _measureContext!, ref _renderContext!, ref _refreshRenderContext!);
        }

        public AndroidRenderKit(IViewPerspective viewPerspective,
                                IViewState viewState,
                                IWindowManager windowManager,
                                AndroidFontProvider fontProvider,
                                DisplayMetrics displayMetrics,
                                IUiProvider uiProvider,
                                IResolver resolver,
                                IStyleContext styleContext, 
                                IVisualBootstrapper visualBootstrapper, 
                                IViewInflater viewInflater) 
            : base(resolver, visualBootstrapper, viewInflater,
                new Dictionary<IVisualElement, ValueCube>())
        {
            ViewState = viewState;
            DisplayMetrics = displayMetrics;
            
            Init(windowManager, styleContext, viewPerspective, displayMetrics,
                fontProvider, viewState, uiProvider,
                ref _measureContext!, ref _renderContext!, ref _refreshRenderContext!);
        }
        
        [SuppressMessage("ReSharper", "RedundantAssignment")]
        private void Init(IWindowManager windowManager,
                          IThemeProvider themeProvider,
                          IViewPerspective viewPerspective,
                          DisplayMetrics displayMetrics,
                          AndroidFontProvider fontProvider,
                          IViewState viewState,
                          IUiProvider uiProvider,
                          ref AndroidMeasureKit measureContext,
                          ref AndroidRenderContext renderContext,
                          ref RefreshRenderContext refreshRenderContext)
        {
            var imageProvider = new AndroidImageProvider(displayMetrics);
           // var lastMeasure = new Dictionary<IVisualElement, ValueSize>();

           var visualLineage = new VisualLineage();
            
            var lastMeasures = new Dictionary<IVisualElement, ValueSize>();

            var layoutQueue = new LayoutQueue();

            measureContext = new AndroidMeasureKit(windowManager, fontProvider, 
                this, lastMeasures,themeProvider, displayMetrics, visualLineage, layoutQueue);

            var visualPositions = new Dictionary<IVisualElement, ValueCube>();

            

            renderContext = new AndroidRenderContext(viewPerspective,
                fontProvider, viewState, this, visualPositions,
                lastMeasures, themeProvider, visualLineage, layoutQueue);
            
            refreshRenderContext = new RefreshRenderContext(viewPerspective, this, 
                visualPositions, lastMeasures, themeProvider, visualLineage, layoutQueue);

            Container.ResolveTo<IImageProvider>(imageProvider);
            Container.ResolveTo(uiProvider);
            Container.ResolveTo(themeProvider);

        }

        protected static readonly DasSerializer Serializer = new DasSerializer();

        IMeasureContext IRenderKit.MeasureContext => _measureContext;

        IRenderContext IRenderKit.RenderContext => _renderContext;

        public IViewState ViewState { get; }

        

        public DisplayMetrics DisplayMetrics { get; }

        public AndroidMeasureKit MeasureContext => _measureContext;

        public RefreshRenderContext RefreshRenderContext => _refreshRenderContext;

        public AndroidRenderContext RenderContext => _renderContext;

        private readonly AndroidMeasureKit _measureContext;
        private readonly AndroidRenderContext _renderContext;
        private readonly RefreshRenderContext _refreshRenderContext;
    }
}