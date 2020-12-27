using System;

namespace Das.Views.Controls
{
    public interface IVisualTemplate //: IContentContainer
    {
        IVisualElement? Content { get; }
    }
}
