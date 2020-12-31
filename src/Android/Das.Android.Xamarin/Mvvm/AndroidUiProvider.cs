using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Das.ViewModels;
using bob = System.Uri;
using Uri = Android.Net.Uri;

namespace Das.Xamarin.Android.Mvvm
{
    public class AndroidUiProvider : BaseUiProvider
    {
        public AndroidUiProvider(Activity activity)
        {
            _activity = activity;
            _looper = _activity.MainLooper ??
                      throw new ArgumentNullException(nameof(Activity.MainLooper));
        }

        public override void BrowseToUri(bob uri)
        {
            Intent intent = new Intent(Intent.ActionView);
            intent.SetData(Uri.Parse(uri.AbsoluteUri));
            _activity.StartActivity(intent);
        }


        public override void Invoke(Action action)
        {
            if (_looper.IsCurrentThread)
                action();
            else
                _activity.RunOnUiThread(action);
        }

        public override T Invoke<T>(Func<T> action)
        {
            T res = default;

            if (_looper.IsCurrentThread)
                return action();

            _activity.RunOnUiThread(() =>
            {
                res = action();
            });
            return res!;
        }

        public override async Task<T> InvokeAsync<T>(Func<T> action)
        {
            if (_looper.IsCurrentThread)
                return action();

            var src = new InvokeCompletionSource<T>(action, _activity);
            return await src.Task;
        }

        public override async Task<TOutput> InvokeAsync<TInput, TOutput>(TInput input,
                                                                         Func<TInput, TOutput> action)
        {
            if (_looper.IsCurrentThread)
                return action(input);

            var src = new InvokeCompletionSource<TInput, TOutput>(action, input, _activity);
            return await src.Task;
        }

        public override async Task InvokeAsync(Action action)
        {
            if (_looper.IsCurrentThread)
                action();
            else
            {
                var src = new InvokeCompletionSource(action, _activity);
                await src.Task;
            }
        }

        private readonly Activity _activity;
        private readonly Looper _looper;
    }
}