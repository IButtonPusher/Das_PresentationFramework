using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Das.Views.Winforms
{
    public class InvokeCompletionSource<T> : TaskCompletionSource<T>
    {
        public InvokeCompletionSource(Action run, Control onControl)
        {
            var d = (Action)(() => RunWrapped(run, this));

            onControl.BeginInvoke(d);
        }

        private static void RunWrapped(Action run, TaskCompletionSource<T> completeMe)
        {
            run();
            completeMe.SetResult(default!);
        }
    }
}
