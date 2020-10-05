using System;
using System.Threading.Tasks;

namespace Das.ViewModels
{
    public interface IVisibilityMutable : IViewModel
    {
        Boolean IsVisible { get; set; }
    }
}