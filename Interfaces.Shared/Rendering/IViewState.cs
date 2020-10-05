using System;
using System.Threading.Tasks;
using Das.Views.Styles;

namespace Das.Views.Rendering
{
    /// <summary>
    ///     Exposes a zoom level read-property
    /// </summary>
    public interface IViewState : IStyleProvider
    {
        Double ZoomLevel { get; }
    }
}