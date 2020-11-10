#if NET40

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace System.Runtime.ExceptionServices
{
    // This class defines support for separating the exception dispatch details
    // (like stack trace, watson buckets, etc) from the actual managed exception
    // object. This allows us to track error (via the exception object) independent
    // of the path the error takes.
    //
    // This is particularly useful for frameworks that wish to propagate 
    // exceptions (i.e. errors to be precise) across threads.
    public sealed class ExceptionDispatchInfo
    {
        //private readonly Exception.DispatchState _dispatchState;

        public ExceptionDispatchInfo(Exception exception)
        {
            SourceException = exception;
            //_dispatchState = exception.CaptureDispatchState();
        }

        // Return the exception object represented by this ExceptionDispatchInfo instance
        public Exception SourceException { get; }

        // This static method is used to create an instance of ExceptionDispatchInfo for
        // the specified exception object and save all the required details that maybe
        // needed to be propagated when the exception is "rethrown" on a different thread.
        public static ExceptionDispatchInfo Capture(Exception source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return new ExceptionDispatchInfo(source);
        }

        // When a framework needs to "Rethrow" an exception on a thread different (but not necessarily so) from
        // where it was thrown, it should invoke this method against the ExceptionDispatchInfo
        // created for the exception in question.
        //
        // This method will restore the original stack trace and bucketing details before throwing
        // the exception so that it is easy, from debugging standpoint, to understand what really went wrong on
        // the original thread.

        [StackTraceHidden]
        public void Throw()
        {
            // Restore the exception dispatch details before throwing the exception.
            //_exception.RestoreDispatchState(_dispatchState);
            throw SourceException;
        }

        // Throws the source exception, maintaining the original bucketing details and augmenting
        // rather than replacing the original stack trace.

        public static void Throw(Exception source)
        {
            Capture(source).Throw();
        }
    }
}

#endif