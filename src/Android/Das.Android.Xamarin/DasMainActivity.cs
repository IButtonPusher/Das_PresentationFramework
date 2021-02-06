using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
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

namespace Das.Xamarin.Android
{
    public abstract class DasMainActivity : AppCompatActivity
    {
        // ReSharper disable once UnusedMember.Global
        protected abstract Func<Task<Boolean>> BackButtonCommand { get; }


        public override async void OnBackPressed()
        {
            var handled = await BackButtonCommand();
            if (!handled)
                base.OnBackPressed();
        }

        public override void OnRequestPermissionsResult(Int32 requestCode,
                                                        String[]? permissions,
                                                        Permission[]? grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected abstract Task<IVisualElement> GetMainViewAsync(IRenderKit renderKit,
                                                                 // ReSharper disable once UnusedParameter.Global
                                                                 IUiProvider uiProvider);

        protected virtual IResolver GetDependencyContainer() => new BaseResolver();

        protected virtual IThemeProvider GetThemeProvider() => BaselineThemeProvider.Instance;

        protected virtual IImageProvider GetImageProvider(DisplayMetrics displayMetrics)
            => new AndroidImageProvider(displayMetrics);

        protected sealed override async void OnCreate(Bundle? savedInstanceState)
        {
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
            SetContentView(prov);
        }


        private IVisualElement? _view;
    }
}
