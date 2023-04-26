using System;

namespace Das.Views.Rendering;

public interface IDisplayMetrics : IZoomLevelAware
{
   Single Density { get; }
}