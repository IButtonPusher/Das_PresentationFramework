using System;
using Android.Util;
using Das.Views.Rendering;

namespace Das.Xamarin.Android
{
    public class AndroidViewState : IViewState
    {
        public AndroidViewState(DisplayMetrics displayMetrics)
        {
            ZoomLevel = displayMetrics.ScaledDensity;
        }

        public Double ZoomLevel { get; }
    }
}