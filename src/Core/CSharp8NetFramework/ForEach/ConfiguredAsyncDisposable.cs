using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct ConfiguredAsyncDisposable
    {
        private readonly IAsyncDisposable _source;

        private readonly Boolean _continueOnCapturedContext;

        internal ConfiguredAsyncDisposable(IAsyncDisposable source, Boolean continueOnCapturedContext)
        {
            _source = source;
            _continueOnCapturedContext = continueOnCapturedContext;
        }

        public ConfiguredValueTaskAwaitable DisposeAsync()
        {
            return _source.DisposeAsync().ConfigureAwait(_continueOnCapturedContext);
        }
    }
}