using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Android.Util;
using Android.Views;
using Das.Container;
using Das.Serializer;
using Das.Views;
using Das.Views.Construction;
using Das.Views.Core;
using Das.Views.Core.Geometry;
using Das.Views.Layout;
using Das.Views.Rendering;
using Das.Views.Styles;
using Das.Xamarin.Android.Images;
using Das.Xamarin.Android.Mvvm;
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
                                IStyleContext styleContext,
                                DisplayMetrics displayMetrics,
                                IResolver container)
        : base(container, styleContext, Serializer.AttributeParser, 
            Serializer.TypeInferrer, Serializer.TypeManipulator, 
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
                                AndroidFontProvider fontProvider,
                                IWindowManager windowManager,
                                AndroidUiProvider uiProvider,
                                IStyleContext styleContext,
                                DisplayMetrics displayMetrics)
            : this(viewPerspective, viewState, fontProvider, windowManager, uiProvider,
                styleContext, displayMetrics, new BaseResolver(TimeSpan.FromSeconds(5)))
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
            : base(resolver, styleContext, visualBootstrapper, viewInflater,
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
                          IStyleContext styleContext,
                          IViewPerspective viewPerspective,
                          DisplayMetrics displayMetrics,
                          AndroidFontProvider fontProvider,
                          IViewState viewState,
                          IUiProvider uiProvider,
                          //IVisualLineage visualLineage,
                          ref AndroidMeasureKit measureContext,
                          ref AndroidRenderContext renderContext,
                          ref RefreshRenderContext refreshRenderContext)
        {
            var imageProvider = new AndroidImageProvider(displayMetrics);
           // var lastMeasure = new Dictionary<IVisualElement, ValueSize>();

           var visualLineage = new VisualLineage();
            
            var lastMeasures = new Dictionary<IVisualElement, ValueSize>();

            measureContext = new AndroidMeasureKit(windowManager, fontProvider, 
                this, lastMeasures,styleContext, displayMetrics, visualLineage);

            var visualPositions = new Dictionary<IVisualElement, ValueCube>();

            

            renderContext = new AndroidRenderContext(viewPerspective,
                fontProvider, viewState, this, visualPositions, displayMetrics,
                lastMeasures, styleContext, visualLineage);
            
            refreshRenderContext = new RefreshRenderContext(viewPerspective, this, visualPositions,
                lastMeasures, styleContext, visualLineage);

            Container.ResolveTo<IImageProvider>(imageProvider);
            Container.ResolveTo(uiProvider);
            Container.ResolveTo(styleContext);

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