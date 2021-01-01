﻿using System;
using System.Threading.Tasks;
using Das.Container;
using Das.Views.Construction;
using Das.Views.Controls;
using Das.Views.Rendering;

namespace Das.Views
{
    public interface IRenderKit
    {
        IMeasureContext MeasureContext { get; }

        IRenderContext RenderContext { get; }

        IResolver Container { get; }

        IVisualBootstrapper VisualBootstrapper { get; }
        
        IViewInflater ViewInflater { get; }

        void RegisterSurrogate<T>(Func<IVisualElement, IVisualSurrogate> builder)
            where T : IVisualElement;
    }
}