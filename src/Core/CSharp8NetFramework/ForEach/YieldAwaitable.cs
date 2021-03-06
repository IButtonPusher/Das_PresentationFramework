﻿using System;
using System.Threading.Tasks;

#if NET40
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
// ReSharper disable All

namespace Microsoft.Runtime.CompilerServices
{
    /// <summary>Provides an awaitable context for switching into a target environment.</summary>
    /// <remarks>This type is intended for compiler use only.</remarks>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct YieldAwaitable
    {
        /// <summary>Gets an awaiter for this <see cref="T:Microsoft.Runtime.CompilerServices.YieldAwaitable" />.</summary>
        /// <returns>An awaiter for this awaitable.</returns>
        /// <remarks>This method is intended for compiler user rather than use directly in code.</remarks>
        public YieldAwaiter GetAwaiter()
        {
            return new YieldAwaiter();
        }

        /// <summary>Provides an awaiter that switches into a target environment.</summary>
        /// <remarks>This type is intended for compiler use only.</remarks>
        [StructLayout(LayoutKind.Sequential, Size = 1)]
        public struct YieldAwaiter : ICriticalNotifyCompletion
        {
            /// <summary>A completed task.</summary>
            private static readonly Task s_completed = TaskEx.FromResult(0);

            /// <summary>Gets whether a yield is not required.</summary>
            /// <remarks>This property is intended for compiler user rather than use directly in code.</remarks>
            public Boolean IsCompleted => false;

            /// <summary>Posts the <paramref name="continuation" /> back to the current context.</summary>
            /// <param name="continuation">The action to invoke asynchronously.</param>
            /// <exception cref="T:System.InvalidOperationException">The awaiter was not properly initialized.</exception>
            public void OnCompleted(Action continuation)
            {
                s_completed.GetAwaiter().OnCompleted(continuation);
            }

            /// <summary>Posts the <paramref name="continuation" /> back to the current context.</summary>
            /// <param name="continuation">The action to invoke asynchronously.</param>
            /// <exception cref="T:System.InvalidOperationException">The awaiter was not properly initialized.</exception>
            public void UnsafeOnCompleted(Action continuation)
            {
                s_completed.GetAwaiter().UnsafeOnCompleted(continuation);
            }

            /// <summary>Ends the await operation.</summary>
            public void GetResult()
            {
            }
        }
    }
}

#endif