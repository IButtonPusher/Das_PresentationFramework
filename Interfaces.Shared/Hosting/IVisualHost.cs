using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views
{
    public interface IVisualHost : IHost,
                                   IPositionOffseter
    {
    }

    public interface IVisualHost<TAsset> : IVisualHost
    {
        TAsset Asset { get; set; }
    }
}