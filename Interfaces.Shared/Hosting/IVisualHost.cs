using System;
using Das.Views.Core.Geometry;

namespace Das.Views
{
    public interface IVisualHost : IHost, IPositionOffseter
    {
        //IVisualRenderer Visual { get; }
    }

    public interface IVisualHost<TAsset> : IVisualHost
    {
        TAsset Asset { get; set; }
    }
}
