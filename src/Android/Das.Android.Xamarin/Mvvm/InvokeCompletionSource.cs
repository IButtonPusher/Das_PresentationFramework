using System;
using System.Threading.Tasks;
using Android.App;
using Object = System.Object;

namespace Das.Xamarin.Android
{
   public class InvokeCompletionSource<T> : TaskCompletionSource<T>
   {
      public InvokeCompletionSource(Func<T> action,
                                    Activity androidActivity)
         : base(TaskCreationOptions.RunContinuationsAsynchronously)
      {
         _androidActivity = androidActivity;
         //Id = Interlocked.Increment(ref _lastId);

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

      //public Int32 Id { get; }

      //private static Int32 _lastId;
      private readonly Activity _androidActivity;
   }

   public class InvokeCompletionSource<TInput, TOutput> : TaskCompletionSource<TOutput>
   {
      public InvokeCompletionSource(Func<TInput, TOutput> action,
                                    TInput input,
                                    Activity androidActivity)
         : base(TaskCreationOptions.RunContinuationsAsynchronously)
      {
         _androidActivity = androidActivity;

         System.Threading.Tasks.Task.Factory.StartNew(_ => RunAction(action, input), input);
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

      private readonly Activity _androidActivity;
   }

   public class InvokeCompletionSource : TaskCompletionSource<Object>
   {

      public InvokeCompletionSource(Func<Task> action,
                                    Activity androidActivity)
         : base(TaskCreationOptions.RunContinuationsAsynchronously)
      {
         _androidActivity = androidActivity;
         //Id = Interlocked.Increment(ref _lastId);

         
         System.Threading.Tasks.Task.Factory.StartNew(() => RunAction(action));
      }

      private void RunAction(Func<Task> action)
      {
         _androidActivity.RunOnUiThread(() =>
         {
            var res = action();
            res.ContinueWith(SetResult);
         });
      }


      public InvokeCompletionSource(Action action,
                                    Activity androidActivity)
         : base(TaskCreationOptions.RunContinuationsAsynchronously)
      {
         _androidActivity = androidActivity;
        // Id = Interlocked.Increment(ref _lastId);

         _androidActivity.RunOnUiThread(() =>
         {
            action();
            SetResult(new Object());
         });
      }

      //public Int32 Id { get; }

      //private static Int32 _lastId;

      // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
      private readonly Activity _androidActivity;
   }
}
