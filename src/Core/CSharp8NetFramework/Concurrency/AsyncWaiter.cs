using System;
using System.Threading.Tasks;

namespace System.Threading
{
    public class SyncWaiter : AsyncTaskCompletionSource<Boolean>,
                              ITaskWaiter
    {
        public SyncWaiter(WorkerTypes workerType)
        {
            WorkerType = workerType;
            Status = TaskStatus.WaitingForActivation;
        }

        public WorkerTypes WorkerType { get; }

        public TaskStatus Status { get; private set; }

        public void BeginExecute()
        {
            Status = TaskStatus.WaitingToRun;
        }

        public void ExecuteBlocking()
        {
            Status = TaskStatus.WaitingToRun;
        }

        public void Cancel()
        {
            Status = TaskStatus.Canceled;
        }
    }


    public enum WorkerTypes
    {
        Reader,
        IteratorReader,

        Writer
    }
}
