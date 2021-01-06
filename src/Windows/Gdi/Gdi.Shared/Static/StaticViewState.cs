using System;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Rendering;

namespace Gdi.Shared.Static
{
    public readonly struct StaticViewState : IViewState
    {
        public StaticViewState(Double zoomLevel,
                               IColorPalette colorPalette)
        {
            ZoomLevel = zoomLevel;
            ColorPalette = colorPalette;
        }


        public IColorPalette ColorPalette { get; }

        public Double ZoomLevel { get; }

    }
}