using System;
using System.Threading.Tasks;

namespace Das.Views.Panels
{
    public interface IVisualFinder
    {
        Boolean Contains(IVisualElement element);
    }
}