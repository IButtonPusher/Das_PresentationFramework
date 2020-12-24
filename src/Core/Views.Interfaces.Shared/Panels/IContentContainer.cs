using System;
using System.Threading.Tasks;

namespace Das.Views.Panels
{
    public interface IContentContainer //: IVisualElement
    {
        IVisualElement? Content { get; set; }
    }
}