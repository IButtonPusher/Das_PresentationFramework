using System;
using System.Threading.Tasks;
using Das.Views.Styles;

namespace Das.Views.Rendering
{
    public interface IVisualContext : IStyleProvider
    {
        Double GetZoomLevel();

    }
}