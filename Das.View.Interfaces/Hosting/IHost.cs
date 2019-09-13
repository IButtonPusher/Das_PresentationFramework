using System;
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

        event EventHandler HostCreated;

        event EventHandler AvailableSizeChanged;

        /// <summary>
        /// Runs the action on the host's UI thread while blocking the caller
        /// </summary>
        /// <param name="action"></param>
        void Invoke(Action action);
    }
}
