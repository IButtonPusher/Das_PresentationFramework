using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

// ReSharper disable All

namespace System.Threading.Tasks.Sources
{
    [StructLayout(LayoutKind.Auto)]
    public struct ManualResetValueTaskSourceCore<TResult>
    {
        private Action<Object>? _continuation;
        private Object? _continuationState;
        private ExecutionContext? _executionContext;
        private Object? _capturedContext;


        private Boolean _completed;
        private TResult _result;
        private ExceptionDispatchInfo? _error;
        private Int16 _version;

        public Boolean RunContinuationsAsynchronously { get; set; }


        public Int16 Version => _version;


        public void Reset()
        {
            _version++;
            _completed = false;
            _result = default(TResult)!;
            _error = null;
            _executionContext = null;
            _capturedContext = null;
            _continuation = null;
            _continuationState = null;
        }

        public void SetResult(TResult result)
        {
            _result = result;
            SignalCompletion();
        }

        public void SetException(Exception error)
        {
            _error = ExceptionDispatchInfo.Capture(error);
            SignalCompletion();
        }

        public ValueTaskSourceStatus GetStatus(Int16 token)
        {
            ValidateToken(token);
            if (_continuation != null && _completed)
            {
                if (_error != null)
                {
                    if (!(_error.SourceException is OperationCanceledException))
                    {
                        return ValueTaskSourceStatus.Faulted;
                    }

                    return ValueTaskSourceStatus.Canceled;
                }

                return ValueTaskSourceStatus.Succeeded;
            }

            return ValueTaskSourceStatus.Pending;
        }

        public TResult GetResult(Int16 token)
        {
            ValidateToken(token);
            if (!_completed)
            {
                throw new InvalidOperationException();
            }

            _error?.Throw();
            return _result;
        }

        public void OnCompleted(Action<Object> continuation, Object state, Int16 token,
                                ValueTaskSourceOnCompletedFlags flags)
        {
            if (continuation == null)
            {
                throw new ArgumentNullException("continuation");
            }

            ValidateToken(token);
            if ((flags & ValueTaskSourceOnCompletedFlags.FlowExecutionContext) != 0)
            {
                _executionContext = ExecutionContext.Capture();
            }

            if ((flags & ValueTaskSourceOnCompletedFlags.UseSchedulingContext) != 0)
            {
                var current = SynchronizationContext.Current;
                if (current != null && current.GetType() != typeof(SynchronizationContext))
                {
                    _capturedContext = current;
                }
                else
                {
                    var current2 = TaskScheduler.Current;
                    if (current2 != TaskScheduler.Default)
                    {
                        _capturedContext = current2;
                    }
                }
            }

            Object? obj = _continuation;
            if (obj == null)
            {
                _continuationState = state;
                obj = Interlocked.CompareExchange(ref _continuation, continuation, null);
            }

            if (obj == null)
            {
                return;
            }

#pragma warning disable 252,253
            if (obj != ManualResetValueTaskSourceCoreShared.s_sentinel)
#pragma warning restore 252,253
            {
                throw new InvalidOperationException();
            }

            var capturedContext = _capturedContext;
            if (capturedContext != null)
            {
                var synchronizationContext = capturedContext as SynchronizationContext;
                if (synchronizationContext == null)
                {
                    var taskScheduler = capturedContext as TaskScheduler;
                    if (taskScheduler != null)
                    {
                        Task.Factory.StartNew(continuation, state, CancellationToken.None,
                            (TaskCreationOptions) 8, taskScheduler);
                    }
                }
                else
                {
                    synchronizationContext.Post(delegate(Object s)
                    {
                        var tuple = (Tuple<Action<Object>, Object>) s;
                        tuple.Item1(tuple.Item2);
                    }, Tuple.Create(continuation, state));
                }
            }
            else
            {
                Task.Factory.StartNew(continuation, state, CancellationToken.None,
                    (TaskCreationOptions) 8,
                    TaskScheduler.Default);
            }
        }

        private void ValidateToken(Int16 token)
        {
            if (token != _version)
            {
                throw new InvalidOperationException();
            }
        }

        private void SignalCompletion()
        {
            if (_completed)
            {
                throw new InvalidOperationException();
            }

            _completed = true;
            if (_continuation != null ||
                Interlocked.CompareExchange(ref _continuation, ManualResetValueTaskSourceCoreShared.s_sentinel, null) !=
                null)
            {
                if (_executionContext != null)
                {
                    ExecutionContext.Run(_executionContext,
                        delegate(Object s) { ((ManualResetValueTaskSourceCore<TResult>) s).InvokeContinuation(); },
                        this);
                }
                else
                {
                    InvokeContinuation();
                }
            }
        }

        private void InvokeContinuation()
        {
            var capturedContext = _capturedContext;
            if (capturedContext != null)
            {
                var synchronizationContext = capturedContext as SynchronizationContext;
                if (synchronizationContext == null)
                {
                    var taskScheduler = capturedContext as TaskScheduler;
                    if (taskScheduler != null)
                    {
                        Task.Factory.StartNew(_continuation, _continuationState, CancellationToken.None,
                            (TaskCreationOptions) 8, taskScheduler);
                    }
                }
                else
                {
                    synchronizationContext.Post(delegate(Object s)
                    {
                        var tuple = (Tuple<Action<Object>, Object>) s;
                        tuple.Item1(tuple.Item2);
                    }, Tuple.Create(_continuation, _continuationState));
                }
            }
            else if (RunContinuationsAsynchronously)
            {
                Task.Factory.StartNew(_continuation, _continuationState, CancellationToken.None,
                    (TaskCreationOptions) 8, TaskScheduler.Default);
            }
            else
            {
                _continuation!(_continuationState!);
            }
        }
    }
}