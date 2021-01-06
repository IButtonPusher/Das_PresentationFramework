using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Panels
{
    public interface IBoundElementContainer
    {
        Size AvailableSize { get; }

        //IViewModel? DataContext { get; set; }

        IVisualElement Element { get; set; }

        //IStyleContext StyleContext { get; set; }
    }
}