// System.Runtime.CompilerServices.AsyncIteratorStateMachineAttribute

using System;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
    /// <summary>Indicates whether a method is an asynchronous iterator.</summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class AsyncIteratorStateMachineAttribute : StateMachineAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="T:System.Runtime.CompilerServices.AsyncIteratorStateMachineAttribute" /> class.
        /// </summary>
        /// <param name="stateMachineType">
        ///     The type object for the underlying state machine type that's used to implement a state
        ///     machine method.
        /// </param>
        public AsyncIteratorStateMachineAttribute(Type stateMachineType)
            : base(stateMachineType)
        {
        }
    }
}