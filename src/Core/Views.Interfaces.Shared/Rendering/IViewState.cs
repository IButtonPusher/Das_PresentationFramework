using System;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;

namespace Das.Views.Rendering
{
    /// <summary>
    ///     Exposes a zoom level read-property
    /// </summary>
    public interface IViewState : //IStyleProvider,
                                  IDisplayMetrics
    {
        IColorPalette ColorPalette { get; }

    }
}