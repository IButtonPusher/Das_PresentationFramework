using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Das.Views;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Styles;
using Das.Xamarin.Android.Mvvm;
using Xamarin.Essentials;
// ReSharper disable VirtualMemberNeverOverridden.Global

namespace Das.Xamarin.Android
{
    public abstract class DasMainActivity : AppCompatActivity, 
                                            IViewState
    {
        protected virtual IStyleContext GetStyleContext()
        {
            return new BaseStyleContext(new DefaultStyle());
        }

        protected sealed override async void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            _styleContext = GetStyleContext();

            var displayMetrics = Resources?.DisplayMetrics ?? throw new NullReferenceException();

            var fontProvider = new AndroidFontProvider(displayMetrics);

            var uiProvider = new AndroidUiProvider(this);

            var renderKit = new AndroidRenderKit(new BasePerspective(), this, 
                fontProvider, WindowManager?? throw new NullReferenceException(
                    "WindowManager cannot be null"), uiProvider);

            _view = await GetMainViewAsync(renderKit, uiProvider);

            var prov = new AndroidView(_view, Application.Context, renderKit, uiProvider);
            SetContentView(prov);
        }

        protected abstract Task<IView> GetMainViewAsync(IRenderKit renderKit,
                                                        IUiProvider uiProvider);

        protected abstract Func<Task<Boolean>> BackButtonCommand { get; }

        public override void OnRequestPermissionsResult(Int32 requestCode, 
                                                        String[]? permissions, 
                                                        Permission[]? grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override async void OnBackPressed()
        {
            var handled = await BackButtonCommand();
            if (!handled)
                base.OnBackPressed();
        }

        public T GetStyleSetter<T>(StyleSetter setter,
                                   IVisualElement element)
            => GetStyleSetter<T>(setter, StyleSelector.None, element);

        public T GetStyleSetter<T>(StyleSetter setter, 
                                   StyleSelector selector, 
                                   IVisualElement element)
        {
            if (_styleContext is {} valid)
                return valid.GetStyleSetter<T>(setter, selector, element);

            if (_view?.StyleContext is {} ctx)
                return ctx.GetStyleSetter<T>(setter, selector, element);

            throw new NullReferenceException();
        }

        public Double ZoomLevel => 1;

        private IStyleContext? _styleContext;
        private IView? _view;
    }
}