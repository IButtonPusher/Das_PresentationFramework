﻿using System;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;

namespace Das.Views.Rendering
{
    public interface IMeasureContext : IVisualContext
    {
        IViewState? ViewState { get; }

        Size GetLastMeasure(IVisualElement element);

        Size MeasureMainView(IVisualElement element, 
                             ISize availableSpace,
                             IViewState viewState);

        Size MeasureElement(IVisualElement element, 
                            ISize availableSpace);

        Size MeasureImage(IImage img);

        Size MeasureString(String s, Font font);

        /// <summary>
        /// The total amount of space available (e.g. size of the window, screen size of a mobile device)
        /// </summary>
        ISize ContextBounds { get; }
    }
}