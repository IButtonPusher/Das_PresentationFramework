using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

// ReSharper disable All

namespace System.Runtime.CompilerServices
{
    public readonly struct ValueTaskAwaiter : ICriticalNotifyCompletion
    {
        internal static readonly Action<Object> s_invokeActionDelegate = state =>
        {
            if (!(state is Action action))
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.state);
            else
                action();
        };

        private readonly ValueTask _value;

        [MethodImpl(256)]
        internal ValueTaskAwaiter(ValueTask value)
        {
            _value = value;
        }

        public Boolean IsCompleted
        {
            [MethodImpl(256)] get { return _value.IsCompleted; }
        }

        [StackTraceHidden]
        [MethodImpl(256)]
        public void GetResult()
        {
            _value.ThrowIfCompletedUnsuccessfully();
        }

        public void OnCompleted(Action continuation)
        {
            var o = _value._obj;
            if (o is Task task)
                task.GetAwaiter().OnCompleted(continuation);
            else if (o != null)
                Unsafe.As<IValueTaskSource>(o).OnCompleted(s_invokeActionDelegate,
                    continuation, _value._token,
                    ValueTaskSourceOnCompletedFlags.UseSchedulingContext |
                    ValueTaskSourceOnCompletedFlags.FlowExecutionContext);
            else
                ValueTask.CompletedTask.GetAwaiter().OnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            var o = _value._obj;
            if (o is Task task)
                task.GetAwaiter().UnsafeOnCompleted(continuation);
            else if (o != null)
                Unsafe.As<IValueTaskSource>(o).OnCompleted(s_invokeActionDelegate, continuation, _value._token,
                    ValueTaskSourceOnCompletedFlags.UseSchedulingContext);
            else
                ValueTask.CompletedTask.GetAwaiter().UnsafeOnCompleted(continuation);
        }
    }

    public readonly struct ValueTaskAwaiter<TResult> : ICriticalNotifyCompletion
    {
        private readonly ValueTask<TResult> _value;

        [MethodImpl(256)]
        internal ValueTaskAwaiter(ValueTask<TResult> value)
        {
            _value = value;
        }

        public Boolean IsCompleted
        {
            [MethodImpl(256)] get { return _value.IsCompleted; }
        }

        [StackTraceHidden]
        [MethodImpl(256)]
        public TResult GetResult()
        {
            return _value.Result;
        }


        public void OnCompleted(Action continuation)
        {
            var o = _value._obj;
            if (o is Task<TResult> task)
                task.GetAwaiter().OnCompleted(continuation);
            else if (o != null)
                Unsafe.As<IValueTaskSource<TResult>>(o).OnCompleted(ValueTaskAwaiter.s_invokeActionDelegate,
                    continuation, _value._token, ValueTaskSourceOnCompletedFlags.UseSchedulingContext
                                                 | ValueTaskSourceOnCompletedFlags.FlowExecutionContext);
            else
                ValueTask.CompletedTask.GetAwaiter().OnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            var o = _value._obj;
            if (o is Task<TResult> task)
                task.GetAwaiter().UnsafeOnCompleted(continuation);
            else if (o != null)
                Unsafe.As<IValueTaskSource<TResult>>(o).OnCompleted(ValueTaskAwaiter.s_invokeActionDelegate,
                    continuation, _value._token, ValueTaskSourceOnCompletedFlags.UseSchedulingContext);
            else
                ValueTask.CompletedTask.GetAwaiter().UnsafeOnCompleted(continuation);
        }
    }
}