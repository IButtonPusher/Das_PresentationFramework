using System;
using System.ComponentModel;

namespace Das.Views.Rendering
{
    public interface IMutableViewPerspective : IViewPerspective, IChangeTracking
    {
        Boolean TrySetZoomLevel(Double zoom);
    }
}