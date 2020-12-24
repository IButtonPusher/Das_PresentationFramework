using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

// ReSharper disable All
#pragma warning disable 8600

namespace System.Runtime.CompilerServices
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct ConfiguredValueTaskAwaitable
    {
        private readonly ValueTask _value;

        [MethodImpl(256)]
        internal ConfiguredValueTaskAwaitable(ValueTask value)
        {
            this._value = value;
        }

        [MethodImpl(256)]
        public ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter GetAwaiter()
        {
            return new ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter(this._value);
        }

        [StructLayout(LayoutKind.Auto)]
        public readonly struct ConfiguredValueTaskAwaiter : ICriticalNotifyCompletion, INotifyCompletion
        {
            private readonly ValueTask _value;

            [MethodImpl(256)]
            internal ConfiguredValueTaskAwaiter(ValueTask value)
            {
                this._value = value;
            }

            public Boolean IsCompleted
            {
                [MethodImpl(256)] get { return this._value.IsCompleted; }
            }

            [StackTraceHidden]
            [MethodImpl(256)]
            public void GetResult()
            {
                this._value.ThrowIfCompletedUnsuccessfully();
            }

            public void OnCompleted(Action continuation)
            {
                Object o = this._value._obj;
                if (o is Task task)
                    task.ConfigureAwait(this._value._continueOnCapturedContext).GetAwaiter().OnCompleted(continuation);
                else if (o != null)
                    Unsafe.As<IValueTaskSource>(o).OnCompleted(ValueTaskAwaiter.s_invokeActionDelegate,
                        (Object) continuation, this._value._token,
                        (ValueTaskSourceOnCompletedFlags) (2 | (this._value._continueOnCapturedContext ? 1 : 0)));
                else
                    ValueTask.CompletedTask.ConfigureAwait(this._value._continueOnCapturedContext).GetAwaiter()
                             .OnCompleted(continuation);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                Object o = this._value._obj;
                if (o is Task task)
                    task.ConfigureAwait(this._value._continueOnCapturedContext).GetAwaiter()
                        .UnsafeOnCompleted(continuation);
                else if (o != null)
                    Unsafe.As<IValueTaskSource>(o).OnCompleted(ValueTaskAwaiter.s_invokeActionDelegate,
                        (Object) continuation, this._value._token,
                        this._value._continueOnCapturedContext
                            ? ValueTaskSourceOnCompletedFlags.UseSchedulingContext
                            : ValueTaskSourceOnCompletedFlags.None);
                else
                    ValueTask.CompletedTask.ConfigureAwait(this._value._continueOnCapturedContext).GetAwaiter()
                             .UnsafeOnCompleted(continuation);
            }
        }
    }

    [StructLayout(LayoutKind.Auto)]
    public readonly struct ConfiguredValueTaskAwaitable<TResult>
    {
        private readonly ValueTask<TResult> _value;

        [MethodImpl(256)]
        internal ConfiguredValueTaskAwaitable(ValueTask<TResult> value)
        {
            this._value = value;
        }

        /// <returns></returns>
        [MethodImpl(256)]
        public ConfiguredValueTaskAwaitable<TResult>.ConfiguredValueTaskAwaiter GetAwaiter()
        {
            return new ConfiguredValueTaskAwaitable<TResult>.ConfiguredValueTaskAwaiter(this._value);
        }


        [StructLayout(LayoutKind.Auto)]
        public readonly struct ConfiguredValueTaskAwaiter : ICriticalNotifyCompletion, INotifyCompletion
        {
            private readonly ValueTask<TResult> _value;

            [MethodImpl(256)]
            internal ConfiguredValueTaskAwaiter(ValueTask<TResult> value)
            {
                this._value = value;
            }

            /// <returns></returns>
            public Boolean IsCompleted
            {
                [MethodImpl(256)] get { return this._value.IsCompleted; }
            }

            /// <returns></returns>
            [StackTraceHidden]
            [MethodImpl(256)]
            public TResult GetResult()
            {
                return this._value.Result;
            }

            /// <param name="continuation"></param>
            public void OnCompleted(Action continuation)
            {
                Object o = this._value._obj;
                if (o is Task<TResult> task)
                    task.ConfigureAwait(this._value._continueOnCapturedContext).GetAwaiter().OnCompleted(continuation);
                else if (o != null)
                    Unsafe.As<IValueTaskSource<TResult>>(o).OnCompleted(ValueTaskAwaiter.s_invokeActionDelegate,
                        (Object) continuation, this._value._token,
                        (ValueTaskSourceOnCompletedFlags) (2 | (this._value._continueOnCapturedContext ? 1 : 0)));
                else
                    ValueTask.CompletedTask.ConfigureAwait(this._value._continueOnCapturedContext).GetAwaiter()
                             .OnCompleted(continuation);
            }

            /// <param name="continuation"></param>
            public void UnsafeOnCompleted(Action continuation)
            {
                Object o = this._value._obj;
                if (o is Task<TResult> task)
                    task.ConfigureAwait(this._value._continueOnCapturedContext).GetAwaiter()
                        .UnsafeOnCompleted(continuation);
                else if (o != null)
                    Unsafe.As<IValueTaskSource<TResult>>(o).OnCompleted(ValueTaskAwaiter.s_invokeActionDelegate,
                        (Object) continuation, this._value._token,
                        this._value._continueOnCapturedContext
                            ? ValueTaskSourceOnCompletedFlags.UseSchedulingContext
                            : ValueTaskSourceOnCompletedFlags.None);
                else
                    ValueTask.CompletedTask.ConfigureAwait(this._value._continueOnCapturedContext).GetAwaiter()
                             .UnsafeOnCompleted(continuation);
            }
        }
    }
}