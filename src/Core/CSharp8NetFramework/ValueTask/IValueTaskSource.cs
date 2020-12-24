using System;
using System.Threading.Tasks;

namespace System.Threading.Tasks.Sources
{
    [Flags]
    public enum ValueTaskSourceOnCompletedFlags
    {
        None = 0,
        UseSchedulingContext = 1,
        FlowExecutionContext = 2
    }

    public enum ValueTaskSourceStatus
    {
        Pending,
        Succeeded,
        Faulted,
        Canceled
    }

    public interface IValueTaskSource
    {
        void GetResult(Int16 token);

        ValueTaskSourceStatus GetStatus(Int16 token);

        void OnCompleted(
            Action<Object> continuation,
            Object state,
            Int16 token,
            ValueTaskSourceOnCompletedFlags flags);
    }

    public interface IValueTaskSource<out TResult>
    {
        TResult GetResult(Int16 token);

        ValueTaskSourceStatus GetStatus(Int16 token);

        void OnCompleted(
            Action<Object> continuation,
            Object state,
            Int16 token,
            ValueTaskSourceOnCompletedFlags flags);
    }
}