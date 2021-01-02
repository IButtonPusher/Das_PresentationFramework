using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Hosting;
using Das.Views.Rendering;

namespace Das.Views
{
    /// <summary>
    ///     Represents the system specific UI Element that is hosting a root level View.
    ///     For example, a wpf Window, windows Form, a 'Control'
    /// </summary>
    public interface IViewHost : IVisualHost,
                                 IViewState,
                                 IChangeTracking,
                                 IDisposable
    {
        
        


        Thickness RenderMargin { get; }
        
        SizeToContent SizeToContent { get; }

        IVisualElement View { get; }

        new Double ZoomLevel { get; set; }

        void Invalidate();
    }

    public interface IViewHost<TAsset> : IViewHost, 
                                         IVisualHost<TAsset>
    {
        
    }
}