﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#endif

namespace Das.Views.Winforms
{
    internal static class WinformExtensions
    {
        public static async Task InvokeAsyncEvent(this Func<Task>? eventDelegate, 
                                                  Boolean isInvokeConcurrently)
        {
            if (!(eventDelegate is {} ev))
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


        public static async Task RunInvokeAsync(this Control ctrl, Action action)
        {
            if (!ctrl.InvokeRequired)
                action();
            else
            {
                var src = new InvokeCompletionSource<Object>(action, ctrl);
                await src.Task;
            }
        }
    }
}
