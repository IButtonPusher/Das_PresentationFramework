using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable All

namespace System.Runtime.CompilerServices
{
    [StructLayout(LayoutKind.Auto)]
    public struct AsyncIteratorMethodBuilder
    {
        private AsyncTaskMethodBuilder _methodBuilder;

        private Object _id;

        internal Object ObjectIdForDebugger => _id ?? Interlocked.CompareExchange(ref _id,
            new Object(), null) ?? _id;

        [DebuggerNonUserCode]
        public static AsyncIteratorMethodBuilder Create()
        {
            var result = default(AsyncIteratorMethodBuilder);
            result._methodBuilder = AsyncTaskMethodBuilder.Create();
            return result;
        }

        [MethodImpl(256)]
        public void MoveNext<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
        {
            _methodBuilder.Start(ref stateMachine);
        }

        [DebuggerNonUserCode]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter,
                                                              ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            _methodBuilder.AwaitOnCompleted(ref awaiter, ref stateMachine);
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter,
                                                                    ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            _methodBuilder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
        }

        public void Complete()
        {
            _methodBuilder.SetResult();
        }
    }
}