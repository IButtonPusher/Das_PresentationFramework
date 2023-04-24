using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PollyFuncs;
#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#endif

namespace Das.Views.Winforms;

internal static class WinformExtensions
{
   public static async Task InvokeAsyncEvent(this Func<Task>? eventDelegate,
                                             Boolean isInvokeConcurrently)
   {
      if (!(eventDelegate is { } ev))
         return;

      var list = ev.GetInvocationList();

      if (!isInvokeConcurrently)
      {
         foreach (var del in list.OfType<Func<Task>>())
         {
            await del();
         }
      }
      else
      {
         var running = new List<Task>(list.Length);
         foreach (var del in list.OfType<Func<Task>>())
         {
            running.Add(del());
         }

         await TaskEx.WhenAll(running);
      }
   }

   public static void RunBeginInvoke(this Control ctrl,
                                     Action action)
   {
      if (!ctrl.InvokeRequired)
         action();
      else
      {
         ctrl.BeginInvoke(action);
      }
   }

   public static T RunInvoke<T>(this Control ctrl,
                                Func<T> action)
   {
      if (ctrl.InvokeRequired)
      {
         var rwaffle = (T)ctrl.Invoke(
            new Func<T>(action));

         return rwaffle;
      }

      return action();
   }

   public static async Task RunInvokeAsync(this Control ctrl,
                                           Action action)
   {
      if (!ctrl.InvokeRequired)
         action();
      else
      {
         var src = new InvokeActionCompletionSource<Object>(action, ctrl);
         await src.Task;
      }
   }

   public static async Task<T> RunInvokeAsync<T>(this Control ctrl,
                                                 Func<T> action)
   {
      if (!ctrl.InvokeRequired)
         return action();

      var src = new InvokeActionCompletionSource<T>(action, ctrl);
      return await src.Task;
   }

   public static async Task RunInvokeAsync(this Control ctrl,
                                           Func<Task> action)
   {
      if (!ctrl.InvokeRequired)
      {
         await action();
         return;
      }

      var src = new InvokeActionCompletionSource<Task>(action, ctrl);
      await src.Task;
   }

   public static async Task<TOutput> RunInvokeAsync<TInput, TOutput>(this Control ctrl,
                                                                     TInput input,
                                                                     Func<TInput, TOutput> action)
   {
      if (!ctrl.InvokeRequired)
         return action(input);

      var poly = new PollyFunc<TInput, TOutput>(action, input);
      return await ctrl.RunInvokeAsync<TOutput, PollyFunc<TInput, TOutput>>(poly);
   }

   public static async Task RunInvokeAsync<TInput>(this Control ctrl,
                                                   TInput input,
                                                   Func<TInput, Task> action)
   {
      if (!ctrl.InvokeRequired)
         await action(input);
      else
      {
         var poly = new PollyFunc<TInput, Task>(action, input);
         await ctrl.RunInvokeAsync<Task, PollyFunc<TInput, Task>>(poly);
      }
   }

   public static async Task<TOutput> RunInvokeAsync<TOutput>(this Control ctrl,
                                                             Func<Task<TOutput>> action)
   {
      if (!ctrl.InvokeRequired)
         return await action();

      var src = new InvokeAsyncActionCompletionSource<TOutput>(action, ctrl);
      return await src.Task;
   }

   public static async Task<TOutput> RunInvokeAsync<TOutput, TPolly>(this Control ctrl,
                                                                     TPolly action)
      where TPolly : struct, IPollyFunc<TOutput>

   {
      var src = new InvokeActionCompletionSource<TOutput>(action.Execute, ctrl);
      return await src.Task;
   }
}
