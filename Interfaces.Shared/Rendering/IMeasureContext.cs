using System;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;

namespace Das.Views.Rendering
{
    public interface IMeasureContext : IVisualContext
    {
        IViewState ViewState { get; set; }

        Size MeasureString(String s, Font font);

        Size MeasureImage(IImage img);

        Size MeasureElement(IVisualElement element, ISize availableSpace);

        Size GetLastMeasure(IVisualElement element);
    }
}