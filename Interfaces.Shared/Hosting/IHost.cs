using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views
{
    /// <summary>
    /// Base interface for a Control, Visual, etc that can host content
    /// </summary>
    public interface IHost
    {
        Boolean IsLoaded { get; }

        Size AvailableSize { get; }

        event Func<Task>? HostCreated;

        event Action<ISize>? AvailableSizeChanged;

        /// <summary>
        /// Runs the action on the host's UI thread while blocking the caller
        /// </summary>
        void Invoke(Action action);

        /// <summary>
        /// Asynchronously runs the action on the host's UI thread
        /// </summary>
        Task InvokeAsync(Action action);
    }
}
