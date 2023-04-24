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
using Das.Views.Core;
using Das.Views.Core.Geometry;
using Das.Views.Images.Svg;
using Das.Views.Layout;
using Das.Views.Rendering;
using Das.Xamarin.Android.Images;
using Das.Xamarin.Android.Rendering;

// ReSharper disable UnusedMember.Global

namespace Das.Xamarin.Android;

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
                           IResolver container,
                           IImageProvider imageProvider)
      : base(imageProvider, Serializer,
         new SvgPathBuilder(imageProvider, Serializer), container,
         themeProvider)
   {
      ViewState = viewState;
      DisplayMetrics = displayMetrics;

      Init(windowManager, themeProvider, viewPerspective, displayMetrics,
         fontProvider, viewState, uiProvider,
         ref _measureContext!, ref _renderContext!, ref _refreshRenderContext!);
   }

   IMeasureContext IRenderKit.MeasureContext => _measureContext;

   IRenderContext IRenderKit.RenderContext => _renderContext;

   public void Clear()
   {
      MeasureContext.Clear();
      RenderContext.Clear();
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
      var visualLineage = new VisualLineage();

      var lastMeasures = new Dictionary<IVisualElement, ValueSize>();

      measureContext = new AndroidMeasureKit(windowManager, fontProvider,
         this, lastMeasures, themeProvider, displayMetrics, visualLineage, _layoutQueue);

      var visualPositions = new Dictionary<IVisualElement, ValueCube>();

      renderContext = new AndroidRenderContext(viewPerspective,
         fontProvider, viewState, this, visualPositions,
         lastMeasures, themeProvider, visualLineage, _layoutQueue);

      refreshRenderContext = new RefreshRenderContext(viewPerspective, this,
         visualPositions, lastMeasures, themeProvider, visualLineage, _layoutQueue,
         renderContext.GetClip);

      var imageProvider = new AndroidImageProvider(displayMetrics); //, renderContext);

      Container.ResolveTo<IImageProvider>(imageProvider);
      Container.ResolveTo(uiProvider);
      Container.ResolveTo(themeProvider);
   }

   public IViewState ViewState { get; }

   public DisplayMetrics DisplayMetrics { get; }

   public AndroidMeasureKit MeasureContext => _measureContext;

   public RefreshRenderContext RefreshRenderContext => _refreshRenderContext;

   public AndroidRenderContext RenderContext => _renderContext;

   protected static readonly DasSerializer Serializer = new DasSerializer();

   private readonly AndroidMeasureKit _measureContext;
   private readonly RefreshRenderContext _refreshRenderContext;
   private readonly AndroidRenderContext _renderContext;
}