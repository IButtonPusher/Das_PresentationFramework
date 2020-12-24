using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Panels
{
    public interface IContentVisual : IContentContainer,
                                      IContentPresenter,
                                      IVisualElement
    {
    }
}
