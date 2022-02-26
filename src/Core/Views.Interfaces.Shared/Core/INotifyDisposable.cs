using System;
using System.Threading.Tasks;

namespace Das.Views.Core
{
    public interface INotifyDisposable : IDisposable
    {
        Boolean IsDisposed { get; }

        event Action<IVisualElement>? Disposed;
    }
}
