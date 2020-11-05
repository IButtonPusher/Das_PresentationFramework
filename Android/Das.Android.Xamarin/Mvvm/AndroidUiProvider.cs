using System;
using System.Threading.Tasks;
using Android.App;
using Das.ViewModels;

namespace Das.Xamarin.Android.Mvvm
{
    public class AndroidUiProvider : BaseUiProvider
    {
        private readonly Activity _activity;

        public AndroidUiProvider(Activity activity)
        {
            _activity = activity;
        }


        public override T Invoke<T>(Func<T> action)
        {
            T res = default;
            _activity.RunOnUiThread(() =>
            {
                res = action();
            });
            return res!;
        }
    }
}