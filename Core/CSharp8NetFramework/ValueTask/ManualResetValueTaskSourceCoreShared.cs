using System;
using System.Threading.Tasks;

// ReSharper disable All


namespace System.Threading.Tasks.Sources
{
    internal static class ManualResetValueTaskSourceCoreShared
    {
        private static void CompletionSentinel(Object _)
        {
            throw new InvalidOperationException();
        }

        internal static readonly Action<Object> s_sentinel = new Action<Object>(CompletionSentinel);
    }
}