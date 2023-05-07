using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Das.Container;
using Das.Views;
using Das.Views.Colors;
using Das.Views.Core;
using Das.Views.Rendering;
using Das.Views.Styles;
using Das.Xamarin.Android.Images;
using Das.Xamarin.Android.Mvvm;
using Xamarin.Essentials;

// ReSharper disable VirtualMemberNeverOverridden.Global

namespace Das.Xamarin.Android;

public abstract class DasMainActivity : AppCompatActivity
{
   protected DasMainActivity()
   {
      _defaultBackButtonCommand = OnDefaultBackButtonCommand;
   }

   private Task<Boolean> OnDefaultBackButtonCommand()
   {
      Finish();
      return Task.FromResult(true);
   }

   public override async void OnBackPressed()
   {
      var handled = await BackButtonCommand();
      if (!handled)
         base.OnBackPressed();
   }

   public override void OnRequestPermissionsResult(Int32 requestCode,
                                                   String[] permissions,
                                                   Permission[] grantResults)
   {
      Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

      base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
   }

   protected abstract Task<IVisualElement> GetMainViewAsync(IRenderKit renderKit,
                                                            // ReSharper disable once UnusedParameter.Global
                                                            IUiProvider uiProvider);

   protected virtual IResolver GetDependencyContainer() => new BaseResolver();

   protected virtual IThemeProvider GetThemeProvider()
   {
      var darkTheme = (Int32)UiMode.NightYes;

      if (Application.Context.Resources?.Configuration?.UiMode is { } uiMode &&
          ((Int32)uiMode & darkTheme) == darkTheme)
      {
         return DefaultDarkThemeProvider.Instance;
      }


      //var rdrr = Application.Context.Resources?.Configuration?.UiMode;
      //if (rdrr != null)
      //{
      //   UILogger.Log("Detected running theme: " + rdrr, LogLevel.Lavel0);
      //   var bob = rdrr.Value;

      //   if (((Int32)bob & 32) == 32)
      //   {
      //      UILogger.Log("Detected dark theme", LogLevel.Lavel0);
      //   }


      //}

      return BaselineThemeProvider.Instance;
   }

   protected virtual IImageProvider GetImageProvider(DisplayMetrics displayMetrics) =>
      new AndroidImageProvider(displayMetrics);

   protected sealed override async void OnCreate(Bundle? savedInstanceState)
   {
      // System.Diagnostics.Debug.WriteLine("********* OnCreate()");

      base.OnCreate(savedInstanceState);
      Platform.Init(this, savedInstanceState);

      var themeProvider = GetThemeProvider();

      var displayMetrics = Resources?.DisplayMetrics ?? throw new NullReferenceException();


      var fontProvider = new AndroidFontProvider(displayMetrics);

      var uiProvider = new AndroidUiProvider(this);

      var windowManager = WindowManager ?? throw new NullReferenceException(
         "WindowManager cannot be null");

      var viewState = new AndroidViewState(displayMetrics, themeProvider);

      var resolver = GetDependencyContainer();
      var imageProvider = GetImageProvider(displayMetrics);

      var renderKit = new AndroidRenderKit(new BasePerspective(), viewState,
         fontProvider, windowManager, uiProvider, themeProvider,
         displayMetrics, resolver, imageProvider);

      _view = await GetMainViewAsync(renderKit, uiProvider);

      var prov = new AndroidView(_view, Application.Context, renderKit, uiProvider);
      _androidView = prov;
      SetContentView(prov);

      await renderKit.Container.ResolveToAsync(prov);
   }

   protected sealed override void OnPause()
   {
      //System.Diagnostics.Debug.WriteLine("********* OnPause()");

      base.OnPause();

      _androidView?.OnPause();
   }

   protected sealed override void OnResume()
   {
      //System.Diagnostics.Debug.WriteLine("********* OnResume()");

      base.OnResume();

      _androidView?.OnResume();
   }

   // ReSharper disable once UnusedMember.Global
   protected abstract Func<Task<Boolean>> BackButtonCommand { get; }

   protected readonly Func<Task<Boolean>> _defaultBackButtonCommand;

   private AndroidView? _androidView;


   private IVisualElement? _view;
}
