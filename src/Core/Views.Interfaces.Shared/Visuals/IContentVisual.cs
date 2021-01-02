using System;
using Das.Views.Core.Geometry;

namespace Das.Views.Panels
{
    public interface IContentVisual : IContentContainer,
                                      IContentPresenter
    {
        QuantifiedThickness Padding { get; set; }
    }
}
