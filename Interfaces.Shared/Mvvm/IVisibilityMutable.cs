using System;

namespace Das.ViewsModels
{
    public interface IVisibilityMutable : IViewModel
    {
        Boolean  IsVisible { get; set; }
    }
}