using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Das.Views.Rendering;

public interface IMutableViewPerspective : IViewPerspective, IChangeTracking
{
   Boolean TrySetZoomLevel(Double zoom);
}