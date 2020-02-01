using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Das.Views.Invocations;

namespace Das.Views.Updaters
{
    public class StaScheduler : TaskScheduler, IUiProvider
    {
        private readonly String _staThreadName;

        public StaScheduler(String staThreadName)
        {
            _staThreadName = staThreadName;
            _taskQueue = new BlockingCollection<Task>();
            Start();
        }

        public void BeginInvoke(Action action)
        {
            Task.Factory.StartNew(action, CancellationToken.None, 
                TaskCreationOptions.PreferFairness, this);
        }

        public void Invoke(Action action)
        {
            InvocationBase.RunSync(() => Task.Factory.StartNew(action, CancellationToken.None,
                TaskCreationOptions.PreferFairness, this));
        }

        public async Task InvokeAsync(Action action) => await Task.Factory.StartNew(action, 
            CancellationToken.None, TaskCreationOptions.PreferFairness, this);

        private void Start()
        {
            var t = new Thread(RunOnCurrentThread) { Name = _staThreadName };
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void RunOnCurrentThread()
        {
            _isExecuting = true;
            Thread.CurrentThread.IsBackground = true;

            try
            {
                foreach (var task in _taskQueue.GetConsumingEnumerable())
                {
                    TryExecuteTask(task);
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                _isExecuting = false;
            }
        }

        [ThreadStatic]
        private static Boolean _isExecuting;

        private readonly BlockingCollection<Task> _taskQueue;
        protected override IEnumerable<Task> GetScheduledTasks() => Enumerable.Empty<Task>();

        protected override void QueueTask(Task task)
        {
            try
            {
                _taskQueue.Add(task);
            }
            catch (OperationCanceledException)
            {
            }
        }

        protected override Boolean TryExecuteTaskInline(Task task, Boolean taskWasPreviouslyQueued)
        {
            if (taskWasPreviouslyQueued)
                return false;

            return _isExecuting && TryExecuteTask(task);
        }
    }
}
