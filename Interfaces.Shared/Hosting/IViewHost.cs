using Das.Views.Rendering;
using System;
using System.ComponentModel;
using Das.Views.Core.Geometry;
using Das.Views.Panels;
using Das.Views.Styles;

namespace Das.Views
{
    /// <summary>
    /// Represents the system specific UI Element that is hosting a root level View.
    /// For example, a wpf Window, windows Form, a 'Control'
    /// </summary>
    public interface IViewHost : IHost, IViewState, IChangeTracking
    {
        IView View { get; }

        IStyleContext StyleContext { get; }

        new Double ZoomLevel { get; set; }

        IViewModel DataContext { get; set; }

        void Invalidate();

        Thickness RenderMargin { get; }
    }

    public interface IViewHost<TAsset> : IViewHost
    {
        TAsset Asset { get; set; }
    }
}