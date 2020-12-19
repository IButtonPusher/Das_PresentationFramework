using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Das.Views;
using Das.Views.Core.Drawing;
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
            return new BaseStyleContext(DefaultStyle.Instance,
                new DefaultColorPalette());
        }

        protected sealed override async void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            _styleContext = GetStyleContext();

            var displayMetrics = Resources?.DisplayMetrics ?? throw new NullReferenceException();
            ZoomLevel = displayMetrics.ScaledDensity;

            var fontProvider = new AndroidFontProvider(displayMetrics, _styleContext);

            var uiProvider = new AndroidUiProvider(this, displayMetrics);

            var windowManager = WindowManager ?? throw new NullReferenceException(
                "WindowManager cannot be null");

            var renderKit = new AndroidRenderKit(new BasePerspective(), this, 
                fontProvider, windowManager, uiProvider, _styleContext, displayMetrics);

            _view = await GetMainViewAsync(renderKit, uiProvider);

            var prov = new AndroidView(_view, Application.Context, renderKit, uiProvider);
            SetContentView(prov);
        }

        protected abstract Task<IVisualElement> GetMainViewAsync(IRenderKit renderKit,
                                                                 IUiProvider uiProvider);

        protected abstract Func<Task<Boolean>> BackButtonCommand { get; }

        public override void OnRequestPermissionsResult(Int32 requestCode, 
                                                        String[]? permissions, 
                                                        Permission[]? grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void RegisterStyleSetter(IVisualElement element, 
                                        StyleSetterType setterType, 
                                        StyleSelector selector, 
                                        Object value)
        {
            CurrentStyleContext.RegisterStyleSetter(element, setterType, selector, value);
        }

        public IColorPalette ColorPalette => CurrentStyleContext.ColorPalette;

        public override async void OnBackPressed()
        {
            var handled = await BackButtonCommand();
            if (!handled)
                base.OnBackPressed();
        }

        public T GetStyleSetter<T>(StyleSetterType setterType,
                                   IVisualElement element)
            => GetStyleSetter<T>(setterType, StyleSelector.None, element);

        public T GetStyleSetter<T>(StyleSetterType setterType, 
                                   StyleSelector selector, 
                                   IVisualElement element)
        {
            return CurrentStyleContext.GetStyleSetter<T>(setterType, selector, element);

            //if (_styleContext is {} valid)
            //    return valid.GetStyleSetter<T>(setter, selector, element);

            //if (_view?.StyleContext is {} ctx)
            //    return ctx.GetStyleSetter<T>(setter, selector, element);

            //throw new NullReferenceException();
        }

        protected IStyleContext CurrentStyleContext =>
            _styleContext //?? _view?.StyleContext 
            ?? throw new NullReferenceException();
        

        public void RegisterStyleSetter(IVisualElement element, 
                                        StyleSetterType setterType,
                                        Object value)
        {
            CurrentStyleContext.RegisterStyleSetter(element, setterType, value);
        }

        //public IColor GetCurrentAccentColor()
        //{
        //    return CurrentStyleContext.GetCurrentAccentColor();
        //}

        public Double ZoomLevel { get; private set; } = 1.0;

        private IStyleContext? _styleContext;
        private IVisualElement? _view;
    }
}