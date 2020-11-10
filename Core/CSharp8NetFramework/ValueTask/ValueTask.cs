using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
#pragma warning disable 8625

// ReSharper disable All
#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#endif
#pragma warning disable 8625
#pragma warning disable 8601

namespace System.Threading.Tasks
{
    [AsyncMethodBuilder(typeof(AsyncValueTaskMethodBuilder))]
    [StructLayout(LayoutKind.Auto)]
    public readonly struct ValueTask : IEquatable<ValueTask>
    {
        private static readonly Task s_canceledTask = TaskEx.Delay(-1, new CancellationToken(true));
        internal readonly Object _obj;
        internal readonly Int16 _token;
        internal readonly Boolean _continueOnCapturedContext;

        internal static Task CompletedTask { get; } = TaskEx.Delay(0);

        [MethodImpl(256)]
        public ValueTask(Task task)
        {
            if (task == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.task);
            _obj = task;
            _continueOnCapturedContext = true;
            _token = 0;
        }

        [MethodImpl(256)]
        public ValueTask(IValueTaskSource source, Int16 token)
        {
            if (source == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            _obj = source;
            _token = token;
            _continueOnCapturedContext = true;
        }

        [MethodImpl(256)]
        private ValueTask(Object obj, Int16 token, Boolean continueOnCapturedContext)
        {
            _obj = obj;
            _token = token;
            _continueOnCapturedContext = continueOnCapturedContext;
        }

        public override Int32 GetHashCode()
        {
            Object obj = _obj;
            if (obj == null)
                return 0;
            return obj.GetHashCode();
        }

        public override Boolean Equals(Object obj)
        {
            if (obj is ValueTask)
                return Equals((ValueTask) obj);
            return false;
        }

        public Boolean Equals(ValueTask other)
        {
            if (_obj == other._obj)
                return _token == other._token;
            return false;
        }

        public static Boolean operator ==(ValueTask left, ValueTask right)
        {
            return left.Equals(right);
        }

        public static Boolean operator !=(ValueTask left, ValueTask right)
        {
            return !left.Equals(right);
        }

        public Task AsTask()
        {
            Object o = _obj;
            if (o != null)
                return o as Task ?? GetTaskForValueTaskSource(Unsafe.As<IValueTaskSource>(o));
            return CompletedTask;
        }

        public ValueTask Preserve()
        {
            if (_obj != null)
                return new ValueTask(AsTask());
            return this;
        }

        private Task GetTaskForValueTaskSource(IValueTaskSource t)
        {
            ValueTaskSourceStatus status = t.GetStatus(_token);
            if (status == ValueTaskSourceStatus.Pending)
                return new ValueTaskSourceAsTask(t, _token).Task;
            try
            {
                t.GetResult(_token);
                return CompletedTask;
            }
            catch (Exception ex)
            {
                if (status == ValueTaskSourceStatus.Canceled)
                    return s_canceledTask;
                TaskCompletionSource<Boolean> completionSource = new TaskCompletionSource<Boolean>();
                completionSource.TrySetException(ex);
                return completionSource.Task;
            }
        }

        public Boolean IsCompleted
        {
            [MethodImpl(256)]
            get
            {
                Object o = _obj;
                if (o == null)
                    return true;
                if (o is Task task)
                    return task.IsCompleted;
                return (UInt32) Unsafe.As<IValueTaskSource>(o).GetStatus(_token) > 0U;
            }
        }

        public Boolean IsCompletedSuccessfully
        {
            [MethodImpl(256)]
            get
            {
                Object o = _obj;
                if (o == null)
                    return true;
                if (o is Task task)
                    return task.Status == TaskStatus.RanToCompletion;
                return Unsafe.As<IValueTaskSource>(o).GetStatus(_token) == ValueTaskSourceStatus.Succeeded;
            }
        }

        public Boolean IsFaulted
        {
            get
            {
                Object o = _obj;
                if (o == null)
                    return false;
                if (o is Task task)
                    return task.IsFaulted;
                return Unsafe.As<IValueTaskSource>(o).GetStatus(_token) == ValueTaskSourceStatus.Faulted;
            }
        }

        public Boolean IsCanceled
        {
            get
            {
                Object o = _obj;
                if (o == null)
                    return false;
                if (o is Task task)
                    return task.IsCanceled;
                return Unsafe.As<IValueTaskSource>(o).GetStatus(_token) == ValueTaskSourceStatus.Canceled;
            }
        }

        [StackTraceHidden]
        [MethodImpl(256)]
        internal void ThrowIfCompletedUnsuccessfully()
        {
            Object o = _obj;
            if (o == null)
                return;
            if (o is Task task)
                task.GetAwaiter().GetResult();
            else
                Unsafe.As<IValueTaskSource>(o).GetResult(_token);
        }

        public ValueTaskAwaiter GetAwaiter()
        {
            return new ValueTaskAwaiter(this);
        }

        [MethodImpl(256)]
        public ConfiguredValueTaskAwaitable ConfigureAwait(
            Boolean continueOnCapturedContext)
        {
            return new ConfiguredValueTaskAwaitable(new ValueTask(_obj, _token, continueOnCapturedContext));
        }

        private sealed class ValueTaskSourceAsTask : TaskCompletionSource<Boolean>
        {
            public ValueTaskSourceAsTask(IValueTaskSource source, Int16 token)
            {
                _token = token;
                _source = source;
                source.OnCompleted(s_completionAction, this, token, ValueTaskSourceOnCompletedFlags.None);
            }

            private static readonly Action<Object> s_completionAction = state =>
            {
                IValueTaskSource source;
                if (!(state is ValueTaskSourceAsTask taskSourceAsTask) || (source = taskSourceAsTask._source) == null)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.state);
                }
                else
                {
                    taskSourceAsTask._source = null;
                    ValueTaskSourceStatus status = source.GetStatus(taskSourceAsTask._token);
                    try
                    {
                        source.GetResult(taskSourceAsTask._token);
                        taskSourceAsTask.TrySetResult(false);
                    }
                    catch (Exception ex)
                    {
                        if (status == ValueTaskSourceStatus.Canceled)
                            taskSourceAsTask.TrySetCanceled();
                        else
                            taskSourceAsTask.TrySetException(ex);
                    }
                }
            };

            private readonly Int16 _token;

            private IValueTaskSource _source;
        }
    }

    [AsyncMethodBuilder(typeof(AsyncValueTaskMethodBuilder<>))]
    [StructLayout(LayoutKind.Auto)]
    public readonly struct ValueTask<TResult> : IEquatable<ValueTask<TResult>>
    {
        private static Task<TResult>? s_canceledTask;
        internal readonly Object? _obj;
        internal readonly TResult _result;
        internal readonly Int16 _token;
        internal readonly Boolean _continueOnCapturedContext;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ValueTask{TResult}"></see> class using the supplied result of a
        ///     successful operation.
        /// </summary>
        /// <param name="result">The result.</param>
        [MethodImpl(256)]
        public ValueTask(TResult result)
        {
            _result = result;
            _obj = null;
            _continueOnCapturedContext = true;
            _token = 0;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ValueTask{TResult}"></see> class using the supplied task that
        ///     represents the operation.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="task">task</paramref> argument is null.</exception>
        [MethodImpl(256)]
        public ValueTask(Task<TResult> task)
        {
            if (task == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.task);
            _obj = task;
            _result = default(TResult)!;
            _continueOnCapturedContext = true;
            _token = 0;
        }

        [MethodImpl(256)]
        public ValueTask(IValueTaskSource<TResult> source, Int16 token)
        {
            if (source == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            _obj = source;
            _token = token;
            _result = default(TResult)!;
            _continueOnCapturedContext = true;
        }

        [MethodImpl(256)]
        private ValueTask(Object obj, TResult result, Int16 token, Boolean continueOnCapturedContext)
        {
            _obj = obj;
            _result = result;
            _token = token;
            _continueOnCapturedContext = continueOnCapturedContext;
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>The hash code for the current object.</returns>
        public override Int32 GetHashCode()
        {
            if (_obj != null)
                return _obj.GetHashCode();
            if (_result == null)
                return 0;
            return _result.GetHashCode();
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override Boolean Equals(Object obj)
        {
            if (obj is ValueTask<TResult>)
                return Equals((ValueTask<TResult>) obj);
            return false;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="ValueTask{TResult}"></see> object is equal to the current
        ///     <see cref="ValueTask{TResult}"></see> object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public Boolean Equals(ValueTask<TResult> other)
        {
            if (_obj == null && other._obj == null)
                return EqualityComparer<TResult>.Default.Equals(_result, other._result);
            if (_obj == other._obj)
                return _token == other._token;
            return false;
        }

        /// <summary>Compares two values for equality.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the two <see cref="ValueTask{TResult}"></see> values are equal; otherwise, false.</returns>
        public static Boolean operator ==(ValueTask<TResult> left, ValueTask<TResult> right)
        {
            return left.Equals(right);
        }

        /// <summary>Determines whether two <see cref="ValueTask{TResult}"></see> values are unequal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The seconed value to compare.</param>
        /// <returns>true if the two <see cref="ValueTask{TResult}"></see> values are not equal; otherwise, false.</returns>
        public static Boolean operator !=(ValueTask<TResult> left, ValueTask<TResult> right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        ///     Retrieves a <see cref="Task{TResult}"></see> object that represents this <see cref="ValueTask{TResult}"></see>
        ///     .
        /// </summary>
        /// <returns>
        ///     The <see cref="Task{TResult}"></see> object that is wrapped in this <see cref="ValueTask{TResult}"></see> if
        ///     one exists, or a new <see cref="Task{TResult}"></see> object that represents the result.
        /// </returns>
        public Task<TResult> AsTask()
        {
            Object? o = _obj;
            if (o == null)
                return TaskEx.FromResult(_result);
            if (o is Task<TResult> task)
                return task;
            return GetTaskForValueTaskSource(Unsafe.As<IValueTaskSource<TResult>>(o));
        }

        public ValueTask<TResult> Preserve()
        {
            if (_obj != null)
                return new ValueTask<TResult>(AsTask());
            return this;
        }

        private Task<TResult> GetTaskForValueTaskSource(IValueTaskSource<TResult> t)
        {
            ValueTaskSourceStatus status = t.GetStatus(_token);
            if (status == ValueTaskSourceStatus.Pending)
                return new ValueTaskSourceAsTask(t, _token).Task;
            try
            {
                return TaskEx.FromResult(t.GetResult(_token));
            }
            catch (Exception ex)
            {
                if (status == ValueTaskSourceStatus.Canceled)
                {
                    Task<TResult>? task = s_canceledTask;
                    if (task == null)
                    {
                        TaskCompletionSource<TResult> completionSource = new TaskCompletionSource<TResult>();
                        completionSource.TrySetCanceled();
                        task = completionSource.Task;
                        s_canceledTask = task;
                    }

                    return task;
                }

                TaskCompletionSource<TResult> completionSource1 = new TaskCompletionSource<TResult>();
                completionSource1.TrySetException(ex);
                return completionSource1.Task;
            }
        }

        /// <summary>Gets a value that indicates whether this object represents a completed operation.</summary>
        /// <returns>true if this object represents a completed operation; otherwise, false.</returns>
        public Boolean IsCompleted
        {
            [MethodImpl(256)]
            get
            {
                Object? o = _obj;
                if (o == null)
                    return true;
                if (o is Task<TResult> task)
                    return task.IsCompleted;
                return (UInt32) Unsafe.As<IValueTaskSource<TResult>>(o).GetStatus(_token) > 0U;
            }
        }

        /// <summary>Gets a value that indicates whether this object represents a successfully completed operation.</summary>
        /// <returns>true if this object represents a successfully completed operation; otherwise, false.</returns>
        public Boolean IsCompletedSuccessfully
        {
            [MethodImpl(256)]
            get
            {
                Object? o = _obj;
                if (o == null)
                    return true;
                if (o is Task<TResult> task)
                    return task.Status == TaskStatus.RanToCompletion;
                return Unsafe.As<IValueTaskSource<TResult>>(o).GetStatus(_token) == ValueTaskSourceStatus.Succeeded;
            }
        }

        /// <summary>Gets a value that indicates whether this object represents a failed operation.</summary>
        /// <returns>true if this object represents a failed operation; otherwise, false.</returns>
        public Boolean IsFaulted
        {
            get
            {
                Object? o = _obj;
                if (o == null)
                    return false;
                if (o is Task<TResult> task)
                    return task.IsFaulted;
                return Unsafe.As<IValueTaskSource<TResult>>(o).GetStatus(_token) == ValueTaskSourceStatus.Faulted;
            }
        }

        /// <summary>Gets a value that indicates whether this object represents a canceled operation.</summary>
        /// <returns>true if this object represents a canceled operation; otherwise, false.</returns>
        public Boolean IsCanceled
        {
            get
            {
                Object? o = _obj;
                if (o == null)
                    return false;
                if (o is Task<TResult> task)
                    return task.IsCanceled;
                return Unsafe.As<IValueTaskSource<TResult>>(o).GetStatus(_token) == ValueTaskSourceStatus.Canceled;
            }
        }

        /// <summary>Gets the result.</summary>
        /// <returns>The result.</returns>
        public TResult Result
        {
            [MethodImpl(256)]
            get
            {
                Object? o = _obj;
                if (o == null)
                    return _result;
                if (o is Task<TResult> task)
                    return task.GetAwaiter().GetResult();
                return Unsafe.As<IValueTaskSource<TResult>>(o).GetResult(_token);
            }
        }

        /// <summary>Creates an awaiter for this value.</summary>
        /// <returns>The awaiter.</returns>
        [MethodImpl(256)]
        public ValueTaskAwaiter<TResult> GetAwaiter()
        {
            return new ValueTaskAwaiter<TResult>(this);
        }

        /// <summary>Configures an awaiter for this value.</summary>
        /// <param name="continueOnCapturedContext">
        ///     true to attempt to marshal the continuation back to the captured context;
        ///     otherwise, false.
        /// </param>
        /// <returns>The configured awaiter.</returns>
        [MethodImpl(256)]
        public ConfiguredValueTaskAwaitable<TResult> ConfigureAwait(
            Boolean continueOnCapturedContext)
        {
            return new ConfiguredValueTaskAwaitable<TResult>(new ValueTask<TResult>(_obj!, _result, _token,
                continueOnCapturedContext));
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
        {
            if (IsCompletedSuccessfully)
            {
                TResult result = Result;
                if (result != null)
                    return result.ToString();
            }

            return String.Empty;
        }

        private sealed class ValueTaskSourceAsTask : TaskCompletionSource<TResult>
        {
            public ValueTaskSourceAsTask(IValueTaskSource<TResult> source, Int16 token)
            {
                _source = source;
                _token = token;
                source.OnCompleted(s_completionAction, this, token,
                    ValueTaskSourceOnCompletedFlags.None);
            }

            private static readonly Action<Object> s_completionAction = state =>
            {
                IValueTaskSource<TResult> source;
                if (!(state is ValueTaskSourceAsTask taskSourceAsTask) ||
                    (source = taskSourceAsTask._source) == null)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.state);
                }
                else
                {
                    taskSourceAsTask._source = null!;
                    ValueTaskSourceStatus status = source.GetStatus(taskSourceAsTask._token);
                    try
                    {
                        taskSourceAsTask.TrySetResult(source.GetResult(taskSourceAsTask._token));
                    }
                    catch (Exception ex)
                    {
                        if (status == ValueTaskSourceStatus.Canceled)
                            taskSourceAsTask.TrySetCanceled();
                        else
                            taskSourceAsTask.TrySetException(ex);
                    }
                }
            };

            private readonly Int16 _token;

            private IValueTaskSource<TResult> _source;
        }
    }

    public static class Unsafe
    {
        /// <summary>Casts the given object to the specified type.</summary>
        /// <param name="o">The object to cast.</param>
        /// <typeparam name="T">The type which the object will be cast to.</typeparam>
        /// <returns>The original object, casted to the given type.</returns>
        [NonVersionable]
        [MethodImpl(256)]
        public static T As<T>(Object o) where T : class
        {
            return (T) o;
        }
    }
}


namespace System.Runtime.Versioning
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method,
        AllowMultiple = false, Inherited = false)]
    internal sealed class NonVersionableAttribute : Attribute
    {
    }
}