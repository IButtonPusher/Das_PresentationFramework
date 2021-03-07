using System;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Rendering;

namespace Gdi.Shared.Static
{
    public readonly struct StaticViewState : IViewState
    {
        public StaticViewState(Double zoomLevel,
                               IColorPalette colorPalette,
                               Single density)
        {
            ZoomLevel = zoomLevel;
            ColorPalette = colorPalette;
            Density = density;
        }


        public IColorPalette ColorPalette { get; }

        public Double ZoomLevel { get; }

        public Single Density { get; }
    }
}