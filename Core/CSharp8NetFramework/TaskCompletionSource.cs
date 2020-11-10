using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

// ReSharper disable All
#pragma warning disable 8625
#pragma warning disable 8618

namespace AsyncResults
{
    public static class TaskCompletionSource
    {
        static TaskCompletionSource()
        {
            // Collect all necessary fields of a Task that needs to be reset.
            #if NETSTANDARD
            var mStateFlags = typeof(Task).GetTypeInfo().GetDeclaredField("m_stateFlags");
            var mContinuationObject = typeof(Task).GetTypeInfo().GetDeclaredField("m_continuationObject");
            var mTaskId = typeof(Task).GetTypeInfo().GetDeclaredField("m_taskId");
            var mStateObject = typeof(Task).GetTypeInfo().GetDeclaredField("m_stateObject");
            #else
            var mStateFlags = typeof(Task).GetField("m_stateFlags",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var mContinuationObject = typeof(Task).GetField("m_continuationObject",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var mTaskId = typeof(Task).GetField("m_taskId",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var mStateObject = typeof(Task).GetField("m_stateObject",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            #endif


            if (mStateFlags != null && mContinuationObject != null && mTaskId != null && mStateObject != null)
                try
                {
                    var defaultStateFlags = (Int32) mStateFlags.GetValue(new TaskCompletionSource<Int32>().Task);

                    var targetArg = Expression.Parameter(typeof(Task), "task");
                    var stateObjectArg = Expression.Parameter(typeof(Object), "stateObject");

                    var body = Expression.Block(
                        Expression.Assign(Expression.MakeMemberAccess(targetArg, mStateFlags),
                            Expression.Constant(defaultStateFlags, typeof(Int32))),
                        Expression.Assign(Expression.MakeMemberAccess(targetArg, mContinuationObject),
                            Expression.Constant(null, typeof(Object))),
                        Expression.Assign(Expression.MakeMemberAccess(targetArg, mTaskId),
                            Expression.Constant(0, typeof(Int32))),
                        Expression.Assign(Expression.MakeMemberAccess(targetArg, mStateObject), stateObjectArg),
                        Expression.Constant(0,
                            typeof(Int32)) // this can be anything of any type - lambda expression allows to compile Func<> only, but not an Action<>
                    );

                    var lambda = Expression.Lambda(body, targetArg, stateObjectArg);
                    _resetTaskFunc = (Func<Task, Object, Int32>) lambda.Compile();

                    // Do initial testing of the reset function
                    TestResetFunction();
                }
                catch
                {
                    // If something goes wrong, the feature just won't be enabled.
                    _resetTaskFunc = null;
                }
        }

        public static void DisableTaskCompletionSourceReUse()
        {
            _resetTaskFunc = null;
        }

        [MethodImpl(256)]
        public static void Reset<T>(ref TaskCompletionSource<T> taskCompletionSource, Object stateObject = null)
        {
            if (_resetTaskFunc != null && taskCompletionSource != null)
            {
                _resetTaskFunc(taskCompletionSource.Task, stateObject);
            }
            else
            {
                taskCompletionSource = new TaskCompletionSource<T>();
            }
        }

        private static void TestResetFunction()
        {
            var stateObject1 = new Object();
            var stateObject2 = new Object();
            var tcs = new TaskCompletionSource<Int32>();

            // Test reset before SetResult
            _resetTaskFunc(tcs.Task, stateObject1);
            if (tcs.Task.IsCanceled || tcs.Task.IsCompleted || tcs.Task.IsFaulted ||
                tcs.Task.AsyncState != stateObject1)
            {
                _resetTaskFunc = null;
                return;
            }

            // Test SetResult
            tcs.SetResult(123);
            if (tcs.Task.IsCanceled || !tcs.Task.IsCompleted || tcs.Task.IsFaulted)
            {
                _resetTaskFunc = null;
                return;
            }

            // Test reset before SetCanceled
            _resetTaskFunc(tcs.Task, stateObject2);
            if (tcs.Task.IsCanceled || tcs.Task.IsCompleted || tcs.Task.IsFaulted ||
                tcs.Task.AsyncState != stateObject2)
            {
                _resetTaskFunc = null;
                return;
            }

            // Test SetCanceled
            tcs.SetCanceled();
            if (!tcs.Task.IsCanceled || !tcs.Task.IsCompleted || tcs.Task.IsFaulted)
            {
                _resetTaskFunc = null;
                return;
            }

            // Test reset before SetException
            _resetTaskFunc(tcs.Task, stateObject1);
            if (tcs.Task.IsCanceled || tcs.Task.IsCompleted || tcs.Task.IsFaulted ||
                tcs.Task.AsyncState != stateObject1)
            {
                _resetTaskFunc = null;
                return;
            }

            // Test SetException
            var ex = new Exception();
            tcs.SetException(ex);
            if (tcs.Task.IsCanceled || !tcs.Task.IsCompleted || !tcs.Task.IsFaulted ||
                tcs.Task.Exception.InnerException != ex)
            {
                _resetTaskFunc = null;
                return;
            }
        }

        private static Func<Task, Object, Int32> _resetTaskFunc;
    }
}