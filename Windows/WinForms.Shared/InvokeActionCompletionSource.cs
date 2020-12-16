using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Das.Views.Winforms
{
    public class InvokeAsyncActionCompletionSource<T> : TaskCompletionSource<T>
    {
        public InvokeAsyncActionCompletionSource(Func<Task<T>> run, 
                                                 Control onControl)
        {
            var d = (Action)(() => RunWrapped(run));

            onControl.BeginInvoke(d);
        }

        private void RunWrapped(Func<Task<T>> run)
        {
            var running = run();
            running.ContinueWith(OnFinished);
        }

        private void OnFinished(Task<T> promise)
        {
            SetResult(promise.Result);
        }
    }

    public class InvokeActionCompletionSource<T> : TaskCompletionSource<T>
    {
        public InvokeActionCompletionSource(Action run, 
                                            Control onControl)
        {
            var d = (Action)(() => RunWrapped(run, this));

            onControl.BeginInvoke(d);
        }

        public InvokeActionCompletionSource(Func<T> run, 
                                            Control onControl)
        {
            var d = (Action)(() => RunWrapped(run, this));

            onControl.BeginInvoke(d);
        }

      

        private static void RunWrapped(Action run, 
                                       TaskCompletionSource<T> completeMe)
        {
            run();
            completeMe.SetResult(default!);
        }

        private static void RunWrapped(Func<T> run, 
                                       TaskCompletionSource<T> completeMe)
        {
            var res = run();
            completeMe.SetResult(res);
        }

       
    }
}
