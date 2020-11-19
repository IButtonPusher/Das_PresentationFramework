using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Mvvm;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views
{
    /// <summary>
    ///     Represents the system specific UI Element that is hosting a root level View.
    ///     For example, a wpf Window, windows Form, a 'Control'
    /// </summary>
    public interface IViewHost : IVisualHost,
                                 IViewState,
                                 IChangeTracking
    {
        IViewModel? DataContext { get; set; }

        Thickness RenderMargin { get; }

        IStyleContext StyleContext { get; }

        IView View { get; }

        new Double ZoomLevel { get; set; }

        void Invalidate();
    }

    public interface IViewHost<TAsset> : IViewHost, IVisualHost<TAsset>
    {
        //TAsset Asset { get; set; }
    }
}