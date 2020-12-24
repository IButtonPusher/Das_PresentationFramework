using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncResults
{
    internal static class CancellationTokenEx
    {
        static CancellationTokenEx()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            Canceled = cts.Token;
        }

        public static readonly CancellationToken Canceled;
    }
}