using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;

namespace Das.Xamarin.Android
{
    public class InvokeCompletionSource<T> : TaskCompletionSource<T>
    {
        private readonly Activity _androidActivity;

        public InvokeCompletionSource(Func<T> action,
                                      Activity androidActivity)
            : base(TaskCreationOptions.RunContinuationsAsynchronously)
        {
            _androidActivity = androidActivity;
            Id = Interlocked.Increment(ref _lastId);

            System.Threading.Tasks.Task.Factory.StartNew(() => RunAction(action));
        }

        private void RunAction(Func<T> action)
        {
            _androidActivity.RunOnUiThread(() =>
            {
                var res = action();
                SetResult(res);
            });
        }

        public Int32 Id { get; }

        private static Int32 _lastId;
    }

    public class InvokeCompletionSource<TInput, TOutput> : TaskCompletionSource<TOutput>
    {
        private readonly Activity _androidActivity;

        public InvokeCompletionSource(Func<TInput, TOutput> action,
                                      TInput input,
                                      Activity androidActivity)
            : base(TaskCreationOptions.RunContinuationsAsynchronously)
        {
            _androidActivity = androidActivity;

            System.Threading.Tasks.Task.Factory.StartNew(t => RunAction(action, input), input);
        }

        private void RunAction(Func<TInput, TOutput> action,
                               TInput input)
        {
            _androidActivity.RunOnUiThread(() =>
            {
                var res = action(input);
                SetResult(res);
            });
        }
    }

    public class InvokeCompletionSource : TaskCompletionSource<Object>
    {
        private readonly Activity _androidActivity;

        public InvokeCompletionSource(Action action,
                                      Activity androidActivity)
            : base(TaskCreationOptions.RunContinuationsAsynchronously)
        {
            _androidActivity = androidActivity;
            Id = Interlocked.Increment(ref _lastId);

            System.Threading.Tasks.Task.Factory.StartNew(() => RunAction(action));
        }

        private void RunAction(Action action)
        {
            _androidActivity.RunOnUiThread(() =>
            {
                action();
                SetResult(new Object());
            });
        }

        public Int32 Id { get; }

        private static Int32 _lastId;
    }
}