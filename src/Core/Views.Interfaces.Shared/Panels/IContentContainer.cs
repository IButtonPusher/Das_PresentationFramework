using System;
using System.Threading.Tasks;

namespace Das.Views.Panels
{
    public interface IContentContainer
    {
        IVisualElement? Content { get; set; }
    }
}