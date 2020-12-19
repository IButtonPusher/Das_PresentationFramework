﻿using System;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;

namespace Das.Views.Rendering
{
    public interface IMeasureContext : IVisualContext
    {
        /// <summary>
        ///     The total amount of space available (e.g. size of the window, screen size of a mobile device)
        /// </summary>
        ValueSize ContextBounds { get; }

        IViewState? ViewState { get; }

        /// <summary>
        /// Applies the amount needed by styles (margin, border) then
        /// calls element.Measure with the reduced size, if applicable.
        /// </summary>
        /// <returns>The amount needed for margin/borders + what the element asked for</returns>
        ValueSize MeasureElement(IVisualElement element,
                            IRenderSize availableSpace);

        ValueSize MeasureImage(IImage img);

        ValueSize MeasureMainView(IVisualElement element,
                                  IRenderSize availableSpace,
                                  IViewState viewState);

        ValueSize MeasureString(String s,
                                IFont font);

        ValueSize GetStyleDesiredSize(IVisualElement element);
    }
}