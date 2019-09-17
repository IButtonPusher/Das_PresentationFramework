using System;
using System.Collections.Generic;
using System.Threading;

namespace Das.Views.Invocations
{
    public class ExclusiveSynchronizationContext : SynchronizationContext
    {
        private Boolean done;
        public Exception InnerException { get; set; }
        readonly AutoResetEvent workItemsWaiting = new AutoResetEvent(false);

        readonly Queue<Tuple<SendOrPostCallback, Object>> items =
            new Queue<Tuple<SendOrPostCallback, Object>>();

        public override void Send(SendOrPostCallback d, Object state)
        {
            throw new NotSupportedException("We cannot send to our same thread");
        }

        public override void Post(SendOrPostCallback d, Object state)
        {
            lock (items)
            {
                items.Enqueue(Tuple.Create(d, state));
            }

            workItemsWaiting.Set();
        }

        public void EndMessageLoop()
        {
            Post(_ => done = true, null);
        }

        public void BeginMessageLoop()
        {
            while (!done)
            {
                Tuple<SendOrPostCallback, Object> task = null;
                lock (items)
                {
                    if (items.Count > 0)
                    {
                        task = items.Dequeue();
                    }
                }

                if (task != null)
                {
                    task.Item1(task.Item2);
                    if (InnerException != null) // the method threw an exeption
                    {
                        throw new AggregateException("AsyncHelpers.Run method threw an exception.", InnerException);
                    }
                }
                else
                {
                    workItemsWaiting.WaitOne();
                }
            }
        }

        public override SynchronizationContext CreateCopy() => this;
    }
}
