using System;
using Das.Views.Core.Geometry;
using Das.Views.Primitives;

namespace Das.Views.Panels;

public interface IContentVisual : IContentContainer,
                                  IContentPresenter,
                                  IFontVisual
{
   QuantifiedThickness Padding { get; set; }
}